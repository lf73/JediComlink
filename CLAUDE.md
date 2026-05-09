# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

JediComlink is a Windows Forms (.NET Framework 4.7.2, C# 8) application that reads, edits, writes, and emulates the codeplug of Motorola Jedi-series two-way radios (MTS2000 etc.) over a serial RIB cable. It also flashes radio firmware and contains a software emulator that impersonates a radio on a COM port.

## Build & run

There is no test suite, no CI, and no `dotnet`-friendly project format. Builds require MSBuild + .NET Framework 4.7.2 targeting pack on Windows (or a Windows VM from WSL):

- Open `JediComlink.sln` in Visual Studio 2019+ and build, **or**
- `msbuild JediComlink.sln /p:Configuration=Debug` (or `Release`)
- Run `JediComlink/bin/Debug/JediComlink.exe`

NuGet uses the legacy `packages.config` format (System.Memory, System.Buffers etc.). Run `nuget restore` before the first build.

`JediCommon/JediDefinitions.json` is loaded with a relative path from the static constructor of `JediCommon.Common`, so it **must be in the working directory at runtime** (copied to bin output by the project) — otherwise model/firmware lookup throws on startup.

Linux/WSL caveat: this targets `System.Windows.Forms` and `System.IO.Ports.SerialPort`, so it cannot run or be meaningfully tested on Linux. Treat WSL as edit-only.

## Solution layout

Six projects, all sharing namespace `JediComlink.*`:

- **JediComlink** — WinForms shell (`Home.cs` is the only form). References JediCodeplug, JediEmulator, JediFlash. Drives all user actions.
- **JediCodeplug** — the codeplug object model (the bulk of the code). Parses/serializes the binary blob.
- **JediCommunication** — serial-port transport. Implements SB9600 (the Motorola control bus) and SBEP (the byte-level memory read/write protocol layered over it).
- **JediEmulator** — the inverse side of JediCommunication: listens on a COM port and pretends to *be* a radio, replying to SB9600/SBEP requests against an in-memory codeplug. Useful for testing without hardware.
- **JediFlash** — flash-mode read of the radio's firmware ROM (page-by-page over a different command set than the codeplug). Includes XOR-encoded boot code (`BootCode.cs` + decode in `FlashCom.cs`, see `docs/BootCode.md`).
- **JediCommon** — shared helpers (`Common.cs` static class, hex utilities, `Model`, `Firmware`, `JediDefinitions.json` loader).

## The codeplug data model — most important architectural concept

The radio's EEPROM is one ~33 KB binary blob (`0x8200` bytes, internal + external EEPROM concatenated). It is a tree of nested **Blocks**, each identified by a one-byte ID (`0x01`, `0x02`, …, `0x30`, `0xDE`, …). Folders:

- `JediCodeplug/InternalBlocks/Block01.cs` … `Block11.cs` — internal-EEPROM blocks
- `JediCodeplug/ExternalBlocks/Block30.cs` … `BlockDE.cs` — external-EEPROM blocks

### Wire format of every block

```
[length:1] [id:1] [payload:length-1] [checksum:1]
```

The checksum is `(-0x55 + length + id + sum(payload)) & 0xFF` — see `Block.Serializer`. The `-0x55` magic must be preserved.

### Vectors

Children are not embedded; they are referenced by 2-byte big-endian **vectors** at fixed offsets inside a parent's payload. A vector of `0x0000` means "no child." `Block.Deserialize<T>` and `Block.SerializeChild` are the two helpers that walk vectors. When you serialize, child addresses are computed live and written into the parent's vector slots before the parent is checksummed.

### Adding or changing a block

A new `BlockXX.cs`:

1. Inherits `JediCodeplug.Block` (or `BlockLong` if its length field is two bytes — this exists; check existing examples).
2. Overrides `Id`, `Description`, `Deserialize`, `Serialize`.
3. Declares each child as a public property of the child's block type. Lists of children use `IEnumerable<Block>` / `List<BlockXX>`.
4. Defines field offsets as `private const int FOO = 0x..;` at the top.

The parent block must add a `private const int BLOCK_XX_VECTOR = 0x..;` and call `Deserialize<BlockXX>(...)` / `SerializeChild(BlockXX, BLOCK_XX_VECTOR, ...)`.

### Reflection-driven UI / dump

`Home.UpdateCodeplug` and `Block.GetTextDump` walk every public property reflectively to build the TreeView and text dump. Two consequences:

- A `[DisplayName]` / `[Description]` / `[TypeConverter]` attribute on a block property changes its appearance in the PropertyGrid for free.
- Properties named `Id`, `Description`, `Contents` are filtered out of dumps.
- `IEnumerable<Block>`-typed properties are recursed; everything else is treated as a leaf value.

Whenever you add a property to a block, decide if it should be a leaf (formatted by a `TypeConverter`, e.g. `HexByteArrayTypeConverter`) or a child block.

### Round-trip invariant

The serializer is expected to round-trip: `new Codeplug(bytes).Serialize()` should produce bytes equal to the input for known-good codeplugs. There is commented-out round-trip diff code in `Home.NormalWriteButton_Click` — useful when debugging a new block. If a known-good codeplug fails to round-trip, the block parser is wrong; do not write to a real radio in that state.

## Serial protocols

`JediCommunication.Com` exposes the high-level operations the rest of the app uses (`Read`, `Write`, `EnterProgrammingMode`, `GetFirmwareVersion`, `Reset`). Internally:

- **SB9600** — 5-byte fixed packets on the SB9600 bus (`SB9600Message.cs`, `SB9600Messages.cs` for the named commands). Used to put the radio into programming mode and to enter SBEP mode. Bytes are TX/RX-shorted, so the sender always reads its own echo back before the radio's reply.
- **SBEP** — variable-length packets layered on top, used for byte-level memory reads (opcode `0x11`) and writes (opcode `0x17`). 8-bit checksum. `Com.Read(addr, len)` and `Com.Write(addr, len, buf)` are the entry points.

Standard codeplug session: `EnterProgrammingMode()` → SB9600 → `EnterSbep()` (auto on first read/write) → many `Read`/`Write` in 0x20-byte chunks → `ExitSbepMode()` → `Reset()`. `Codeplug.ReadFromRadio` / `WriteToRadio` are the canonical examples.

Flash mode (`JediFlash.FlashCom`) is a separate, higher-baud channel using the patched bootloader described in `docs/BootCode.md`. The two boot blobs in `BootCode.cs` are base64-encoded and XOR'd with `0x55`; **do not "clean up" the constants** — the XOR is intentional obfuscation against casually scraping Motorola's ROM.

## Auth code

`JediCodeplug/AuthCode.cs` reimplements the Motorola codeplug authentication algorithm: a permutation (`ORDER`) over a 0x50-byte buffer composed of model + factory code + flash signature + Block10 feature/flashcode bytes, with a small XOR/shift loop. The auth code is required after most edits or the radio rejects the codeplug; call `Codeplug.RecalculateAuthCode()` before writing edits back. This requires a known firmware signature — `JediDefinitions.json` ships them per firmware version.

## Filename convention

`Codeplug.GetProposedFileName()` and `Codeplug(string path)` both encode/parse the convention:

```
{serial}_{yyyy-MM-dd_HH-mm-ss}_R{firmware:00.00}_{factoryCodeHex}.bin
```

Reading from a radio writes a file in this format to the current working directory automatically.

## Hard-coded values to know

- `Emulator.Start()` opens **`COM4`** unconditionally — the UI does not pass through `EmulatorComPortComboBox`. Edit the literal if testing on a different port (or fix it).
- `Codeplug.Serialize()` allocates a fixed `0x8200`-byte buffer (internal + external EEPROM total). Larger codeplugs would silently truncate.
- `Block` checksum uses the `-0x55` constant; same constant is reproduced in the emulator and parser sides.

## Things that look like bugs but aren't (don't "fix" without asking)

- **`x ^= 0x55` in a `.Select`** in `FlashCom.cs` — yes, the assignment-in-expression is intentional; that's how the obfuscation is decoded.
- **Comments in `Home.button2_Click`** referencing `WhoreTest` / various blocks set to `null` — this is the developer's experimental cleanup pad for a specific radio. It is invoked from a "Fix" button. Treat it as scratch code, not a real feature.
- **`packages.config`-style NuGet** rather than `<PackageReference>` — keep it; mixing the two breaks the build for this project format.
