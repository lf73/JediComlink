# Auth code ("Toolproofing")

The MTS2000 enforces a power-on check that ties the codeplug to the radio it
was generated for. If the check fails the radio displays **`Fail 01/93`** and
halts the rest of the Power-On Self-Test.

The check was added by Motorola Service Repair Note **SRN 1173** ("Toolproofing")
on **27 February 1995** to prevent unauthorized field upgrades of feature
options. JediComlink reproduces the algorithm in
[`JediCodeplug/AuthCode.cs`](../JediCodeplug/AuthCode.cs); the original
firmware code was reverse-engineered from a dumped 64 KB MC6809 address
window of the boot-time ROM.

## The two checks

There are actually two independent comparisons that can produce `Fail 01/93`:

1. **Factory-code compare.** EEPROM `0x81F8-0x81FF` is compared byte-for-byte
   to Flash `0x3FFF8-0x3FFFF`. Both regions hold the same eight tail bytes of
   the 16-byte factory code.
2. **Auth-code hash.** A 10-byte hash is computed over a fixed-layout 0x50
   buffer and XOR-masked with the serial number, then compared against the
   stored auth code in the Internal Radio Block (Block 01) at EEPROM
   `0x0032-0x003B`.

Only the second check is implemented in `AuthCode.cs` — the first is a trivial
memcmp the codeplug parser does not need to recompute.

## Hash input buffer (0x50 bytes)

The hash is computed over a buffer assembled like this:

| Buffer offset | Length | Source | EEPROM / Flash address |
|---|---|---|---|
| `0x00-0x0F` | 16 | Model number (12 ASCII + 4 zeros) | EEPROM `0x000E-0x001D` |
| `0x10-0x1F` | 16 | Factory code | Flash `0x3FFF0-0x3FFFF` |
| `0x20-0x27` | 8  | Firmware signature bytes | Flash `0x03000`, `0x0B000`, `0x13000`, `0x1B000`, `0x23000`, `0x2B000`, `0x33000`, `0x3B000` |
| `0x28-0x2A` | 3  | Zero padding | — |
| `0x2B-0x43` | 25 | FDB Part A — Block 10 feature block | EEPROM length-prefixed at `0x01DE`, body at `0x01DF` |
| `0x44-0x49` | 6  | FDB Part B — Block 10 flashcode | follows Part A |
| `0x4A-0x4F` | 6  | Zero padding | — |

The lengths of FDB Parts A and B are typically 0x19 and 0x06 but are read
from length-prefix bytes in the codeplug. The serial number is **not** in
this buffer — it is mixed in at the very end as an output mask
(see [Final XOR](#final-xor)).

The eight firmware-signature bytes are why
`Codeplug.RecalculateAuthCode()` requires a known firmware version: they
must be looked up from `JediDefinitions.json` (or read directly out of
flash) per firmware revision.

## The mixing function

The algorithm walks an 80-byte permutation `ORDER` once (`AuthCode.cs:10-16`),
producing one output byte every eight iterations — ten output bytes total.
It maintains a single byte of running state, `z`, that is reset between
output bytes:

```csharp
foreach (byte o in ORDER)
{
    byte a = buffer[o];

    a ^= z;  z = a;                      // chain
    a = (byte)((sbyte)a >> 1);            // ASR
    a = (byte)(a >> 1);                   // LSR
    a ^= z;                               // mix back

    byte b = a;
    a = (byte)((a << 1) & 0xF0);          // high nibble
    b = (byte)((sbyte)b >> 1);
    if ((b & 0x80) == 0x80) b = (byte)~b; // sign-conditional complement
    b &= 0x0F;                            // low nibble
    a = (byte)(a + b);                    // recombine

    a ^= KEY[z & 7];                      // table lookup, 3 bits of state
    z = a;

    if (++i % 8 == 0) { authCode[j++] = z; z = 0; }
}
```

The inline comments in `AuthCode.cs` (`asra`, `lsra`, `tab`, `aba`, `bmi`,
`comb`, `ldx #$D8B9`, etc.) preserve the original MC6809 mnemonics. The
`#$D8B9` is the ROM address of the `KEY` table in the dumped firmware
window.

## Final XOR

The 10-byte hash is XORed with the 10 ASCII serial-number bytes
(`AuthCode.cs:108-112`). This is what makes the same model + firmware +
factory + flashcode produce a different stored auth code on every radio.

## The `ORDER` permutation

`ORDER` is a permutation of `0x00-0x4F` (a true permutation; every value
in that range appears exactly once). Indices `0x28`, `0x29`, `0x2A`,
`0x4A-0x4F` correspond to the zero-padded buffer slots and contribute
nothing to the output.

No structural pattern (bit-reverse, affine map, derived table) is apparent.
It looks hand-shuffled.

## The `KEY` constants — internal structure

```
KEY = { 0x00, 0x99, 0xAC, 0x35, 0xC7, 0x5E, 0x6B, 0xF2 }
```

These are not random. **Adjacent pairs all XOR to `0x99`:**

| Pair | XOR |
|---|---|
| `0x00 ^ 0x99` | `0x99` |
| `0xAC ^ 0x35` | `0x99` |
| `0xC7 ^ 0x5E` | `0x99` |
| `0x6B ^ 0xF2` | `0x99` |

So the table is really four base values `{0x00, 0xAC, 0xC7, 0x6B}` with each
duplicated and the duplicate XORed by `0x99`. With the lookup index being
`z & 7`, the LSB of `z` chooses raw-vs-flipped-by-`0x99` and the next two
bits choose which of the four bases. This is a deliberate balancing
choice, not derived from any published S-box that we have been able to
identify.

## Is this a known algorithm?

No. The closest published reference in *concept* is **Pearson hashing**
(running state `h` updated by `h = T[h ^ b]` over input bytes) — both share a
single byte of state and a state-keyed table lookup. But the resemblance
is loose:

- Input bytes are consumed in **permuted** order (`ORDER`), not source order.
- The lookup table has **8 entries indexed by 3 bits of state**, not 256
  entries indexed by the full state byte. It functions as a whitening
  step on top of the main mixing, not as the sole nonlinearity.
- The per-byte mixing — ASR/LSR, double XOR with `z`, nibble split with
  sign-conditional `~b`, add halves — is much heavier than Pearson's
  single XOR-and-lookup.
- Output is **fixed 10 bytes** by truncating `z` every 8 iterations and
  resetting `z` between blocks.

The outer chain (each input XORs into running state, state feeds the next
step) is **CBC-MAC-shaped**, but there is no underlying block cipher — the
mixing core is hand-built and does not match any published 80s/90s hash or
MAC primitive. The instruction sequence reads as if it was designed at the
MC6809 instruction level for cheapness on that CPU, rather than transcribed
from a higher-level spec.

In short: **bespoke Motorola work**, with Pearson as a distant conceptual
cousin and CBC-MAC as a structural shape. There is no NIST or other
standard to fall back on; anyone reimplementing it needs the exact `ORDER`
and `KEY` constants verbatim.

## Practical notes

- `Codeplug.RecalculateAuthCode()` must be called before writing edits
  back to the radio, or the radio will display `Fail 01/93` on next power
  on.
- The factory code's bytes 7-8 are a checksum the calculator recomputes
  before the hash runs (`AuthCode.cs:42-52`). The codeplug stores zeros at
  those positions; the radio's flash holds the real values.
- The serial number must be 10 ASCII characters — shorter serials would
  require zero-padding (this has not been observed in known-good
  codeplugs).
- The mixing function has no known inverse, so editing the auth code to
  hide arbitrary changes requires recomputing the full hash — there is no
  shortcut to patch a single byte.

## References

- `JediCodeplug/AuthCode.cs` — implementation
- `JediCommon/JediDefinitions.json` — per-firmware signature bytes
- Internal Radio Block (Block 01) stores the auth code at payload
  offset `0x32` — see [`BlockTypes.md`](BlockTypes.md)
- Motorola Service Repair Note SRN 1173, "Toolproofing" (1995)
