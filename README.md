# JediComlink

An open-source codeplug, firmware, and recovery tool for Motorola Jedi-series two-way radios — most notably the **MTS2000** portable. JediComlink replaces the long-obsolete factory CPS (Customer Programming Software) with a transparent, scriptable .NET application that can read, edit, write, emulate, and unbrick these radios from a modern PC.

## Why this matters today

The MTS2000 was a flagship Motorola public-safety portable in the 1990s and is many years past end-of-production. It is no longer relevant for commercial fleet service, but the hardware is rugged, plentiful on the surplus market, and still genuinely useful for:

- **Amateur radio.** The VHF and UHF variants cover ham allocations and remain one of the toughest, best-sounding portables available to a hobbyist on a budget.
- **800 MHz volunteer/auxiliary use.** Volunteer organizations operating on 800 MHz interop or mutual-aid channels — where authorized — can put cheap surplus MTS2000s into legitimate service without the original five-figure programming-software contract.
- **800 MHz licensed users.** The FCC continues to issue licenses in the 800 MHz band, the radio remains a credible option for users who want commercial-grade hardware without commercial-grade pricing.

## What JediComlink does

| Capability | Notes |
|---|---|
| Read codeplug from radio | Over a standard RIB cable on any modern serial/USB-serial port. |
| Write codeplug back to radio | With on-the-fly auth code recalculation so the radio accepts the edited blob. |
| Open / save codeplug `.bin` files | Round-trip safe — you can edit, save, share, and reload. |
| Edit the codeplug as a structured tree | Property-grid UI driven by the actual block hierarchy, not a hex editor. |
| Read raw firmware via flash mode | Pulls the on-board firmware ROM out of the radio. |
| Emulate a radio | Talks SB9600 / SBEP back to a PC pretending to be a real MTS2000 — useful for testing CPS-side changes without risking hardware. |
| Recover bricked radios | Bootstrap-mode upload of a patched bootloader to revive a radio that won't boot or shows a software fault code. |

## Toolproof and why the auth code matters

Motorola's "Toolproof" mechanism was the firmware's way of refusing to honor a codeplug it didn't trust with unauthorized feature enablement. Each codeplug carries an authentication code derived from the radio's model string, factory code, the firmware's own signature bytes, and a feature/flashcode block. Edit any of those without recomputing the auth code and the radio will reject the codeplug — historically a one-way trip back to the dealer.

JediComlink defeats Toolproof by reimplementing the auth code algorithm in the open. When you load a codeplug, the program checks the auth code; when you save edits back to the radio, it recomputes and writes a valid one. Practically:

- A read from a working radio captures the firmware signature needed for the calculation.
- Codeplugs from unknown firmware versions can still be opened — they just can't be re-signed until that firmware's signature bytes are added to `JediCommon/JediDefinitions.json` (or read from the radio in flash mode).
- Edits that would historically have soft-bricked the radio are now safe.

## Recovering "bricked" radios

If a radio shows a software-related fault code on power-up, refuses to enter programming mode, or appears completely dead but still draws current, it is almost always recoverable. JediComlink can drive the radio's CPU into bootstrap mode and upload a patched boot ROM directly into RAM, bypassing whatever is broken in flash. From bootstrap you can:

- Re-flash firmware.
- Rewrite the codeplug from scratch.
- Read the factory code and other flash regions that aren't accessible from normal programming mode.

Details and the boot-image memory map live in [`docs/BootCode.md`](docs/BootCode.md).

## Project layout

The solution is six C# projects under `JediComlink.sln`. Each one is independently buildable and has a single, well-defined job:

### `JediComlink` — the WinForms shell

The user-facing application. A single `Home.cs` form with tabs for normal codeplug operations, flash-mode firmware reads, and the emulator. Picks COM ports, drives reads/writes, hosts the codeplug tree-view and PropertyGrid editor, and writes timestamped `.bin` files to the working directory.

### `JediCodeplug` — the codeplug object model

The heart of the project. The codeplug is an ~33 KB binary blob organized as a tree of **blocks**, each tagged by a one-byte ID, prefixed with a length, and terminated by an 8-bit checksum. `Block01` is the internal-EEPROM root; `Block30` is the external-EEPROM root; everything else hangs off those by 2-byte vector references. The folder structure mirrors that split:

- `InternalBlocks/Block01.cs` … `Block11.cs` — radio identity, model, serial, auth code, mode lists.
- `ExternalBlocks/Block30.cs` … `BlockDE.cs` — channels, scan lists, signaling, display strings, and the long tail of feature blocks.

`AuthCode.cs` reimplements the Toolproof signing algorithm. `TypeDescriptors.cs` makes byte arrays and integers display as hex in the UI without manual formatting in every block. `SerialDecoder.cs` and `ModelDecoder.cs` interpret the radio's model and serial number strings.

### `JediCommunication` — the serial transport

Two protocols, one COM port:

- **SB9600** (`SB9600Message.cs`, `SB9600Messages.cs`) — 5-byte fixed packets on the Motorola control bus. Used to put the radio into programming mode and request firmware version.
- **SBEP** (`SbepMessage.cs`) — variable-length, layered on top of SB9600 once the radio is in programming mode. Used for actual byte-level memory reads (`0x11`) and writes (`0x17`).

`Com.cs` is the public facade: `EnterProgrammingMode()`, `Read(address, length)`, `Write(address, length, buffer)`, `Reset()`. The TX/RX lines on a RIB are shorted, so every send is followed by an echo read of the sender's own bytes before the radio's actual reply.

### `JediEmulator` — a software radio

The mirror image of `JediCommunication`. Listens on a serial port and answers SB9600 / SBEP requests as if it were a real MTS2000, backed by an in-memory codeplug. Lets you exercise programming software (including JediComlink itself) end-to-end without a radio on the bench. Useful for development, regression testing, and reverse-engineering protocol corner cases.

### `JediFlash` — firmware read and bootstrap recovery

A separate, higher-baud channel for talking to the radio's flash chip rather than its EEPROM. Reads the entire firmware image one page at a time, identifies the firmware version by locating the embedded copyright string, and writes a `.bin` file named after the version. This is also where bricked-radio recovery lives: `BootCode.cs` contains a base64-encoded, XOR-obfuscated copy of the patched MTSXBoot bootloader, and `FlashCom.cs` decodes it and uploads it over the serial link in two stages (1 KB bootstrap at 3600 baud, then 4.5 KB boot loader at 115 200 baud).

### `JediCommon` — shared definitions

Cross-project utilities and the radio knowledge base:

- `JediDefinitions.json` — every known firmware version with its signature bytes and MD5 (required for auth code recalculation), plus the model number → name/band/description mapping.
- `Common.cs` — hex string conversion, model/firmware lookup.
- `Model.cs`, `Firmware.cs`, `JediDefinition.cs` — POCOs for the JSON.

`JediDefinitions.json` is loaded by relative path at startup, so it must sit next to the executable when the program runs. Adding support for a new firmware version is usually as simple as appending a record here.

## Build and run

Requires Windows + Visual Studio 2019 or newer + .NET Framework 4.7.2 targeting pack. NuGet is in legacy `packages.config` mode.

```
nuget restore JediComlink.sln
msbuild JediComlink.sln /p:Configuration=Release
JediComlink\bin\Release\JediComlink.exe
```

A USB-to-serial adapter and a RIB (or RIB-less cable) are required to actually talk to a radio. Without hardware, the emulator project plus a virtual COM-port pair is enough to exercise most of the application.

## Status and contributions

This is a reverse-engineered, community-built tool. Block parsers exist for the fields most users care about, and large stretches of the codeplug are still represented as opaque byte arrays awaiting documentation. New `BlockXX.cs` files, firmware signature contributions, and model entries in `JediDefinitions.json` are all welcome — see `CLAUDE.md` for the conventions block parsers follow.

## Disclaimer

JediComlink is not affiliated with or endorsed by Motorola Solutions. Use only on radios you own and only on frequencies you are authorized to operate on. The MTS2000 is a Part 90 commercial radio; transmitting on it requires an appropriate FCC license (or, in the case of amateur use, programming the radio for an amateur band you are licensed in).