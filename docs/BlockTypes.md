# Codeplug block types

The MTS2000 codeplug is a tree of variable-length records ("blocks"), each
identified by a one-byte ID and parsed by a corresponding
[`BlockXX.cs`](../JediCodeplug/) file. This document lists every block type
and the fields its parser decodes today.

For the on-the-wire framing (length / id / payload / checksum and the
2-byte vector references that link blocks together) see
[`../CLAUDE.md`](../CLAUDE.md#wire-format-of-every-block).

A note on offsets: every offset below is the byte position **inside the
payload** of the block, not the absolute address in EEPROM. A value like
`0x02–0x0B` means bytes 2 through 11 of the payload.

## Quick index

### Internal codeplug (root: Block 01)

| ID | Description | Status |
|----|---|---|
| `01` | Internal Radio | fully decoded |
| `02` | Hardware Config Radio | mostly decoded |
| `03` | Hardware Config Vector | child vectors only |
| `04` | Hardware Config Conventional | undecoded |
| `05` | Hardware Config Secure | mic gain decoded, rest unknown |
| `06` | Softpot Vector | child vectors only |
| `07` | Softpot Radio | fully decoded |
| `08` | Softpot Interpol Vector | list container |
| `09` | Softpot Interpol | fully decoded |
| `0A` | Softpot B/W Vector | fixed list of 4 |
| `0B` | Softpot B/W | undecoded |
| `0C` | Unknown | undecoded |
| `0D` | Hardware Config MDC | undecoded |
| `0E` | Test Channel Table | partially decoded |
| `0F` | AD Switch Level | TX/RX VCO crossover decoded |
| `10` | Feature Descriptor Block | extensively decoded (Toolproof source) |
| `11` | AD Button Level | undecoded |

### External codeplug (root: Block 30)

| ID | Description | Status |
|----|---|---|
| `30` | External Radio | fully decoded |
| `31` | Radio Wide | undecoded |
| `34` | Bus Option Vector | undecoded |
| `35` | Unknown | undecoded |
| `36` | Display & Menu | child vector decoded |
| `37` | Zone Chan Text Vector | list container |
| `38` | Zone Chan Text | undecoded (variable-length) |
| `39` | Phone List Vector | child vector only |
| `3A` | Phone List | undecoded (variable-length) |
| `3B` | DTMF Codes List | undecoded |
| `3C` | Hardware Button Configuration | undecoded |
| `3D` | Signaling Vector | child vectors only |
| `3E` | Conv Configuration | child vectors only |
| `3F` | Control Head | undecoded |
| `41` | Menu Item | undecoded |
| `42` | Unknown | child vector only |
| `43` | Unknown | undecoded |
| `44` | MDC Configuration | child vectors only |
| `47` | MDC System Vector | list container |
| `48` | MDC System | undecoded |
| `4A` | Trunk Configuration | child vectors only |
| `4B` | Trunk System Vector | list container |
| `51` | Scan Configuration | child vector only |
| `52` | Scan List Vector | list container |
| `53` | Scan List | undecoded |
| `54` | Zone Chan Assignment | undecoded |
| `55` | Personality Vector | list container |
| `56` / `62` | Conv / Trunk-Test Personality | partial |
| `57` | Zone Chan TG Table | undecoded |
| `58` | Trunk Call List Vector | child vector only |
| `59` | Trunk Call List | undecoded |
| `5A` | Trunk System | child vectors + Dynamic Trunk |
| `61` | Unknown | undecoded |
| `62` | Unknown | child vectors only |
| `63` | Trunk II TG | conditional Block 65 link |
| `65` | SP Tone Table / Trunk II FS Channel | undecoded |
| `67` | Trunk Pers Emergency | undecoded |
| `73` | Trunk Status List / Zone Vector | undecoded |
| `74` | Trunk Message List / Zone Chan TG Accel | undecoded |
| `8E` | Status List Vector | child vector only |
| `8F` | Status List | undecoded |
| `90` | Message List Vector | child vector only |
| `91` | Message List | undecoded |
| `92` | MDC Call List Vector | child vector only |
| `93` | MDC Call List | undecoded |
| `9F` | MDC Repeater ID Table | undecoded |
| `A0` | Aux Signalling | child vectors only |
| `A1` | Singletone System Vector | list container |
| `A2` | Singletone System | undecoded |
| `A3` | Singletone List | undecoded |
| `A4` | Unknown | list container |
| `A5` | Unknown | undecoded |
| `A9` | Unknown | undecoded |
| `DE` | Trunk II FS Channel Rebanded | undecoded |

---

## Internal blocks

### Block 01 — Internal Radio
Root of the internal codeplug. Holds the radio's identity and the vectors
that anchor every other internal block. Payload length: `0x3A`.

| Offset | Field | Type | Notes |
|---|---|---|---|
| 0x00–0x01 | External Codeplug Vector | uint16 BE | Usually 512 for 800 MHz, 640 for others. Internal EEPROM is 512 bytes; the internal codeplug can overflow into external EEPROM. |
| 0x02–0x0B | Serial | ASCII (10) | |
| 0x0C–0x1B | Model | ASCII (16) | |
| 0x1C–0x1D | Internal Codeplug Version | uint16 BE | |
| 0x1E–0x1F | Internal Codeplug Size | uint16 BE | |
| 0x20–0x23 | Unknown 1 | byte[4] | |
| 0x24–0x25 | → Block 02 vector | ptr | Hardware Config Radio |
| 0x26–0x27 | → Block 56 vector | ptr | (early/internal-side reference) |
| 0x28–0x2B | Unknown 2 | byte[4] | |
| 0x2C–0x2D | → Block 10 vector | ptr | Feature Descriptor Block |
| 0x2E–0x2F | Unknown 3 | byte[2] | |
| 0x30–0x39 | Auth Code | byte[10] | Toolproof signature; recomputed via `AuthCode.Calculate` |

### Block 02 — Hardware Config Radio
Length: `0x31` (`0x32` on some VHF models).

| Offset | Field | Type | Notes |
|---|---|---|---|
| 0x00–0x01 | → Block 06 vector | ptr | Softpot Vector |
| 0x02–0x03 | → Block 03 vector | ptr | Hardware Config Vector |
| 0x04–0x05 | → Block 0C vector | ptr | (Unknown 0C) |
| 0x06–0x12 | Unknown 1 | byte[13] | |
| 0x13 | Mic Gain | byte | Bit-packed: Internal Mic Pre-Amp Gain (0–7), External Mic Pre-Amp Gain (0–7), Unknown Mic Bits |
| 0x14–0x1C | Unknown 2 | byte[9] | |
| 0x1D | RSSI Alignment | byte | N/A on 800 MHz models |
| 0x1E–0x1F | Unknown 3 | byte[2] | |
| 0x20–0x21 | → Block 0F vector | ptr | AD Switch Level |
| 0x22–0x2A | Unknown 4 | byte[9] | |
| 0x2B–0x2C | → Block 11 vector | ptr | AD Button Level |
| 0x2D–0x30 | Unknown 5 | byte[4] | |
| 0x31 | Unknown Extra Byte | byte | Present on VHF (`HasExtraByte`); reason TBD |

### Block 03 — Hardware Config Vector
Length: `0x06`. Pure pointer block.

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 04 vector — Hardware Config Conventional |
| 0x02–0x03 | → Block 05 vector — Hardware Config Secure |
| 0x04–0x05 | → Block 0D vector — Hardware Config MDC |

### Block 04 — Hardware Config Conventional
Length: `0x0B`. Payload not yet decoded — exposed as `Unknown1: byte[11]`.

### Block 05 — Hardware Config Secure
Length: `0x07`.

| Offset | Field | Type | Notes |
|---|---|---|---|
| 0x00 | Unknown 1 | byte | |
| 0x01 | Mic Gain | byte | Internal Mic Pre-Amp Gain (0–7), External Mic Pre-Amp Gain (0–7), Unknown Mic Bits |
| 0x02–0x06 | Unknown 2–6 | byte each | |

### Block 06 — Softpot Vector
Length: `0x06`. Pure pointer block.

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 07 vector — Softpot Radio |
| 0x02–0x03 | → Block 08 vector — Softpot Interpol Vector |
| 0x04–0x05 | → Block 0A vector — Softpot B/W Vector |

### Block 07 — Softpot Radio
Length: `0x14` (sometimes `0x15`).

| Offset | Field | Type | Notes |
|---|---|---|---|
| 0x00 | TX Reference Oscillator | byte | |
| 0x01 | Signaling DTMF | byte | 0–31 |
| 0x02 | Signaling High Speed | byte | 0–31 |
| 0x03 | Signaling MDC1200 | byte | 0–31 |
| 0x04 | Unknown 1 | byte | |
| 0x05 | TX Deviation Reference 12.5 kHz | byte | 0–127, N/A on 800 MHz |
| 0x06 | TX Deviation Reference 20.0 kHz | byte | 0–127 |
| 0x07–0x0F | Unknown 2 | byte[9] | |
| 0x10 | RX Rated Audio Calibration | byte | |
| 0x11 | Unknown 3 | byte | |
| 0x12 | TX Secure Deviation | byte | 0–127 |
| 0x13 | RX Secure Discriminator | byte | 0–127 |
| 0x14 | Unknown Extra Byte | byte | VHF only (`HasExtraByte`) |

### Block 08 — Softpot Interpol Vector
List container. First byte is a count, followed by 2-byte vectors.

| Offset | Field |
|---|---|
| 0x00 | Count |
| 0x01… | List of → Block 09 vectors |

### Block 09 — Softpot Interpol
Length: `0x0D`. One entry per softpot calibration point.

| Offset | Field | Type |
|---|---|---|
| 0x00 | Unknown 1 | byte |
| 0x01 | Unknown 2 | byte |
| 0x02 | TX Deviation Balance Compensation | byte |
| 0x03 | TX Deviation Limit | byte |
| 0x04 | RX Front End Bandpass Filter | byte |
| 0x05 | Unknown 3 | byte |
| 0x06 | TX Power High | byte |
| 0x07 | Unknown 4 | byte |
| 0x08 | Unknown 5 | byte |
| 0x09 | TX Power Low | byte |
| 0x0A | Squelch Attenuator 12.5 kHz | byte | (unused on 800 MHz) |
| 0x0B | Squelch Attenuator 20 kHz | byte |
| 0x0C | Squelch Attenuator 25/30 kHz | byte |

### Block 0A — Softpot B/W Vector
Fixed list of four `Block 0B` vectors (no count byte).

### Block 0B — Softpot B/W
Length: `0x04`. Payload not yet decoded — `Unknown1: byte[4]`.

### Block 0C — Unknown
Length: `0x1E`. Payload not yet decoded — `Unknown1: byte[30]`.

### Block 0D — Hardware Config MDC
Length: `0x12`. Payload not yet decoded — `Unknown1: byte[18]`.

### Block 0E — Test Channel Table
Variable length. Currently exposes only the leading count and a raw byte
array; per-frequency decoding is TODO.

| Offset | Field |
|---|---|
| 0x00 | Frequency Count |
| 0x01… | Unknown 1 (raw test-frequency data) |

### Block 0F — AD Switch Level
Length: `0x0C`.

| Offset | Field | Type | Notes |
|---|---|---|---|
| 0x00–0x05 | Unknown 1 | byte[6] | |
| 0x06–0x07 | TX VCO Crossover | decimal | Unused on 800 MHz |
| 0x08 | Unknown 2 | byte | |
| 0x09–0x0A | RX VCO Crossover | decimal | Unused on 800 MHz |
| 0x0B | Unknown 3 | byte | |

### Block 10 — Feature Descriptor Block
The most heavily decoded block in the codeplug. This is the structure
Toolproof signs over and the place where most "feature unlocks" live.

Top-level layout:

| Offset | Field | Length | Notes |
|---|---|---|---|
| 0x00 | Feature Block Length | 1 | Always `0x19` in observed codeplugs |
| 0x01–0x19 | Feature Block | 25 | Bit-flag feature licensing — see below |
| 0x1A | Flashcode Length | 1 | Always `0x06` |
| 0x1B–0x20 | Flashcode | 6 | Hardware/firmware feature gate |

The 25-byte **Feature Block** is decoded bit-by-bit into ~200 individual
boolean and small-enum properties in [`Block10.cs`](../JediCodeplug/InternalBlocks/Block10.cs).
They group, very roughly, into:

| Region | Categories of features |
|---|---|
| Radio identity | Radio Type (Conv-only / Trunk+Conv), Radio Lock, Selectable Keypad Mute, Battery Saver, Out-of-Range Indicator, Talk-around, Tx Inhibit, etc. |
| Trunking | Privacy Plus, SmartNet, Failsoft per-mode, Auto Affiliation, AMSS, Trunked Status/Message, OBT, Dynamic Regrouping, Reprogram Request, Emergency, Phone Select, Private Call, Call Alert, Smartzone, CWE, Trunk I features |
| Scan | Mode Slaved, Operator Selectable, Trunking Priority Monitor, Talkgroup, Conventional Priority, Home Mode Revert, Off-hook Suspend, Smartzone Full-Spectrum |
| MDC | Status, Message, RTT, Radio Inhibit, Selective Call (and type/encode), Multi-system, Call Alert, Emergency, RAC |
| DTMF | Telephone Interconnect type, Selective Call, Call Alert encode, RAC, future-feature reserves |
| Conventional / signaling | Operator Selectable PL Encode, PAC RT, Single Tone, VRM, Clone & Destroy, Conventional Voting Scan |
| HHCH | Future-feature reserves (1–15) |
| Secure | Single-Key SW Encryption, Multikey SW Encryption, FIPS II, Infinite Key Retention, Conventional Multikey/OTAR, Trunked Multikey/OTAR, Remote Monitor Radio Trace |
| Mobile-only | Trunking Horn & Lights, MDC Horn & Lights, Motorcycle, Smart Status VIP, Metrocom (Trunk/Conv), Auxiliary Receiver, Multi-radio System, Speaker Absent, Internal PA, Vehicular Repeater, Siren PA, DEK, APCO Packet Data, Enhanced RCP, IMBE Digital (CAI), Omnilink, REDI Smart Messaging, APCO Trunking (ASTRO25 9.6 kbps), Scan with VRS, Zone Mode NJSP |
| Flash memory | 256 K / 384 K / 512 K / 1 M flash, DSP variants, Flash Portable Radio |

Many flags carry hints in their `[Description(...)]` attribute about whether
they're safe to enable, what CPS UI they unlock, and whether they have any
observable effect — read the source for the full detail.

### Block 11 — AD Button Level
Length: `0x08`. Payload not yet decoded — `Unknown1: byte[8]`.

---

## External blocks

### Block 30 — External Radio
Root of the external codeplug. Length: `0x4E`. Holds the radio's
re-programmable identity, programming history, and the vectors that anchor
every external block.

| Offset | Field | Type | Notes |
|---|---|---|---|
| 0x00–0x01 | Unknown 1 | byte[2] | Preserved by CPS but no observed effect |
| 0x02–0x0B | Serial | ASCII (10) | |
| 0x0C–0x1B | Model | ASCII (16) | |
| 0x1C–0x20 | Time Stamp | DateTime | Last programming time |
| 0x21 | Programming Source | enum | |
| 0x22–0x23 | External Codeplug Version | uint16 BE | CPS rejects values higher than its own (e.g. `0x001D` for R02.03.00) |
| 0x24–0x25 | External Codeplug Size | uint16 BE | |
| 0x26–0x27 | → Block 31 vector | ptr | Radio Wide |
| 0x28–0x29 | → Block 3D vector | ptr | Signaling |
| 0x2A–0x2B | → Block 36 vector | ptr | Display & Menu |
| 0x2C–0x2D | → Block 55 vector | ptr | Personality |
| 0x2E–0x2F | → Block 54 vector | ptr | Zone Chan Assignment |
| 0x30–0x31 | → Block 51 vector | ptr | Scan |
| 0x32–0x33 | Dynamic Radio vector | ptr | Points at a 0x3A-byte raw region (radio runtime scratch?) |
| 0x34–0x35 | → Block 39 vector | ptr | Phone List |
| 0x36–0x37 | → Block 3B vector | ptr | DTMF Codes |
| 0x38–0x39 | → Block 34 vector | ptr | Bus Option |
| 0x3A–0x3B | → Block 35 vector | ptr | (Unknown 35) |
| 0x3C–0x3D | → Block 3C vector | ptr | Hardware Button Config |
| 0x3E–0x3F | → Block 73 vector | ptr | Trunk Status / Zone Vector |
| 0x40–0x49 | Unknown 2 | byte[10] | CPS resets to `0x00` |
| 0x4A–0x4B | Dynamic Mode Select vector | ptr | Points at a 0x4E-byte raw region |
| 0x4C–0x4D | Unknown 3 | byte[2] | CPS resets to `0x00` |

### Block 31 — Radio Wide
Length: `0x36`. Payload not yet decoded — `Unknown1: byte[54]`.

### Block 34 — Bus Option Vector
Raw byte payload (`Contents`). Not yet decoded.

### Block 35 — Unknown
Raw byte payload. Not yet decoded.

### Block 36 — Display & Menu
Length: `0x27`.

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 37 vector — Zone Chan Text Vector |
| 0x02–0x26 | Unknown 1 (37 bytes) |

### Block 37 — Zone Chan Text Vector
List container.

| Offset | Field |
|---|---|
| 0x00 | Count |
| 0x01… | List of → Block 38 vectors |

### Block 38 — Zone Chan Text
`BlockLong` (2-byte length field). Variable-length list of zone/channel
display names. Raw payload exposed as `Contents`.

### Block 39 — Phone List Vector

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 3A vector — Phone List |

### Block 3A — Phone List
`BlockLong`. Raw byte payload.

### Block 3B — DTMF Codes List
Raw byte payload.

### Block 3C — Hardware Button Configuration
Raw byte payload.

### Block 3D — Signaling Vector
Length: `0x0E`.

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 3E vector — Conv Configuration |
| 0x02–0x03 | → Block 44 vector — MDC Configuration |
| 0x04–0x05 | → Block 4A vector — Trunk Configuration |
| 0x06–0x07 | UnknownPointer 2 |
| 0x08–0x09 | UnknownPointer 3 |
| 0x0A–0x0B | UnknownPointer 4 |
| 0x0C–0x0D | → Block A0 vector — Aux Signalling |

### Block 3E — Conv Configuration

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 3F vector — Control Head |
| 0x02–0x03 | → Block 41 vector — Menu Item |
| 0x04–0x06 | UnknownBytes 1 |
| 0x07–0x08 | → Block 42 vector |
| 0x09–0x0B | UnknownBytes 2 |
| 0x0C–0x0D | → Block 8E vector — Status List Vector |
| 0x0E–0x0F | → Block 90 vector — Message List Vector |

### Block 3F — Control Head
Length: `0x13`. Payload not yet decoded — `Unknown1: byte[19]`.

### Block 41 — Menu Item
Raw byte payload.

### Block 42 — Unknown

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 43 vector |

### Block 43 — Unknown
`BlockLong`. Raw byte payload.

### Block 44 — MDC Configuration

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 47 vector — MDC System Vector |
| 0x02–0x03 | → Block 9F vector — MDC Repeater ID Table |
| 0x04–0x05 | → Block 92 vector — MDC Call List Vector |

### Block 47 — MDC System Vector
List container.

| Offset | Field |
|---|---|
| 0x00 | Count |
| 0x01… | List of → Block 48 vectors |

### Block 48 — MDC System
Raw byte payload.

### Block 4A — Trunk Configuration

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 3F vector — Control Head |
| 0x02–0x03 | → Block 41 vector — Menu Item |
| 0x04–0x05 | → Block 57 vector — Zone Chan TG Table |
| 0x0A–0x0B | → Block 4B vector — Trunk System Vector |
| 0x0D–0x0E | → Block 74 vector — Trunk Message List |

### Block 4B — Trunk System Vector
List container.

| Offset | Field |
|---|---|
| 0x00 | Count |
| 0x01… | List of → Block 5A vectors |

### Block 51 — Scan Configuration

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 52 vector — Scan List Vector |

### Block 52 — Scan List Vector
List container.

| Offset | Field |
|---|---|
| 0x00 | Count |
| 0x01… | List of → Block 53 vectors |

### Block 53 — Scan List
Raw byte payload.

### Block 54 — Zone Chan Assignment
`BlockLong`. Raw byte payload.

### Block 55 — Personality Vector
List container. Each entry may be a `Block 56` (conventional) or `Block 62`
(trunked / test) — the parser branches on context.

| Offset | Field |
|---|---|
| 0x00 | Count |
| 0x01… | List of → Block 56-or-62 vectors |

### Block 56 — Conventional Personality (or `62`: Trunk/Test Mode Personality)
One entry per channel. Variable width (rows are tagged with `+1 byte`).

| Offset | Field |
|---|---|
| 0x00–0x18 | Unknown 1 (channel parameters — frequency, PL, options) |
| 0x19–0x1A | → Block 0E vector — Test Channel Table |
| 0x1B–0x22 | Unknown 2 |

### Block 57 — Zone Chan TG Table
`BlockLong`. Raw byte payload.

### Block 58 — Trunk Call List Vector (or `CD`: ASTRO25 Trunk Call List Vector)

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 59 vector — Trunk Call List |

### Block 59 — Trunk Call List
`BlockLong`. Raw byte payload.

### Block 5A — Trunk System

| Offset | Field |
|---|---|
| 0x02–0x03 | → Block 58 vector — Trunk Call List Vector |
| 0x04–0x05 | → Block 61 vector |
| 0x1A–0x1B | Dynamic Trunk vector | ptr to a 0x28-byte raw region (radio runtime scratch?) |

### Block 61 — Unknown
Raw byte payload.

### Block 62 — Unknown
The trunked counterpart to Block 56.

| Offset | Field |
|---|---|
| 0x03–0x04 | → Block 67 vector — Trunk Pers Emergency |
| 0x0C–0x0D | → Block 63 vector — Trunk II TG |
| (var) | → Block DE vector — Trunk II FS Channel Rebanded |

### Block 63 — Trunk II TG
Variable size. If first byte is `0x9F`, bytes 0x03–0x04 hold a vector to
Block 65 (`// total stab in the dark for now`). Otherwise raw payload.

### Block 65 — SP Tone Table / Trunk II FS Channel
Raw byte payload. Two distinct uses depending on parent context.

### Block 67 — Trunk Pers Emergency
Raw byte payload.

### Block 73 — Trunk Status List / Zone Vector
`BlockLong`. Raw byte payload. Two distinct uses depending on parent context.

### Block 74 — Trunk Message List / Zone Chan TG Table Accel Vector
`BlockLong`. Raw byte payload. Two distinct uses depending on parent context.

### Block 8E — Status List Vector

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 8F vector — Status List |

### Block 8F — Status List
`BlockLong`. Raw byte payload (typically ASCII status strings padded with
spaces).

### Block 90 — Message List Vector

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 91 vector — Message List |

### Block 91 — Message List
`BlockLong`. Raw byte payload (typically ASCII message strings padded with
spaces).

### Block 92 — MDC Call List Vector

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block 93 vector — MDC Call List |

### Block 93 — MDC Call List
`BlockLong`. Raw byte payload.

### Block 9F — MDC Repeater ID Table
Raw byte payload.

### Block A0 — Aux Signalling

| Offset | Field |
|---|---|
| 0x00–0x01 | → Block A4 vector |
| 0x02–0x03 | → Block A1 vector — Singletone System Vector |
| 0x04–0x05 | → Block A3 vector — Singletone List |
| 0x08–0x09 | → Block A9 vector |

### Block A1 — Singletone System Vector
List container.

| Offset | Field |
|---|---|
| 0x00 | Count |
| 0x01… | List of → Block A2 vectors |

### Block A2 — Singletone System
Raw byte payload.

### Block A3 — Singletone List
Raw byte payload.

### Block A4 — Unknown
List container.

| Offset | Field |
|---|---|
| 0x00 | Count |
| 0x01… | List of → Block A5 vectors |

### Block A5 — Unknown
Raw byte payload.

### Block A9 — Unknown
Raw byte payload.

### Block DE — Trunk II FS Channel Rebanded
Raw byte payload.

---

## Conventions used in the source

Every parser follows a small set of patterns worth knowing if you're going
to extend one:

- **`private const int FOO = 0x..;`** at the top names the byte offsets the
  parser reads from. The trailing `// 01 02 03` comment (when present)
  enumerates the additional bytes the field spans.
- **`Block` vs `BlockLong`** — the base class uses a 1-byte length field;
  `BlockLong` uses 2 bytes, so its records can exceed 255 bytes.
- **`Contents`** — when a block is partially or fully undecoded, its raw
  payload is exposed as a `byte[]`/`Span<byte>` on a `Contents` property.
  This lets the round-trip serializer reproduce the bytes verbatim even
  before they're properly parsed.
- **List containers** ("Vector" blocks) almost always begin with a 1-byte
  count, followed by 2-byte big-endian vectors to the listed children. The
  named exception is `Block 0A`, which has a fixed list of four entries
  with no count byte.
- **"Unknown" blocks** are placeholders for blocks whose ID is observed in
  real codeplugs but whose internal structure hasn't been worked out yet —
  good starting points for further reverse engineering.
