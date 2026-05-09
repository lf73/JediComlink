# JediComlink boot code

The two byte arrays in [`JediFlash/BootCode.cs`](../JediFlash/BootCode.cs) are
base64-encoded and XOR-encoded with `0x55`. After decryption they are the live
regions of the patched MTSXBoot ROM, which Jedi-Comlink sends to the radio over
the programming serial link to take over the radio's bootstrap mode.

| Symbol | Size | CPU load address | Upload protocol |
|---|---|---|---|
| `BOOT_STRAP` | 1024 B | `$0000-$03FF` | raw, 3600 baud, on RX break |
| `BOOT_LOADER` | 4608 B | `$2000-$31FF` | SBEP `0x80` writes, 115200 baud |

The XOR happens in [`JediFlash/FlashCom.cs`](../JediFlash/FlashCom.cs):

```csharp
private static byte[] _bootStrap  = BootCode.BootStrap.Select(x => x ^= 0x55).ToArray();
private static byte[] _bootLoader = BootCode.BootLoader.Select(x => x ^= 0x55).ToArray();
```

Decoded, the bootstrap bytes match `MTSXBoot - Patched.bin[0x000:0x400]`
exactly, and the boot loader matches `MTSXBoot - Patched.bin[0x2000:0x3200]`.

The CPU is a Motorola 68HC11F1. The on-chip register block sits at
`$1000-$105F`; the boot ROM accesses it almost exclusively as `$NN,x` with
`X = $1000`. Inline `;` comments in the listings below name those registers
and merge the hand annotations from `MTS2000/Bootstrap/Boot Load Disassm.txt`
(prefixed `orig:`) where they existed.

## Bootstrap memory map (`$0000-$03EE`)

| Range | Purpose |
|---|---|
| `$0000-$0003` | Padding (`sbca $F0,x`, `nop`, `nop`) |
| `$0004-$002B` | CPU bring-up: SCSR.TC wait, CSCTL=`$8C`, CSGSIZ=`$8A`, HPRIO\|=`$20`, SP=`$3FFF`, init at `$00A5`, jump to `$016E` |
| `$002C-$004F` | SPI helper + 21-byte init table at `$003B` |
| `$0050-$005B` | `write_spi_block(X, B)` |
| `$005C-$0077` | Synth-select SPI write + 13-byte payload at `$006B` |
| `$0078-$00A4` | SBEP transmit helper for command `0x17` |
| `$00A5-$00C1` | One-shot register-drain init |
| `$00C4-$00FF` | Pseudo-vector table (mirrors HC11 vectors) |
| `$0100-$0141` | PORTB MSB toggles, software delays, ASCFIC_SEL / SYN_SEL bit gates |
| `$0143-$014F` | Fatal-error trap (busy loop) |
| `$0151-$016D` | TOC1 ISR |
| `$016E-$01BD` | Main port/SPI bring-up: PORTG=`$FF`, DDRA=`$F8`, DDRD=`$38`, SPCR=`$5E`, BAUD=`$01`, SCCR2=`$2C`; disable COP via CONFIG; falls into `$02B2` |
| `$01BE-$01F2` | SCI receive ISR |
| `$01F3-$0240` | First-byte handler — recognizes `$F0..$FF`, `$50`, `$60` framing |
| `$0241-$028B` | RX state machine (length / body / checksum) |
| `$028C-$028F` | Ignore-until-timeout sink |
| `$0290-$02B1` | Frame-complete: stash `$0002`, ack with `$50`, reset RX vector |
| `$02B2-$032D` | Main service loop — dispatches SBEP commands `0x17`, `0x15`, `0x11`, `0x19`, `0x13` |
| `$032E-$0364` | SBEP-ack TX |
| `$0365-$0372` | SPI-write primitive |
| `$0373-$037B` | PORTB MSB set helper |
| `$037C-$038D` | "Command not matched" reply |
| `$038E-$03EE` | Command `0x17` (write memory) dispatcher — top-nibble routes into the boot-loader region: `$80` = local RAM, others jump to `$2EF4`/`$2EC5`/`$2EC6`/`$2F53` |

## Boot-loader memory map (`$2000-$31FF`)

The boot loader is uploaded *by* the bootstrap. It implements:

- `$2000-$200B` — SLIC enable (set bit 6 of `$106A` and `$106D`)
- `$200D-$2037` — Flash-write primitive: clears `$1035`, walks `$103B` (PPROG)
  through erase/program states with a delay loop at `$0118`
- The remainder is the SBEP-extended command handlers the bootstrap dispatches
  to: erase/write flash banks, EEPROM I/O, and the read-flash extension that
  the original Motorola loader did not have (the reason this custom boot exists)

## Bootstrap annotated disassembly

```asm
0000: A2 F0           sbca   $F0,x      ; orig: Garbarge?
0002: 01              nop
0003: 01              nop
0004: B6 10 2E        ldaa   $102E      ; SCSR (SCI status) | orig: ldaa	0x102e  Check SCSR  for Transmit Complete Flag
0007: 85 40           bita   #$40
0009: 27 F9           beq    $0004      ; orig: Loop until 0
000B: CE 10 00        ldx    #$1000     ; orig: ldx	#0x1000
000E: 86 8C           ldaa   #$8C       ; orig: <<<<<< CSCTL set to 8c
0010: A7 5D           staa   $5D,x      ; orig: <<<<<<
0012: 86 8A           ldaa   #$8A       ; orig: <<<<<< CSGSIZ set to 8a
0014: A7 5F           staa   $5F,x      ; orig: <<<<<<
0016: E6 3C           ldab   $3C,x      ; orig: <<<<<< HPRIO or'd with 20
0018: CA 20           orab   #$20       ; orig: <<<<<<
001A: E7 3C           stab   $3C,x      ; orig: <<<<<<
001C: 8E 3F FF        lds    #$3FFF     ; orig: lds	#0x3fff   Load Stack Pointer to End of RAM
001F: CE 10 00        ldx    #$1000     ; orig: ldx	#0x1000
0022: 86 80           ldaa   #$80       ; orig: <<<<<<
0024: A7 65           staa   $65,x      ; orig: ???  Unknown Register to 80
0026: BD 00 A5        jsr    $00A5      ; orig: jsr	0x00a5    Some kind of setup, only called once
0029: 7E 01 6E        jmp    $016E      ; orig: jmp	0x016e
002C: BD 01 26        jsr    $0126      ; orig: jsr	0x0126   Clear  ASCFIC_SEL
002F: CE 00 3B        ldx    #$003B     ; orig: ldx	#0x003b
0032: C6 15           ldab   #$15
0034: BD 00 50        jsr    $0050      ; orig: jsr	0x0050
0037: BD 01 2D        jsr    $012D      ; orig: jsr	0x012d
003A: 39              rts
003B: 22 21           bhi    $005E
003D: 00              test
003E: C5 00           bitb   #$00
0040: 00              test
0041: C9 80           adcb   #$80
0043: C4 AF           andb   #$AF
0045: FE 00 00        ldx    $0000
0048: 00              test
0049: 00              test
004A: 00              test
004B: 00              test
004C: 00              test
004D: 21 00           brn    $004F
004F: 11              cba

; ---  | Takes X and B.  Loops until B is 0.. Writes SPI ---
0050: 36              psha
0051: A6 00           ldaa   $00,x
0053: BD 03 65        jsr    $0365      ; orig: jsr	0x0365
0056: 08              inx
0057: 5A              decb
0058: 26 F7           bne    $0051
005A: 32              pula
005B: 39              rts
005C: BD 01 34        jsr    $0134      ; orig: CLEAR  SYN_SEL
005F: CE 00 6B        ldx    #$006B     ; orig: ldx	#0x006b
0062: C6 0D           ldab   #$0D
0064: BD 00 50        jsr    $0050      ; orig: Writes SPI  (6B through 77)
0067: BD 01 3B        jsr    $013B      ; orig: SET SYN_SEL
006A: 39              rts
006B: FF FF FF        stx    $FFFF
006E: FE 05 40        ldx    $0540
0071: 06              tap
0072: 05              asld
0073: 0A              clv
0074: FC 6E 16        ldd    $6E16
0077: 45              .byte $45

; --- Called by CMD 17 ---
0078: 36              psha
0079: 37              pshb
007A: 18 3C           pshy
007C: 18 CE 0C 00     ldy    #$0C00
0080: 86 F4           ldaa   #$F4
0082: 18 A7 03        staa   $03,y      ; orig: Store F4 to 0c03
0085: 96 0C           ldaa   *$0C
0087: 85 80           bita   #$80
0089: 26 04           bne    $008F
008B: 86 84           ldaa   #$84
008D: 20 02           bra    $0091
008F: 86 85           ldaa   #$85
0091: 18 A7 04        staa   $04,y      ; orig: staa	0x4,y
0094: 18 E7 05        stab   $05,y      ; orig: stab	0x5,y
0097: CD EF 06        stx    $06,y      ; orig: stx	0x6,y
009A: 4F              clra
009B: C6 05           ldab   #$05
009D: BD 03 2E        jsr    $032E      ; orig: jsr	0x032e
00A0: 18 38           puly
00A2: 33              pulb
00A3: 32              pula
00A4: 39              rts

; ---  | Boot, first call for some kind of setup ---
00A5: 36              psha
00A6: 37              pshb
00A7: 3C              pshx
00A8: 07              tpa
00A9: 0F              sei
00AA: CE 10 00        ldx    #$1000     ; orig: ldx	#0x1000
00AD: E6 65           ldab   $65,x      ; orig: Simple act of reading?
00AF: E6 6A           ldab   $6A,x      ; orig: Or garbage statements.
00B1: E6 61           ldab   $61,x
00B3: E6 74           ldab   $74,x
00B5: E6 60           ldab   $60,x
00B7: E6 75           ldab   $75,x
00B9: E6 6A           ldab   $6A,x
00BB: E6 60           ldab   $60,x
00BD: 06              tap
00BE: 38              pulx
00BF: 33              pulb
00C0: 32              pula
00C1: 39              rts
00C2: FF FF 7E        stx    $FF7E
00C5: 01              nop
00C6: BE 7E 01        lds    $7E01
00C9: 43              coma
00CA: 7E 01 43        jmp    $0143      ; orig: Pulse Accumulator Input Edge
00CD: 7E 01 43        jmp    $0143      ; orig: Pulse Accumulator Overflow
00D0: 7E 01 43        jmp    $0143      ; orig: Timer Overflow
00D3: 7E 01 43        jmp    $0143      ; orig: Timer Input Capture 4/Output Compare 5
00D6: FF AF 00        stx    $AF00      ; orig: Timer Output Compare 4
00D9: 00              test              ; orig: Timer Output Compare 3
00DA: 01              nop
00DB: 35              txs
00DC: 2E 30           bgt    $010E      ; orig: Timer Output Compare 2
00DE: 01              nop
00DF: 7E 01 51        jmp    $0151      ; orig: Timer Output Compare 1
00E2: 7E 01 43        jmp    $0143      ; orig: Timer Input Capture 3
00E5: 7E 01 43        jmp    $0143      ; orig: Timer Input Capture 2
00E8: 7E 01 43        jmp    $0143      ; orig: Timer Input Capture 1
00EB: 7E 2B EC        jmp    $2BEC      ; orig: Real-Time Interrupt
00EE: 7E 01 43        jmp    $0143      ; orig: IRQ
00F1: 7E 01 43        jmp    $0143      ; orig: XIRG Pin
00F4: 7E 01 43        jmp    $0143      ; orig: Software Interrupt
00F7: 7E 01 43        jmp    $0143      ; orig: Illegal Opcode Trap
00FA: 7E 01 43        jmp    $0143      ; orig: COP Failure
00FD: 7E 01 43        jmp    $0143      ; orig: Clock Monitor Fail

; --- Not referenced ---
0100: 3C              pshx
0101: CE 10 00        ldx    #$1000     ; orig: ldx	#0x1000
0104: 1D 61 80        bclr   $61,x #$80 ; orig: bclr	0x61,x, #0x80 Clear 1061 MSB
0107: 38              pulx
0108: 39              rts
0109: 39              rts

; ---  | Referenced 2 times  (1 by unused function) ---
010A: 36              psha              ; orig: Looks like a delay function
010B: 37              pshb
010C: 3D              mul
010D: 01              nop
010E: 33              pulb
010F: 32              pula
0110: 39              rts

; --- Not Referenced ---
0111: BD 01 0A        jsr    $010A      ; orig: jsr	0x010a
0114: 5A              decb
0115: 26 FA           bne    $0111
0117: 39              rts

; ---  | Referenced 1 Time ---
0118: 36              psha
0119: 86 62           ldaa   #$62
011B: BD 01 0A        jsr    $010A      ; orig: jsr	0x010a
011E: 4A              deca
011F: 26 FA           bne    $011B
0121: 5A              decb
0122: 26 F5           bne    $0119
0124: 32              pula
0125: 39              rts

; --- Clear  ASCFIC_SEL ---
0126: CE 10 02        ldx    #$1002     ; PORTG (F1) | orig: ldx	#0x1002
0129: 1D 00 08        bclr   $00,x #$08 ; orig: bclr	0x0,x, #0x08  Clear  ASCFIC_SEL
012C: 39              rts
012D: CE 10 02        ldx    #$1002     ; PORTG (F1) | orig: ldx	#0x1002
0130: 1C 00 08        bset   $00,x #$08 ; orig: bset	0x0,x, #0x08   SET   ASCFIC_SEL
0133: 39              rts
0134: CE 10 02        ldx    #$1002     ; PORTG (F1) | orig: ldx	#0x1002
0137: 1D 00 01        bclr   $00,x #$01 ; orig: bclr	0x0,x, #0x01  CLEAR  SYN_SEL
013A: 39              rts
013B: CE 10 02        ldx    #$1002     ; PORTG (F1) | orig: ldx	#0x1002
013E: 1C 00 01        bset   $00,x #$01 ; orig: bset	0x0,x, #0x01   SET SYN_SEL
0141: 39              rts
0142: 00              test
0143: CE 00 3B        ldx    #$003B     ; orig: ldx	#0x003b
0146: A6 08           ldaa   $08,x
0148: 84 7F           anda   #$7F
014A: A7 08           staa   $08,x
014C: BD 00 2C        jsr    $002C      ; orig: Writes something to SPI? Maybe screen or LED?
014F: 20 FE           bra    $014F      ; orig: Loop forever

; --- Interrupt Service Routine for Timer Output Compare 1 ---
0151: 86 80           ldaa   #$80
0153: B7 10 23        staa   $1023      ; TFLG1 (timer flag 1) | orig: staa	0x1023  TFLG1 — Timer Interrupt Flag 1    -- Sets Output Compare 1 Flag  (OC1F)
0156: CE 10 22        ldx    #$1022     ; TMSK1 (timer mask 1) | orig: ldx	#0x1022
0159: 1D 00 80        bclr   $00,x #$80 ; orig: bclr	0x0,x, #0x80
015C: 7F 00 02        clr    $0002      ; orig: clr	0x0002
015F: CE 01 F3        ldx    #$01F3     ; orig: Rester SCR Handler to Byte 0
0162: FF 00 03        stx    $0003      ; orig: stx	0x0003
0165: B6 10 2E        ldaa   $102E      ; SCSR (SCI status) | orig: ldaa	0x102e
0168: 86 60           ldaa   #$60
016A: B7 10 2F        staa   $102F      ; SCDR (SCI data) | orig: Write 60 to Serial Port
016D: 3B              rti
016E: C6 C0           ldab   #$C0
0170: E7 61           stab   $61,x      ; orig: X still set to 1000... 1061 set to c0
0172: CE 10 00        ldx    #$1000     ; orig: ldx	#0x1000
0175: 86 FF           ldaa   #$FF
0177: A7 02           staa   $02,x      ; orig: PORTG Set all pins to On
0179: A7 03           staa   $03,x      ; orig: DDRG  Set All pins to output
017B: 86 F8           ldaa   #$F8       ; orig: PA7, PA6, PA5, PA4, PA3 output
017D: A7 01           staa   $01,x      ; orig: DDRA
017F: 1C 08 20        bset   $08,x #$20 ; orig: bset	0x8,x, #0x20   PORTD Set PD5
0182: 86 38           ldaa   #$38       ; orig: ‭00111000‬  PD5 PD4 PD3 OUT
0184: A7 09           staa   $09,x      ; orig: DDRD

; --- Setup SPI ---
0186: 86 5E           ldaa   #$5E       ; orig: ‭0101 1110‬ SPE MSTR CPOL CPHA SPR1
0188: A7 28           staa   $28,x      ; orig: SPCR
018A: C6 2C           ldab   #$2C
018C: B6 00 DE        ldaa   $00DE      ; orig: ldaa	0x00de     DE contains 01
018F: A7 2B           staa   $2B,x      ; orig: BAUD  01  00000001 ‬  SCR0
0191: E7 2D           stab   $2D,x      ; orig: SCCR2 2C  ‭00101100‬   RIE  TE  RE
0193: CE 10 00        ldx    #$1000     ; orig: ldx	#0x1000
0196: C6 1F           ldab   #$1F
0198: E7 63           stab   $63,x      ; orig: 1F
019A: 86 A0           ldaa   #$A0
019C: A7 69           staa   $69,x      ; orig: A0
019E: 86 F0           ldaa   #$F0
01A0: A7 6C           staa   $6C,x      ; orig: F0
01A2: 86 54           ldaa   #$54
01A4: A7 6A           staa   $6A,x      ; orig: 54
01A6: 86 F4           ldaa   #$F4
01A8: A7 6D           staa   $6D,x      ; orig: F4
01AA: BD 00 2C        jsr    $002C      ; orig: jsr	0x002c        Somthing to do with SPI
01AD: BD 00 5C        jsr    $005C      ; orig: jsr	0x005c        Somthing to do with SPI
01B0: B6 00 4F        ldaa   $004F      ; orig: ldaa	0x004f    Get CONFIG Register
01B3: 84 EE           anda   #$EE       ; orig: ‭1110 1110‬    (According to docs.. these bits do nothing in bootstrap mode.. Double check??)
01B5: B7 00 4F        staa   $004F      ; orig: staa	0x004f     EE3 EE2 EE1  NOCOP   (COP System Disable; (does not force reset on time-out), Moves on-chip EEPROM to EE00 - EFFF
01B8: BD 00 2C        jsr    $002C      ; orig: jsr	0x002c       Somthing to do with SPI
01BB: 7E 02 B2        jmp    $02B2      ; orig: jmp	0x02b2

; --- Interrupt Service Routine for SCI Serial System ---
01BE: FC 10 0E        ldd    $100E      ; TCNT (free-running counter) | orig: ldd	0x100e
01C1: C3 FF FF        addd   #$FFFF     ; orig: addd	#0xffff
01C4: FD 10 16        std    $1016      ; TOC1 (output compare 1) | orig: std	0x1016  Write to TOC1 High / Low  (Addding FFFF catches next full cycle)
01C7: F6 10 2E        ldab   $102E      ; SCSR (SCI status) | orig: ldab	0x102e
01CA: B6 10 2F        ldaa   $102F      ; SCDR (SCI data) | orig: Read Serial Port
01CD: 16              tab
01CE: DB 02           addb   *$02       ; orig: Add what was received with what is stored at 0x02
01D0: D7 02           stab   *$02       ; orig: Could be a checksum
01D2: DE 00           ldx    *$00       ; orig: Set Index X to values stored in 0x00 and 0x01
01D4: E6 00           ldab   $00,x
01D6: 27 12           beq    $01EA      ; orig: If 0x00 is not zero then junk?
01D8: CE 02 8C        ldx    #$028C     ; orig: Maybe sets to ignore bytes until next time out
01DB: DF 03           stx    *$03
01DD: C6 80           ldab   #$80
01DF: F7 10 23        stab   $1023      ; TFLG1 (timer flag 1) | orig: stab	0x1023   TFLG1  Sets OC1F
01E2: CE 10 22        ldx    #$1022     ; TMSK1 (timer mask 1) | orig: ldx	#0x1022
01E5: 1C 00 80        bset   $00,x #$80 ; orig: bset	0x0,x, #0x80    TMSK1   sets OC1I
01E8: 20 08           bra    $01F2
01EA: 18 DE 03        ldy    *$03       ; orig: ldy	*0x0003
01ED: 18 AD 00        jsr    $00,y      ; orig: jsr	0x0,y
01F0: DF 03           stx    *$03
01F2: 3B              rti

; ---  | Byte 0 Handler? | Some kind of Interrupt Handler.. Address is put in 0003/0004 | A Register is set with received byte ---
01F3: 7F 00 07        clr    $0007      ; orig: clr	0x0007
01F6: 7F 00 08        clr    $0008      ; orig: clr	0x0008
01F9: 7F 00 0B        clr    $000B      ; orig: clr	0x000b
01FC: C6 80           ldab   #$80
01FE: F7 10 23        stab   $1023      ; TFLG1 (timer flag 1) | orig: stab	0x1023     TFLG1  Sets OC1F
0201: 18 CE 10 22     ldy    #$1022     ; TMSK1 (timer mask 1)
0205: 18 1C 00 80     bset   $00,y #$80 ; orig: TMSK1   sets OC1I
0209: 16              tab
020A: C1 F0           cmpb   #$F0
020C: 24 1B           bcc    $0229
020E: C1 50           cmpb   #$50
0210: 27 04           beq    $0216
0212: C1 60           cmpb   #$60
0214: 26 0E           bne    $0224
0216: 7F 00 02        clr    $0002      ; orig: clr	0x0002
0219: CE 10 22        ldx    #$1022     ; TMSK1 (timer mask 1) | orig: ldx	#0x1022
021C: 1D 00 80        bclr   $00,x #$80 ; orig: bclr	0x0,x, #0x80    TMSK1   clears OC1I
021F: CE 01 F3        ldx    #$01F3     ; orig: ldx	#0x01f3
0222: 20 03           bra    $0227
0224: CE 02 8C        ldx    #$028C     ; orig: Maybe sets to ignore bytes until next timeout
0227: 20 17           bra    $0240

; --- Good input.. First byte ---
0229: C4 0F           andb   #$0F
022B: C1 0F           cmpb   #$0F
022D: 26 08           bne    $0237
022F: 14 0B 80        bset   $0B #$80   ; orig: bset	*0x000b, #0x80
0232: CE 02 41        ldx    #$0241     ; orig: ldx	#0x0241
0235: 20 09           bra    $0240
0237: 4F              clra              ; orig: LSN is not #F... LSN contains packet size.
0238: 5A              decb
0239: ED 04           std    $04,x
023B: DD 09           std    *$09
023D: CE 02 41        ldx    #$0241     ; orig: ldx	#0x0241
0240: 39              rts

; ---  | Called by SCR ISR  -- Vector Set in Above Function | A Register is set with received byte ---
0241: A7 03           staa   $03,x      ; orig: Write byte
0243: 96 0B           ldaa   *$0B
0245: 85 80           bita   #$80
0247: 27 05           beq    $024E      ; orig: Last byte was FF, need to set size of packet
0249: CE 02 5B        ldx    #$025B     ; orig: Set Vector
024C: 20 0C           bra    $025A
024E: DC 09           ldd    *$09
0250: 26 05           bne    $0257
0252: CE 02 90        ldx    #$0290     ; orig: Set Vector
0255: 20 03           bra    $025A
0257: CE 02 6F        ldx    #$026F     ; orig: Set Vector
025A: 39              rts

; ---  | A Register is set with received byte ---
025B: A7 04           staa   $04,x
025D: CE 02 61        ldx    #$0261     ; orig: ldx	#0x0261
0260: 39              rts

; ---  | A Register is set with received byte ---
0261: 16              tab
0262: A6 04           ldaa   $04,x
0264: 83 00 01        subd   #$0001     ; orig: subd	#0x0001
0267: ED 04           std    $04,x
0269: DD 09           std    *$09
026B: CE 02 6F        ldx    #$026F     ; orig: ldx	#0x026f
026E: 39              rts

; ---  | A Register is set with received byte ---
026F: 8F              xgdx
0270: D3 07           addd   *$07
0272: 8F              xgdx
0273: A7 06           staa   $06,x
0275: DE 07           ldx    *$07
0277: 08              inx
0278: DF 07           stx    *$07
027A: DC 09           ldd    *$09
027C: 83 00 01        subd   #$0001     ; orig: subd	#0x0001
027F: DD 09           std    *$09
0281: 26 05           bne    $0288
0283: CE 02 90        ldx    #$0290     ; orig: ldx	#0x0290
0286: 20 03           bra    $028B
0288: CE 02 6F        ldx    #$026F     ; orig: ldx	#0x026f
028B: 39              rts

; ---  | A Register is set with received byte | Seems to be an ignore byte function.. ---
028C: CE 02 8C        ldx    #$028C     ; orig: ldx	#0x028c
028F: 39              rts

; ---  | A Register is set with received byte ---
0290: 96 02           ldaa   *$02
0292: 4C              inca
0293: 26 16           bne    $02AB      ; orig: Uh-oh, Branch if not 0
0295: 97 02           staa   *$02       ; orig: Store received byte in 0x0002
0297: 86 FF           ldaa   #$FF
0299: A7 00           staa   $00,x
029B: CE 10 22        ldx    #$1022     ; TMSK1 (timer mask 1) | orig: ldx	#0x1022       Timer
029E: 1D 00 80        bclr   $00,x #$80 ; orig: bclr	0x0,x, #0x80
02A1: 86 50           ldaa   #$50
02A3: B7 10 2F        staa   $102F      ; SCDR (SCI data) | orig: Send 0x50 to serial port.
02A6: CE 01 F3        ldx    #$01F3     ; orig: Back to First Byte Handler
02A9: 20 06           bra    $02B1
02AB: 7F 00 02        clr    $0002      ; orig: clr	0x0002
02AE: CE 02 8C        ldx    #$028C     ; orig: Maybe ignore bytes until timeout
02B1: 39              rts

; --- Pickup on boot....part 3 | Main Service Loop ---
02B2: 7F 00 0C        clr    $000C      ; orig: clr	0x000c     Clear ram byte 000C
02B5: CE 01 F3        ldx    #$01F3     ; orig: ldx	#0x01f3
02B8: DF 03           stx    *$03       ; orig: Store 01 to 0003 and F3 to 0004
02BA: CE 04 00        ldx    #$0400     ; orig: ldx	#0x0400
02BD: 18 CE 08 00     ldy    #$0800
02C1: 1A EF 01        sty    $01,x      ; orig: sty	0x1,x      Store 08 to 0401 and 00  to 0402
02C4: CD EF 01        stx    $01,y      ; orig: stx	0x1,y      Store 94 to 0801 and 00 to  0802
02C7: 4F              clra
02C8: A7 00           staa   $00,x      ; orig: store 00 to 0400
02CA: 18 A7 00        staa   $00,y      ; orig: staa	0x0,y   store 00 to 0800
02CD: 97 02           staa   *$02
02CF: CE 04 00        ldx    #$0400     ; orig: ldx	#0x0400
02D2: DF 00           stx    *$00       ; orig: Store 04 to 0000 and 00 to 0001
02D4: CC FF FF        ldd    #$FFFF     ; orig: ldd	#0xffff
02D7: 1A EE 01        ldy    $01,x      ; orig: ldy	0x1,x       Set Y to value in 0401  which is 0800
02DA: 18 ED 06        std    $06,y      ; orig: std	0x6,y       Write FFFF to 0806
02DD: 18 A7 08        staa   $08,y      ; orig: staa	0x8,y   Write 00 to 0808
02E0: C6 64           ldab   #$64
02E2: BD 01 18        jsr    $0118      ; orig: Seems to be just delay method
02E5: 86 50           ldaa   #$50
02E7: B7 10 2F        staa   $102F      ; SCDR (SCI data) | orig: Write 50 to serial port
02EA: 0E              cli
02EB: DE 00           ldx    *$00       ; orig: Load from 0000 which should be 0400
02ED: 6D 00           tst    $00,x      ; orig: Look at 0400 for no zero
02EF: 27 FC           beq    $02ED      ; orig: loop until not 0
02F1: 1A EE 01        ldy    $01,x      ; orig: ldy	0x1,x
02F4: 18 6F 00        clr    $00,y      ; orig: clr	0x0,y
02F7: 18 DF 00        sty    *$00       ; orig: sty	*0x00
02FA: A6 03           ldaa   $03,x
02FC: 81 17           cmpa   #$17
02FE: 26 05           bne    $0305
0300: BD 03 8E        jsr    $038E      ; orig: jsr	0x038e   Jump if 17
0303: 20 27           bra    $032C
0305: 81 15           cmpa   #$15
0307: 26 05           bne    $030E
0309: BD 24 12        jsr    $2412      ; orig: jsr	0x2412  Jump if 15
030C: 20 1E           bra    $032C
030E: 81 11           cmpa   #$11
0310: 26 05           bne    $0317
0312: BD 2A E0        jsr    $2AE0      ; orig: jsr	0x2ae0  Jump if 11
0315: 20 15           bra    $032C
0317: 81 19           cmpa   #$19
0319: 26 05           bne    $0320
031B: BD 2B E8        jsr    $2BE8      ; orig: jsr	0x2be8  Jump if 19
031E: 20 0C           bra    $032C
0320: 81 13           cmpa   #$13
0322: 26 05           bne    $0329
0324: BD 2C 61        jsr    $2C61      ; orig: jsr	0x2c61   Jump if 13
0327: 20 03           bra    $032C
0329: BD 03 7C        jsr    $037C      ; orig: jsr	0x037c  /Jump if not matched
032C: 20 BD           bra    $02EB

; --- SBEP Ackowledgement Packet ---
032E: 3C              pshx
032F: 8F              xgdx
0330: FC 00 03        ldd    $0003      ; orig: ldd	0x0003
0333: 1A 83 01 F3     cpd    #$01F3
0337: 26 F7           bne    $0330
0339: C6 FF           ldab   #$FF
033B: 0F              sei
033C: B6 10 2E        ldaa   $102E      ; SCSR (SCI status) | orig: ldaa	0x102e
033F: 2A FB           bpl    $033C
0341: 18 A6 03        ldaa   $03,y      ; orig: ldaa	0x3,y
0344: B7 10 2F        staa   $102F      ; SCDR (SCI data) | orig: Send Serial Port
0347: B6 10 2E        ldaa   $102E      ; SCSR (SCI status) | orig: ldaa	0x102e
034A: 2A FB           bpl    $0347
034C: 18 E0 03        subb   $03,y      ; orig: subb	0x3,y
034F: 18 08           iny
0351: 09              dex
0352: 26 E8           bne    $033C
0354: F7 10 2F        stab   $102F      ; SCDR (SCI data) | orig: stab	0x102f
0357: B6 10 2E        ldaa   $102E      ; SCSR (SCI status) | orig: ldaa	0x102e
035A: 2A FB           bpl    $0357
035C: F6 10 2E        ldab   $102E      ; SCSR (SCI status) | orig: ldab	0x102e
035F: F6 10 2F        ldab   $102F      ; SCDR (SCI data) | orig: ldab	0x102f
0362: 0E              cli
0363: 38              pulx
0364: 39              rts

; ---  | Function Write  SPDR — SPI Data Register with contents of A ---
0365: 37              pshb
0366: F6 10 29        ldab   $1029      ; SPSR (SPI status) | orig: ldab	0x1029
0369: B7 10 2A        staa   $102A      ; SPDR (SPI data) | orig: staa	0x102a  Write SPDR
036C: F6 10 29        ldab   $1029      ; SPSR (SPI status) | orig: ldab	0x1029
036F: 2A FB           bpl    $036C      ; orig: Wait for SPSR
0371: 33              pulb
0372: 39              rts
0373: 3C              pshx
0374: CE 10 00        ldx    #$1000     ; orig: ldx	#0x1000
0377: 1C 61 80        bset   $61,x #$80 ; orig: bset	0x61,x, #0x80  Set 0x1061 MSB
037A: 38              pulx
037B: 39              rts

; --- Not Matched ---
037C: 18 CE 0C 00     ldy    #$0C00
0380: 86 F1           ldaa   #$F1
0382: C6 01           ldab   #$01
0384: 18 ED 03        std    $03,y      ; orig: std	0x3,y
0387: CC 00 02        ldd    #$0002     ; orig: ldd	#0x0002
038A: BD 03 2E        jsr    $032E      ; orig: jsr	0x032e
038D: 39              rts

; --- Command 17  Write Memory Contents ---
038E: 15 0C 80        bclr   $0C #$80   ; orig: bclr	*0x000c, #0x80          Clear MSB of 000C
0391: A6 06           ldaa   $06,x      ; orig: Look at MSB of Address (3 byte address)
0393: 84 F0           anda   #$F0
0395: 81 40           cmpa   #$40
0397: 26 05           bne    $039E
0399: BD 2E F4        jsr    $2EF4      ; orig: Not Availble until Part 2 Loaded
039C: 20 27           bra    $03C5
039E: 81 50           cmpa   #$50
03A0: 26 05           bne    $03A7
03A2: BD 2E C5        jsr    $2EC5      ; orig: Not Availble until Part 2 Loaded
03A5: 20 1E           bra    $03C5
03A7: 81 00           cmpa   #$00
03A9: 26 05           bne    $03B0
03AB: BD 2E C6        jsr    $2EC6      ; orig: Not Availble until Part 2 Loaded
03AE: 20 15           bra    $03C5
03B0: 81 80           cmpa   #$80
03B2: 26 05           bne    $03B9
03B4: BD 03 CD        jsr    $03CD      ; orig: Ram Load?
03B7: 20 0C           bra    $03C5
03B9: 81 E0           cmpa   #$E0
03BB: 26 05           bne    $03C2
03BD: BD 2F 53        jsr    $2F53      ; orig: Not Availble until Part 2 Loaded
03C0: 20 03           bra    $03C5
03C2: 14 0C 80        bset   $0C #$80   ; orig: Error flag?
03C5: E6 06           ldab   $06,x
03C7: EE 07           ldx    $07,x
03C9: BD 00 78        jsr    $0078      ; orig: jsr	0x0078
03CC: 39              rts

; --- Write Routing for MSB = 80 -- Local Ram? ---
03CD: 3C              pshx
03CE: 1A EE 07        ldy    $07,x      ; orig: ldy	0x7,x
03D1: EC 04           ldd    $04,x
03D3: 83 00 03        subd   #$0003     ; orig: subd	#0x0003
03D6: 36              psha
03D7: A6 09           ldaa   $09,x
03D9: 18 A7 00        staa   $00,y      ; orig: staa	0x0,y
03DC: 18 A1 00        cmpa   $00,y      ; orig: cmpa	0x0,y
03DF: 27 03           beq    $03E4
03E1: 14 0C 80        bset   $0C #$80   ; orig: bset	*0x000c, #0x80
03E4: 08              inx
03E5: 18 08           iny
03E7: 32              pula
03E8: 83 00 01        subd   #$0001     ; orig: subd	#0x0001
03EB: 26 E9           bne    $03D6
03ED: 38              pulx
03EE: 39              rts
03EF: FF FF FF        stx    $FFFF
03F2: FF FF FF        stx    $FFFF
03F5: FF FF FF        stx    $FFFF
03F8: FF FF FF        stx    $FFFF
03FB: FF FF FF        stx    $FFFF
03FE: FF FF           .byte ...
```

## Boot loader annotated disassembly

```asm
2000: 3C              pshx
2001: CE 10 00        ldx    #$1000
2004: 1C 6A 40        bset   $6A,x #$40
2007: 1C 6D 40        bset   $6D,x #$40
200A: 38              pulx
200B: 39              rts
200C: 39              rts
200D: 37              pshb
200E: 3C              pshx
200F: 7F 10 35        clr    $1035      ; BPROT-shadow / F1 ext (verify)
2012: CE FE 00        ldx    #$FE00
2015: C6 06           ldab   #$06
2017: F7 10 3B        stab   $103B      ; PPROG (program ctl)
201A: F7 FE 00        stab   $FE00
201D: C6 07           ldab   #$07
201F: F7 10 3B        stab   $103B      ; PPROG (program ctl)
2022: C6 0A           ldab   #$0A
2024: BD 01 18        jsr    $0118
2027: 7F 10 3B        clr    $103B      ; PPROG (program ctl)
202A: E6 00           ldab   $00,x
202C: 5C              incb
202D: 27 03           beq    $2032
202F: 14 0C 80        bset   $0C #$80
2032: 08              inx
2033: 26 F5           bne    $202A
2035: 38              pulx
2036: 33              pulb
2037: 39              rts
2038: 39              rts
2039: 39              rts
203A: 36              psha
203B: 96 11           ldaa   *$11
203D: 81 D5           cmpa   #$D5
203F: 26 04           bne    $2045
2041: 86 07           ldaa   #$07
2043: 20 29           bra    $206E
2045: 81 BD           cmpa   #$BD
2047: 26 04           bne    $204D
2049: 86 0F           ldaa   #$0F
204B: 20 21           bra    $206E
204D: 81 2A           cmpa   #$2A
204F: 26 04           bne    $2055
2051: 86 0F           ldaa   #$0F
2053: 20 19           bra    $206E
2055: 81 DA           cmpa   #$DA
2057: 26 04           bne    $205D
2059: 86 0F           ldaa   #$0F
205B: 20 11           bra    $206E
205D: 81 5B           cmpa   #$5B
205F: 26 04           bne    $2065
2061: 86 1F           ldaa   #$1F
2063: 20 09           bra    $206E
2065: 81 A2           cmpa   #$A2
2067: 26 04           bne    $206D
2069: 86 3F           ldaa   #$3F
206B: 20 01           bra    $206E
206D: 4F              clra
206E: 97 1C           staa   *$1C
2070: 96 13           ldaa   *$13
2072: 81 D5           cmpa   #$D5
2074: 26 04           bne    $207A
2076: 86 07           ldaa   #$07
2078: 20 19           bra    $2093
207A: 81 BD           cmpa   #$BD
207C: 26 04           bne    $2082
207E: 86 0F           ldaa   #$0F
2080: 20 11           bra    $2093
2082: 81 2A           cmpa   #$2A
2084: 26 04           bne    $208A
2086: 86 0F           ldaa   #$0F
2088: 20 09           bra    $2093
208A: 81 DA           cmpa   #$DA
208C: 26 04           bne    $2092
208E: 86 0F           ldaa   #$0F
2090: 20 01           bra    $2093
2092: 4F              clra
2093: 97 1D           staa   *$1D
2095: 32              pula
2096: 39              rts
2097: 00              test
2098: 36              psha
2099: 3C              pshx
209A: BD 2B 92        jsr    $2B92
209D: BD 01 00        jsr    $0100
20A0: 4F              clra
20A1: 7F 10 62        clr    $1062
20A4: CE 40 00        ldx    #$4000
20A7: A8 00           eora   $00,x
20A9: 08              inx
20AA: 8C 80 00        cpx    #$8000
20AD: 26 F8           bne    $20A7
20AF: BD 27 55        jsr    $2755
20B2: 24 F0           bcc    $20A4
20B4: BD 03 73        jsr    $0373
20B7: 4C              inca
20B8: 26 03           bne    $20BD
20BA: 0C              clc
20BB: 20 01           bra    $20BE
20BD: 0D              sec
20BE: 38              pulx
20BF: 32              pula
20C0: 39              rts
20C1: C6 01           ldab   #$01
20C3: BD 21 60        jsr    $2160
20C6: C6 AA           ldab   #$AA
20C8: BD 01 11        jsr    $0111
20CB: 39              rts
20CC: 16              tab
20CD: 44              lsra
20CE: 44              lsra
20CF: 44              lsra
20D0: 44              lsra
20D1: 81 09           cmpa   #$09
20D3: 22 04           bhi    $20D9
20D5: 8B 30           adda   #$30
20D7: 20 02           bra    $20DB
20D9: 8B 37           adda   #$37
20DB: C4 0F           andb   #$0F
20DD: C1 09           cmpb   #$09
20DF: 22 04           bhi    $20E5
20E1: CB 30           addb   #$30
20E3: 20 02           bra    $20E7
20E5: CB 37           addb   #$37
20E7: 39              rts
20E8: 37              pshb
20E9: C6 0F           ldab   #$0F
20EB: BD 01 18        jsr    $0118
20EE: C6 30           ldab   #$30
20F0: BD 21 64        jsr    $2164
20F3: C6 04           ldab   #$04
20F5: BD 01 18        jsr    $0118
20F8: C6 30           ldab   #$30
20FA: BD 21 64        jsr    $2164
20FD: C6 04           ldab   #$04
20FF: BD 01 18        jsr    $0118
2102: C6 30           ldab   #$30
2104: BD 21 64        jsr    $2164
2107: C6 04           ldab   #$04
2109: BD 01 18        jsr    $0118
210C: C6 30           ldab   #$30
210E: BD 21 64        jsr    $2164
2111: C6 04           ldab   #$04
2113: BD 01 11        jsr    $0111
2116: C6 08           ldab   #$08
2118: BD 21 64        jsr    $2164
211B: C6 04           ldab   #$04
211D: BD 01 11        jsr    $0111
2120: C6 0C           ldab   #$0C
2122: BD 21 64        jsr    $2164
2125: C6 04           ldab   #$04
2127: BD 01 11        jsr    $0111
212A: BD 21 35        jsr    $2135
212D: BD 20 C1        jsr    $20C1
2130: BD 21 7B        jsr    $217B
2133: 33              pulb
2134: 39              rts
2135: 37              pshb
2136: C6 02           ldab   #$02
2138: BD 21 60        jsr    $2160
213B: C6 AA           ldab   #$AA
213D: BD 01 11        jsr    $0111
2140: 33              pulb
2141: 39              rts
2142: BD 21 46        jsr    $2146
2145: 39              rts
2146: 36              psha
2147: 37              pshb
2148: BD 27 43        jsr    $2743
214B: BD 27 31        jsr    $2731
214E: 17              tba
214F: BD 03 65        jsr    $0365
2152: BD 27 3A        jsr    $273A
2155: BD 27 4C        jsr    $274C
2158: C6 05           ldab   #$05
215A: BD 01 11        jsr    $0111
215D: 33              pulb
215E: 32              pula
215F: 39              rts
2160: BD 21 64        jsr    $2164
2163: 39              rts
2164: 36              psha
2165: 37              pshb
2166: BD 27 4C        jsr    $274C
2169: BD 27 31        jsr    $2731
216C: 17              tba
216D: BD 03 65        jsr    $0365
2170: BD 27 3A        jsr    $273A
2173: C6 05           ldab   #$05
2175: BD 01 11        jsr    $0111
2178: 33              pulb
2179: 32              pula
217A: 39              rts
217B: 37              pshb
217C: C6 06           ldab   #$06
217E: BD 21 60        jsr    $2160
2181: C6 0A           ldab   #$0A
2183: BD 01 11        jsr    $0111
2186: 33              pulb
2187: 39              rts
2188: 36              psha
2189: 37              pshb
218A: BD 20 CC        jsr    $20CC
218D: 37              pshb
218E: 16              tab
218F: BD 21 42        jsr    $2142
2192: 33              pulb
2193: BD 21 42        jsr    $2142
2196: 32              pula
2197: 36              psha
2198: BD 20 CC        jsr    $20CC
219B: 37              pshb
219C: 16              tab
219D: BD 21 42        jsr    $2142
21A0: 33              pulb
21A1: BD 21 42        jsr    $2142
21A4: 33              pulb
21A5: 32              pula
21A6: 39              rts
21A7: BD 20 C1        jsr    $20C1
21AA: BD 21 35        jsr    $2135
21AD: BD 21 7B        jsr    $217B
21B0: E6 00           ldab   $00,x
21B2: 27 08           beq    $21BC
21B4: BD 21 42        jsr    $2142
21B7: 08              inx
21B8: E6 00           ldab   $00,x
21BA: 20 F6           bra    $21B2
21BC: 39              rts
21BD: 37              pshb
21BE: 3C              pshx
21BF: F6 10 62        ldab   $1062
21C2: C4 C0           andb   #$C0
21C4: F7 10 62        stab   $1062
21C7: C6 80           ldab   #$80
21C9: BD 2C 18        jsr    $2C18
21CC: C6 10           ldab   #$10
21CE: BD 2C 18        jsr    $2C18
21D1: C6 14           ldab   #$14
21D3: BD 01 18        jsr    $0118
21D6: BD 01 00        jsr    $0100
21D9: CE 40 00        ldx    #$4000
21DC: E6 00           ldab   $00,x
21DE: C1 FF           cmpb   #$FF
21E0: 27 03           beq    $21E5
21E2: 14 0C 80        bset   $0C #$80
21E5: 08              inx
21E6: 8C 80 00        cpx    #$8000
21E9: 26 F1           bne    $21DC
21EB: 7C 10 62        inc    $1062
21EE: F6 10 62        ldab   $1062
21F1: C4 3F           andb   #$3F
21F3: C1 08           cmpb   #$08
21F5: 26 E2           bne    $21D9
21F7: BD 03 73        jsr    $0373
21FA: 38              pulx
21FB: 33              pulb
21FC: 39              rts
21FD: 36              psha
21FE: 37              pshb
21FF: 3C              pshx
2200: BD 30 81        jsr    $3081
2203: 96 0C           ldaa   *$0C
2205: 85 80           bita   #$80
2207: 26 5C           bne    $2265
2209: B6 10 62        ldaa   $1062
220C: 84 C0           anda   #$C0
220E: B7 10 62        staa   $1062
2211: CC 0B B8        ldd    #$0BB8
2214: DD 0E           std    *$0E
2216: CE 40 00        ldx    #$4000
2219: 86 A0           ldaa   #$A0
221B: A7 00           staa   $00,x
221D: BD 01 09        jsr    $0109
2220: BD 01 00        jsr    $0100
2223: A6 00           ldaa   $00,x
2225: BD 03 73        jsr    $0373
2228: 4C              inca
2229: 27 16           beq    $2241
222B: 86 20           ldaa   #$20
222D: B7 40 00        staa   $4000
2230: B7 40 00        staa   $4000
2233: C6 0A           ldab   #$0A
2235: BD 01 18        jsr    $0118
2238: DC 0E           ldd    *$0E
223A: 83 00 01        subd   #$0001
223D: DD 0E           std    *$0E
223F: 20 01           bra    $2242
2241: 08              inx
2242: DC 0E           ldd    *$0E
2244: 27 05           beq    $224B
2246: 8C 80 00        cpx    #$8000
2249: 26 CE           bne    $2219
224B: DE 0E           ldx    *$0E
224D: 7C 10 62        inc    $1062
2250: B6 10 62        ldaa   $1062
2253: 84 3F           anda   #$3F
2255: 81 10           cmpa   #$10
2257: 27 05           beq    $225E
2259: 8C 00 00        cpx    #$0000
225C: 26 B8           bne    $2216
225E: DE 0E           ldx    *$0E
2260: 26 03           bne    $2265
2262: 14 0C 80        bset   $0C #$80
2265: 38              pulx
2266: 33              pulb
2267: 32              pula
2268: 39              rts
2269: 37              pshb
226A: 3C              pshx
226B: F6 10 62        ldab   $1062
226E: C4 C0           andb   #$C0
2270: F7 10 62        stab   $1062
2273: BD 27 A4        jsr    $27A4
2276: 86 FF           ldaa   #$FF
2278: B7 40 00        staa   $4000
227B: C6 14           ldab   #$14
227D: BD 01 18        jsr    $0118
2280: C6 80           ldab   #$80
2282: BD 2C 18        jsr    $2C18
2285: C6 10           ldab   #$10
2287: BD 2C 18        jsr    $2C18
228A: C6 14           ldab   #$14
228C: BD 01 18        jsr    $0118
228F: BD 01 00        jsr    $0100
2292: CE 40 00        ldx    #$4000
2295: E6 00           ldab   $00,x
2297: C1 FF           cmpb   #$FF
2299: 27 03           beq    $229E
229B: 14 0C 80        bset   $0C #$80
229E: 08              inx
229F: 8C 80 00        cpx    #$8000
22A2: 26 F1           bne    $2295
22A4: 7C 10 62        inc    $1062
22A7: F6 10 62        ldab   $1062
22AA: C4 3F           andb   #$3F
22AC: C1 10           cmpb   #$10
22AE: 26 E2           bne    $2292
22B0: BD 03 73        jsr    $0373
22B3: 38              pulx
22B4: 33              pulb
22B5: 39              rts
22B6: 37              pshb
22B7: 3C              pshx
22B8: F6 10 62        ldab   $1062
22BB: C4 C0           andb   #$C0
22BD: F7 10 62        stab   $1062
22C0: C6 80           ldab   #$80
22C2: BD 2C 18        jsr    $2C18
22C5: C6 10           ldab   #$10
22C7: BD 2C 18        jsr    $2C18
22CA: C6 14           ldab   #$14
22CC: BD 01 18        jsr    $0118
22CF: BD 01 00        jsr    $0100
22D2: CE 40 00        ldx    #$4000
22D5: E6 00           ldab   $00,x
22D7: C1 FF           cmpb   #$FF
22D9: 27 03           beq    $22DE
22DB: 14 0C 80        bset   $0C #$80
22DE: 08              inx
22DF: 8C 80 00        cpx    #$8000
22E2: 26 F1           bne    $22D5
22E4: 7C 10 62        inc    $1062
22E7: F6 10 62        ldab   $1062
22EA: C4 3F           andb   #$3F
22EC: C1 20           cmpb   #$20
22EE: 26 E2           bne    $22D2
22F0: BD 03 73        jsr    $0373
22F3: 38              pulx
22F4: 33              pulb
22F5: 39              rts
22F6: 36              psha
22F7: 37              pshb
22F8: B6 10 62        ldaa   $1062
22FB: 84 C0           anda   #$C0
22FD: B7 10 62        staa   $1062
2300: 86 FF           ldaa   #$FF
2302: B7 40 00        staa   $4000
2305: 86 50           ldaa   #$50
2307: B7 40 00        staa   $4000
230A: CC 20 D0        ldd    #$20D0
230D: B7 40 00        staa   $4000
2310: F7 40 00        stab   $4000
2313: BD 2D 63        jsr    $2D63
2316: 27 03           beq    $231B
2318: 14 0C 80        bset   $0C #$80
231B: B6 10 62        ldaa   $1062
231E: 8B 04           adda   #$04
2320: B7 10 62        staa   $1062
2323: 84 3F           anda   #$3F
2325: 26 E3           bne    $230A
2327: B6 10 62        ldaa   $1062
232A: 84 C0           anda   #$C0
232C: B7 10 62        staa   $1062
232F: 86 FF           ldaa   #$FF
2331: B7 40 00        staa   $4000
2334: 33              pulb
2335: 32              pula
2336: 39              rts
2337: 39              rts
2338: 36              psha
2339: BD 2D 20        jsr    $2D20
233C: 96 11           ldaa   *$11
233E: 81 BD           cmpa   #$BD
2340: 26 08           bne    $234A
2342: 7F 10 62        clr    $1062
2345: BD 21 FD        jsr    $21FD
2348: 20 3F           bra    $2389
234A: 81 2A           cmpa   #$2A
234C: 26 08           bne    $2356
234E: 7F 10 62        clr    $1062
2351: BD 21 FD        jsr    $21FD
2354: 20 33           bra    $2389
2356: 81 A2           cmpa   #$A2
2358: 26 08           bne    $2362
235A: 7F 10 62        clr    $1062
235D: BD 22 F6        jsr    $22F6
2360: 20 27           bra    $2389
2362: 81 D5           cmpa   #$D5
2364: 26 08           bne    $236E
2366: 7F 10 62        clr    $1062
2369: BD 21 BD        jsr    $21BD
236C: 20 1B           bra    $2389
236E: 81 DA           cmpa   #$DA
2370: 26 08           bne    $237A
2372: 7F 10 62        clr    $1062
2375: BD 22 69        jsr    $2269
2378: 20 0F           bra    $2389
237A: 81 5B           cmpa   #$5B
237C: 26 08           bne    $2386
237E: 7F 10 62        clr    $1062
2381: BD 22 B6        jsr    $22B6
2384: 20 03           bra    $2389
2386: 14 0C 80        bset   $0C #$80
2389: 96 13           ldaa   *$13
238B: 81 FF           cmpa   #$FF
238D: 27 57           beq    $23E6
238F: 81 BD           cmpa   #$BD
2391: 26 0A           bne    $239D
2393: 86 40           ldaa   #$40
2395: B7 10 62        staa   $1062
2398: BD 21 FD        jsr    $21FD
239B: 20 49           bra    $23E6
239D: 81 2A           cmpa   #$2A
239F: 26 0A           bne    $23AB
23A1: 86 40           ldaa   #$40
23A3: B7 10 62        staa   $1062
23A6: BD 21 FD        jsr    $21FD
23A9: 20 3B           bra    $23E6
23AB: 81 A2           cmpa   #$A2
23AD: 26 0A           bne    $23B9
23AF: 86 40           ldaa   #$40
23B1: B7 10 62        staa   $1062
23B4: BD 22 F6        jsr    $22F6
23B7: 20 2D           bra    $23E6
23B9: 81 D5           cmpa   #$D5
23BB: 26 0A           bne    $23C7
23BD: 86 40           ldaa   #$40
23BF: B7 10 62        staa   $1062
23C2: BD 21 BD        jsr    $21BD
23C5: 20 1F           bra    $23E6
23C7: 81 DA           cmpa   #$DA
23C9: 26 0A           bne    $23D5
23CB: 86 40           ldaa   #$40
23CD: B7 10 62        staa   $1062
23D0: BD 22 69        jsr    $2269
23D3: 20 11           bra    $23E6
23D5: 81 5B           cmpa   #$5B
23D7: 26 0A           bne    $23E3
23D9: 86 40           ldaa   #$40
23DB: B7 10 62        staa   $1062
23DE: BD 22 B6        jsr    $22B6
23E1: 20 03           bra    $23E6
23E3: 14 0C 80        bset   $0C #$80
23E6: BD 27 9B        jsr    $279B
23E9: 32              pula
23EA: 39              rts
23EB: 37              pshb
23EC: 18 E6 00        ldab   $00,y
23EF: 5C              incb
23F0: 27 1E           beq    $2410
23F2: C6 16           ldab   #$16
23F4: F7 10 3B        stab   $103B      ; PPROG (program ctl)
23F7: 18 E7 00        stab   $00,y
23FA: C6 17           ldab   #$17
23FC: F7 10 3B        stab   $103B      ; PPROG (program ctl)
23FF: C6 0A           ldab   #$0A
2401: BD 01 18        jsr    $0118
2404: 7F 10 3B        clr    $103B      ; PPROG (program ctl)
2407: 18 E6 00        ldab   $00,y
240A: 5C              incb
240B: 27 03           beq    $2410
240D: 14 0C 80        bset   $0C #$80
2410: 33              pulb
2411: 39              rts
2412: 15 0C 80        bclr   $0C #$80
2415: E6 06           ldab   $06,x
2417: C4 F0           andb   #$F0
2419: C1 40           cmpb   #$40
241B: 26 05           bne    $2422
241D: BD 23 38        jsr    $2338
2420: 20 15           bra    $2437
2422: C1 00           cmpb   #$00
2424: 26 05           bne    $242B
2426: BD 20 0D        jsr    $200D
2429: 20 0C           bra    $2437
242B: C1 50           cmpb   #$50
242D: 26 05           bne    $2434
242F: BD 23 37        jsr    $2337
2432: 20 03           bra    $2437
2434: 14 0C 80        bset   $0C #$80
2437: CA 0F           orab   #$0F
2439: CE FF FF        ldx    #$FFFF
243C: BD 00 78        jsr    $0078
243F: 39              rts
2440: 36              psha
2441: 37              pshb
2442: 3C              pshx
2443: 18 3C           pshy
2445: 3C              pshx
2446: 18 CE 25 7D     ldy    #$257D
244A: CE 00 0E        ldx    #$000E
244D: C6 10           ldab   #$10
244F: BD 29 C6        jsr    $29C6
2452: 18 A7 00        staa   $00,y
2455: 08              inx
2456: 18 08           iny
2458: 5A              decb
2459: 26 F4           bne    $244F
245B: 18 CE 25 8D     ldy    #$258D
245F: BD 01 00        jsr    $0100
2462: CC 03 C0        ldd    #$03C0
2465: BD 2C 45        jsr    $2C45
2468: CE 7F F0        ldx    #$7FF0
246B: C6 10           ldab   #$10
246D: A6 00           ldaa   $00,x
246F: 18 A7 00        staa   $00,y
2472: 08              inx
2473: 18 08           iny
2475: 5A              decb
2476: 26 F5           bne    $246D
2478: BD 03 73        jsr    $0373
247B: 18 CE 25 9D     ldy    #$259D
247F: BD 01 00        jsr    $0100
2482: CC 00 00        ldd    #$0000
2485: BD 2C 45        jsr    $2C45
2488: B6 70 00        ldaa   $7000
248B: 18 A7 00        staa   $00,y
248E: CC 00 80        ldd    #$0080
2491: BD 2C 45        jsr    $2C45
2494: B6 70 00        ldaa   $7000
2497: 18 A7 01        staa   $01,y
249A: CC 01 00        ldd    #$0100
249D: BD 2C 45        jsr    $2C45
24A0: B6 70 00        ldaa   $7000
24A3: 18 A7 02        staa   $02,y
24A6: CC 01 80        ldd    #$0180
24A9: BD 2C 45        jsr    $2C45
24AC: B6 70 00        ldaa   $7000
24AF: 18 A7 03        staa   $03,y
24B2: CC 02 00        ldd    #$0200
24B5: BD 2C 45        jsr    $2C45
24B8: B6 70 00        ldaa   $7000
24BB: 18 A7 04        staa   $04,y
24BE: CC 02 80        ldd    #$0280
24C1: BD 2C 45        jsr    $2C45
24C4: B6 70 00        ldaa   $7000
24C7: 18 A7 05        staa   $05,y
24CA: CC 03 00        ldd    #$0300
24CD: BD 2C 45        jsr    $2C45
24D0: B6 70 00        ldaa   $7000
24D3: 18 A7 06        staa   $06,y
24D6: CC 03 80        ldd    #$0380
24D9: BD 2C 45        jsr    $2C45
24DC: B6 70 00        ldaa   $7000
24DF: 18 A7 07        staa   $07,y
24E2: BD 03 73        jsr    $0373
24E5: 18 CE 25 A5     ldy    #$25A5
24E9: 18 6F 00        clr    $00,y
24EC: 18 6F 01        clr    $01,y
24EF: 18 6F 02        clr    $02,y
24F2: 18 CE 25 A8     ldy    #$25A8
24F6: CE 00 2E        ldx    #$002E
24F9: BD 29 C6        jsr    $29C6
24FC: 36              psha
24FD: 08              inx
24FE: BD 29 C6        jsr    $29C6
2501: 16              tab
2502: 32              pula
2503: C3 00 03        addd   #$0003
2506: 8F              xgdx
2507: C6 19           ldab   #$19
2509: BD 29 C6        jsr    $29C6
250C: 18 A7 00        staa   $00,y
250F: 08              inx
2510: 18 08           iny
2512: 5A              decb
2513: 26 F4           bne    $2509
2515: 18 CE 25 C1     ldy    #$25C1
2519: C6 0C           ldab   #$0C
251B: 18 6F 00        clr    $00,y
251E: 18 08           iny
2520: 5A              decb
2521: 26 F8           bne    $251B
2523: 18 CE 25 C1     ldy    #$25C1
2527: CE 00 2E        ldx    #$002E
252A: BD 29 C6        jsr    $29C6
252D: 36              psha
252E: 08              inx
252F: BD 29 C6        jsr    $29C6
2532: 16              tab
2533: 32              pula
2534: C3 00 1C        addd   #$001C
2537: 8F              xgdx
2538: BD 29 C6        jsr    $29C6
253B: 08              inx
253C: 16              tab
253D: BD 29 C6        jsr    $29C6
2540: 18 A7 00        staa   $00,y
2543: 08              inx
2544: 18 08           iny
2546: 5A              decb
2547: 26 F4           bne    $253D
2549: 18 CE 25 CD     ldy    #$25CD
254D: CE 00 04        ldx    #$0004
2550: C6 0A           ldab   #$0A
2552: BD 29 C6        jsr    $29C6
2555: 18 A7 00        staa   $00,y
2558: 08              inx
2559: 18 08           iny
255B: 5A              decb
255C: 26 F4           bne    $2552
255E: 38              pulx
255F: CC 00 0D        ldd    #$000D
2562: ED 04           std    $04,x
2564: 6F 06           clr    $06,x
2566: CC 00 32        ldd    #$0032
2569: ED 07           std    $07,x
256B: 8F              xgdx
256C: C3 00 09        addd   #$0009
256F: 8F              xgdx
2570: 18 CE 25 7D     ldy    #$257D
2574: BD 20 38        jsr    $2038
2577: 18 38           puly
2579: 38              pulx
257A: 33              pulb
257B: 32              pula
257C: 39              rts
257D: FF FF FF        stx    $FFFF
2580: FF FF FF        stx    $FFFF
2583: FF FF FF        stx    $FFFF
2586: FF FF FF        stx    $FFFF
2589: FF FF FF        stx    $FFFF
258C: FF FF FF        stx    $FFFF
258F: FF FF FF        stx    $FFFF
2592: FF FF FF        stx    $FFFF
2595: FF FF FF        stx    $FFFF
2598: FF FF FF        stx    $FFFF
259B: FF FF FF        stx    $FFFF
259E: FF FF FF        stx    $FFFF
25A1: FF FF FF        stx    $FFFF
25A4: FF 00 00        stx    $0000
25A7: 00              test
25A8: FF FF FF        stx    $FFFF
25AB: FF FF FF        stx    $FFFF
25AE: FF FF FF        stx    $FFFF
25B1: FF FF FF        stx    $FFFF
25B4: FF FF FF        stx    $FFFF
25B7: FF FF FF        stx    $FFFF
25BA: FF FF FF        stx    $FFFF
25BD: FF FF FF        stx    $FFFF
25C0: FF FF FF        stx    $FFFF
25C3: FF FF FF        stx    $FFFF
25C6: FF FF FF        stx    $FFFF
25C9: FF FF FF        stx    $FFFF
25CC: FF FF FF        stx    $FFFF
25CF: FF FF FF        stx    $FFFF
25D2: FF FF FF        stx    $FFFF
25D5: FF FF 36        stx    $FF36
25D8: 37              pshb
25D9: 6F 06           clr    $06,x
25DB: CC 81 F0        ldd    #$81F0
25DE: ED 07           std    $07,x
25E0: 33              pulb
25E1: 32              pula
25E2: 39              rts
25E3: 36              psha
25E4: 37              pshb
25E5: 3C              pshx
25E6: 18 3C           pshy
25E8: CC 02 03        ldd    #$0203
25EB: ED 04           std    $04,x
25ED: CC 03 FE        ldd    #$03FE
25F0: ED 06           std    $06,x
25F2: 6F 08           clr    $08,x
25F4: 3C              pshx
25F5: 18 38           puly
25F7: 18 8F           xgdy
25F9: C3 01 F0        addd   #$01F0
25FC: 18 8F           xgdy
25FE: EC 09           ldd    $09,x
2600: 18 ED 09        std    $09,y
2603: EC 0B           ldd    $0B,x
2605: 18 ED 0B        std    $0B,y
2608: EC 0D           ldd    $0D,x
260A: 18 ED 0D        std    $0D,y
260D: EC 0F           ldd    $0F,x
260F: 18 ED 0F        std    $0F,y
2612: 4F              clra
2613: 8A 81           oraa   #$81
2615: B7 10 62        staa   $1062
2618: BD 01 00        jsr    $0100
261B: BD 2C EE        jsr    $2CEE
261E: FC 7F F8        ldd    $7FF8
2621: 18 ED 11        std    $11,y
2624: FC 7F FA        ldd    $7FFA
2627: 18 ED 13        std    $13,y
262A: FC 7F FC        ldd    $7FFC
262D: 18 ED 15        std    $15,y
2630: FC 7F FE        ldd    $7FFE
2633: 18 ED 17        std    $17,y
2636: BD 2C F7        jsr    $2CF7
2639: BD 03 73        jsr    $0373
263C: 18 3C           pshy
263E: CC 03 FC        ldd    #$03FC
2641: BD 2C 45        jsr    $2C45
2644: BD 2D 20        jsr    $2D20
2647: BD 2B 92        jsr    $2B92
264A: BD 27 9B        jsr    $279B
264D: BD 01 00        jsr    $0100
2650: 18 CE 7E 00     ldy    #$7E00
2654: CC 01 F0        ldd    #$01F0
2657: 36              psha
2658: 18 A6 00        ldaa   $00,y
265B: A7 09           staa   $09,x
265D: 32              pula
265E: 08              inx
265F: 18 08           iny
2661: 83 00 01        subd   #$0001
2664: 26 F1           bne    $2657
2666: BD 03 73        jsr    $0373
2669: 18 38           puly
266B: BD 2C D8        jsr    $2CD8
266E: 24 25           bcc    $2695
2670: CC 00 00        ldd    #$0000
2673: 18 E3 09        addd   $09,y
2676: 18 E3 0B        addd   $0B,y
2679: 18 E3 0D        addd   $0D,y
267C: 18 E3 11        addd   $11,y
267F: 18 E3 13        addd   $13,y
2682: 18 E3 15        addd   $15,y
2685: 18 E3 17        addd   $17,y
2688: 83 FF F8        subd   #$FFF8
268B: 43              coma
268C: 53              comb
268D: C3 00 01        addd   #$0001
2690: 18 ED 0F        std    $0F,y
2693: 20 34           bra    $26C9
2695: 4F              clra
2696: 18 A8 09        eora   $09,y
2699: 18 A8 0A        eora   $0A,y
269C: 18 A8 0B        eora   $0B,y
269F: 18 A8 0C        eora   $0C,y
26A2: 18 A8 0D        eora   $0D,y
26A5: 18 A8 0E        eora   $0E,y
26A8: 18 A8 0F        eora   $0F,y
26AB: 18 A8 10        eora   $10,y
26AE: 18 A8 11        eora   $11,y
26B1: 18 A8 12        eora   $12,y
26B4: 18 A8 13        eora   $13,y
26B7: 18 A8 14        eora   $14,y
26BA: 18 A8 15        eora   $15,y
26BD: 18 A8 16        eora   $16,y
26C0: 18 A8 17        eora   $17,y
26C3: 18 A8 18        eora   $18,y
26C6: 18 A7 10        staa   $10,y
26C9: 18 38           puly
26CB: 38              pulx
26CC: 33              pulb
26CD: 32              pula
26CE: 39              rts
26CF: 36              psha
26D0: 37              pshb
26D1: 3C              pshx
26D2: 18 3C           pshy
26D4: BD 29 C6        jsr    $29C6
26D7: 16              tab
26D8: 4F              clra
26D9: C3 00 01        addd   #$0001
26DC: 18 8F           xgdy
26DE: 5F              clrb
26DF: BD 29 C6        jsr    $29C6
26E2: 1B              aba
26E3: 16              tab
26E4: 08              inx
26E5: 18 09           dey
26E7: 26 F6           bne    $26DF
26E9: 53              comb
26EA: 86 AA           ldaa   #$AA
26EC: 10              sba
26ED: BD 2F 7E        jsr    $2F7E
26F0: 18 38           puly
26F2: 38              pulx
26F3: 33              pulb
26F4: 32              pula
26F5: 39              rts
26F6: 86 90           ldaa   #$90
26F8: B7 40 00        staa   $4000
26FB: BD 01 00        jsr    $0100
26FE: B6 40 00        ldaa   $4000
2701: F6 40 01        ldab   $4001
2704: BD 03 73        jsr    $0373
2707: 39              rts
2708: C6 90           ldab   #$90
270A: BD 2C 18        jsr    $2C18
270D: C6 0F           ldab   #$0F
270F: BD 01 18        jsr    $0118
2712: BD 01 00        jsr    $0100
2715: 7C 10 5C        inc    $105C      ; PORTH (F1)
2718: B6 40 00        ldaa   $4000
271B: F6 40 01        ldab   $4001
271E: 7A 10 5C        dec    $105C      ; PORTH (F1)
2721: BD 03 73        jsr    $0373
2724: 37              pshb
2725: C6 F0           ldab   #$F0
2727: BD 2C 18        jsr    $2C18
272A: C6 0F           ldab   #$0F
272C: BD 01 18        jsr    $0118
272F: 33              pulb
2730: 39              rts
2731: 3C              pshx
2732: CE 10 69        ldx    #$1069
2735: 1D 00 40        bclr   $00,x #$40
2738: 38              pulx
2739: 39              rts
273A: 3C              pshx
273B: CE 10 69        ldx    #$1069
273E: 1C 00 40        bset   $00,x #$40
2741: 38              pulx
2742: 39              rts
2743: 3C              pshx
2744: CE 10 6A        ldx    #$106A
2747: 1C 00 20        bset   $00,x #$20
274A: 38              pulx
274B: 39              rts
274C: 3C              pshx
274D: CE 10 6A        ldx    #$106A
2750: 1D 00 20        bclr   $00,x #$20
2753: 38              pulx
2754: 39              rts
2755: 36              psha
2756: 37              pshb
2757: B6 10 62        ldaa   $1062
275A: 16              tab
275B: 84 3F           anda   #$3F
275D: C5 40           bitb   #$40
275F: 26 1A           bne    $277B
2761: 91 1C           cmpa   *$1C
2763: 25 10           bcs    $2775
2765: 7D 00 1D        tst    $001D
2768: 27 08           beq    $2772
276A: 86 40           ldaa   #$40
276C: B7 10 62        staa   $1062
276F: 0C              clc
2770: 20 01           bra    $2773
2772: 0D              sec
2773: 20 04           bra    $2779
2775: 7C 10 62        inc    $1062
2778: 0C              clc
2779: 20 0B           bra    $2786
277B: 91 1D           cmpa   *$1D
277D: 25 03           bcs    $2782
277F: 0D              sec
2780: 20 04           bra    $2786
2782: 7C 10 62        inc    $1062
2785: 0C              clc
2786: 33              pulb
2787: 32              pula
2788: 39              rts
2789: 3C              pshx
278A: CE 10 00        ldx    #$1000
278D: 1D 60 20        bclr   $60,x #$20
2790: 38              pulx
2791: 39              rts
2792: 3C              pshx
2793: CE 10 00        ldx    #$1000
2796: 1C 35 0F        bset   $35,x #$0F
2799: 38              pulx
279A: 39              rts
279B: 3C              pshx
279C: CE 10 00        ldx    #$1000
279F: 1D 60 10        bclr   $60,x #$10
27A2: 38              pulx
27A3: 39              rts
27A4: 37              pshb
27A5: C6 80           ldab   #$80
27A7: BD 2C 18        jsr    $2C18
27AA: C6 20           ldab   #$20
27AC: BD 2C 18        jsr    $2C18
27AF: 33              pulb
27B0: 39              rts
27B1: 37              pshb
27B2: C6 A0           ldab   #$A0
27B4: BD 2C 18        jsr    $2C18
27B7: 33              pulb
27B8: 39              rts
27B9: 18 8C FE 00     cpy    #$FE00
27BD: 25 27           bcs    $27E6
27BF: 18 8C FF FF     cpy    #$FFFF
27C3: 22 21           bhi    $27E6
27C5: 37              pshb
27C6: C6 02           ldab   #$02
27C8: F7 10 3B        stab   $103B      ; PPROG (program ctl)
27CB: 18 A7 00        staa   $00,y
27CE: C6 03           ldab   #$03
27D0: F7 10 3B        stab   $103B      ; PPROG (program ctl)
27D3: C6 0A           ldab   #$0A
27D5: BD 01 18        jsr    $0118
27D8: 7F 10 3B        clr    $103B      ; PPROG (program ctl)
27DB: 18 A1 00        cmpa   $00,y
27DE: 27 03           beq    $27E3
27E0: 14 0C 80        bset   $0C #$80
27E3: 33              pulb
27E4: 20 03           bra    $27E9
27E6: 14 0C 80        bset   $0C #$80
27E9: 39              rts
27EA: 37              pshb
27EB: C6 19           ldab   #$19
27ED: 37              pshb
27EE: 0F              sei
27EF: C6 40           ldab   #$40
27F1: E7 00           stab   $00,x
27F3: A7 00           staa   $00,x
27F5: BD 01 0A        jsr    $010A
27F8: C6 C0           ldab   #$C0
27FA: E7 00           stab   $00,x
27FC: BD 01 00        jsr    $0100
27FF: E6 00           ldab   $00,x
2801: 0E              cli
2802: BD 03 73        jsr    $0373
2805: 11              cba
2806: 33              pulb
2807: 27 01           beq    $280A
2809: 5A              decb
280A: 26 E1           bne    $27ED
280C: 5D              tstb
280D: 26 03           bne    $2812
280F: 14 0C 80        bset   $0C #$80
2812: C6 00           ldab   #$00
2814: E7 00           stab   $00,x
2816: 33              pulb
2817: 39              rts
2818: 37              pshb
2819: C6 50           ldab   #$50
281B: F7 40 00        stab   $4000
281E: C6 40           ldab   #$40
2820: E7 00           stab   $00,x
2822: A7 00           staa   $00,x
2824: BD 2D 63        jsr    $2D63
2827: 27 03           beq    $282C
2829: 14 0C 80        bset   $0C #$80
282C: 33              pulb
282D: 39              rts
282E: 15 0C 80        bclr   $0C #$80
2831: 36              psha
2832: 3C              pshx
2833: BD 20 00        jsr    $2000
2836: 86 04           ldaa   #$04
2838: 97 1E           staa   *$1E
283A: 86 03           ldaa   #$03
283C: 97 1F           staa   *$1F
283E: BD 20 E8        jsr    $20E8
2841: CE 2D 4D        ldx    #$2D4D
2844: BD 21 A7        jsr    $21A7
2847: CE 10 00        ldx    #$1000
284A: 1C 26 03        bset   $26,x #$03
284D: CE 10 00        ldx    #$1000
2850: 1C 60 02        bset   $60,x #$02
2853: 38              pulx
2854: 32              pula
2855: BD 2A 7E        jsr    $2A7E
2858: 96 11           ldaa   *$11
285A: D6 10           ldab   *$10
285C: 1A 83 FF FF     cpd    #$FFFF
2860: 26 03           bne    $2865
2862: 14 0C 80        bset   $0C #$80
2865: C6 FF           ldab   #$FF
2867: CE FF FF        ldx    #$FFFF
286A: BD 00 78        jsr    $0078
286D: 3C              pshx
286E: CE 10 00        ldx    #$1000
2871: 1C 24 40        bset   $24,x #$40
2874: 38              pulx
2875: BD 2B 92        jsr    $2B92
2878: 39              rts
2879: 36              psha
287A: 37              pshb
287B: 3C              pshx
287C: 18 3C           pshy
287E: FC FE 02        ldd    $FE02
2881: 83 02 00        subd   #$0200
2884: C3 40 26        addd   #$4026
2887: 18 8F           xgdy
2889: 4F              clra
288A: 8A 80           oraa   #$80
288C: B7 10 62        staa   $1062
288F: BD 01 00        jsr    $0100
2892: BD 2C EE        jsr    $2CEE
2895: 18 EC 00        ldd    $00,y
2898: BD 2C F7        jsr    $2CF7
289B: BD 03 73        jsr    $0373
289E: 18 CE 0C 00     ldy    #$0C00
28A2: 18 ED 07        std    $07,y
28A5: 86 FF           ldaa   #$FF
28A7: C6 80           ldab   #$80
28A9: 18 ED 03        std    $03,y
28AC: 86 00           ldaa   #$00
28AE: C6 03           ldab   #$03
28B0: 18 ED 05        std    $05,y
28B3: CC 00 06        ldd    #$0006
28B6: BD 03 2E        jsr    $032E
28B9: 18 38           puly
28BB: 38              pulx
28BC: 33              pulb
28BD: 32              pula
28BE: 39              rts
28BF: 36              psha
28C0: 37              pshb
28C1: 3C              pshx
28C2: 18 3C           pshy
28C4: 18 CE 0C 00     ldy    #$0C00
28C8: 86 FF           ldaa   #$FF
28CA: C6 80           ldab   #$80
28CC: 18 ED 03        std    $03,y
28CF: 4F              clra
28D0: E6 06           ldab   $06,x
28D2: C3 00 04        addd   #$0004
28D5: 18 ED 05        std    $05,y
28D8: EC 07           ldd    $07,x
28DA: 18 ED 07        std    $07,y
28DD: A6 09           ldaa   $09,x
28DF: 18 A7 09        staa   $09,y
28E2: E6 06           ldab   $06,x
28E4: 37              pshb
28E5: EC 08           ldd    $08,x
28E7: 8F              xgdx
28E8: 33              pulb
28E9: 37              pshb
28EA: A6 00           ldaa   $00,x
28EC: 18 A7 0A        staa   $0A,y
28EF: 08              inx
28F0: 18 08           iny
28F2: 5A              decb
28F3: 26 F5           bne    $28EA
28F5: 18 CE 0C 00     ldy    #$0C00
28F9: 4F              clra
28FA: 33              pulb
28FB: C3 00 07        addd   #$0007
28FE: BD 03 2E        jsr    $032E
2901: 18 38           puly
2903: 38              pulx
2904: 33              pulb
2905: 32              pula
2906: 39              rts
2907: 36              psha
2908: 37              pshb
2909: 3C              pshx
290A: 18 3C           pshy
290C: BD 2D 20        jsr    $2D20
290F: BD 2B 92        jsr    $2B92
2912: BD 27 9B        jsr    $279B
2915: 18 CE 0C 00     ldy    #$0C00
2919: 86 FF           ldaa   #$FF
291B: C6 80           ldab   #$80
291D: 18 ED 03        std    $03,y
2920: CC 00 1F        ldd    #$001F
2923: 18 ED 05        std    $05,y
2926: 86 E0           ldaa   #$E0
2928: 18 A7 07        staa   $07,y
292B: 18 6F 08        clr    $08,y
292E: 18 6F 09        clr    $09,y
2931: BD 01 00        jsr    $0100
2934: CC 03 C0        ldd    #$03C0
2937: BD 2C 45        jsr    $2C45
293A: CE 7F F0        ldx    #$7FF0
293D: C6 10           ldab   #$10
293F: A6 00           ldaa   $00,x
2941: 18 A7 0A        staa   $0A,y
2944: 08              inx
2945: 18 08           iny
2947: 5A              decb
2948: 26 F5           bne    $293F
294A: CC 00 00        ldd    #$0000
294D: BD 2C 45        jsr    $2C45
2950: B6 70 00        ldaa   $7000
2953: 18 A7 0A        staa   $0A,y
2956: CC 00 80        ldd    #$0080
2959: BD 2C 45        jsr    $2C45
295C: B6 70 00        ldaa   $7000
295F: 18 A7 0B        staa   $0B,y
2962: CC 01 00        ldd    #$0100
2965: BD 2C 45        jsr    $2C45
2968: B6 70 00        ldaa   $7000
296B: 18 A7 0C        staa   $0C,y
296E: CC 01 80        ldd    #$0180
2971: BD 2C 45        jsr    $2C45
2974: B6 70 00        ldaa   $7000
2977: 18 A7 0D        staa   $0D,y
297A: CC 02 00        ldd    #$0200
297D: BD 2C 45        jsr    $2C45
2980: B6 70 00        ldaa   $7000
2983: 18 A7 0E        staa   $0E,y
2986: CC 02 80        ldd    #$0280
2989: BD 2C 45        jsr    $2C45
298C: B6 70 00        ldaa   $7000
298F: 18 A7 0F        staa   $0F,y
2992: CC 03 00        ldd    #$0300
2995: BD 2C 45        jsr    $2C45
2998: B6 70 00        ldaa   $7000
299B: 18 A7 10        staa   $10,y
299E: CC 03 80        ldd    #$0380
29A1: BD 2C 45        jsr    $2C45
29A4: B6 70 00        ldaa   $7000
29A7: 18 A7 11        staa   $11,y
29AA: 18 6F 12        clr    $12,y
29AD: 18 6F 13        clr    $13,y
29B0: 18 6F 14        clr    $14,y
29B3: BD 03 73        jsr    $0373
29B6: 18 CE 0C 00     ldy    #$0C00
29BA: CC 00 22        ldd    #$0022
29BD: BD 03 2E        jsr    $032E
29C0: 18 38           puly
29C2: 38              pulx
29C3: 33              pulb
29C4: 32              pula
29C5: 39              rts
29C6: 37              pshb
29C7: 3C              pshx
29C8: 8C 02 00        cpx    #$0200
29CB: 24 09           bcc    $29D6
29CD: 8F              xgdx
29CE: C3 FE 00        addd   #$FE00
29D1: 8F              xgdx
29D2: A6 00           ldaa   $00,x
29D4: 20 23           bra    $29F9
29D6: 8F              xgdx
29D7: 83 02 00        subd   #$0200
29DA: 37              pshb
29DB: 16              tab
29DC: 4F              clra
29DD: 05              asld
29DE: 05              asld
29DF: 8A 80           oraa   #$80
29E1: B7 10 62        staa   $1062
29E4: 54              lsrb
29E5: 54              lsrb
29E6: 17              tba
29E7: 33              pulb
29E8: 8A 40           oraa   #$40
29EA: 8F              xgdx
29EB: BD 01 00        jsr    $0100
29EE: BD 2C EE        jsr    $2CEE
29F1: A6 00           ldaa   $00,x
29F3: BD 2C F7        jsr    $2CF7
29F6: BD 03 73        jsr    $0373
29F9: 38              pulx
29FA: 33              pulb
29FB: 39              rts
29FC: 36              psha
29FD: 37              pshb
29FE: 3C              pshx
29FF: 18 3C           pshy
2A01: 18 CE 0C 00     ldy    #$0C00
2A05: 86 FF           ldaa   #$FF
2A07: C6 80           ldab   #$80
2A09: 18 ED 03        std    $03,y
2A0C: 4F              clra
2A0D: E6 06           ldab   $06,x
2A0F: C3 00 04        addd   #$0004
2A12: 18 ED 05        std    $05,y
2A15: EC 07           ldd    $07,x
2A17: 18 ED 07        std    $07,y
2A1A: A6 09           ldaa   $09,x
2A1C: 18 A7 09        staa   $09,y
2A1F: E6 06           ldab   $06,x
2A21: 37              pshb
2A22: EC 08           ldd    $08,x
2A24: 1A 83 02 00     cpd    #$0200
2A28: 24 06           bcc    $2A30
2A2A: C3 FE 00        addd   #$FE00
2A2D: 8F              xgdx
2A2E: 20 17           bra    $2A47
2A30: 7F 10 62        clr    $1062
2A33: 83 02 00        subd   #$0200
2A36: 37              pshb
2A37: 16              tab
2A38: 4F              clra
2A39: 05              asld
2A3A: 05              asld
2A3B: 8A 80           oraa   #$80
2A3D: B7 10 62        staa   $1062
2A40: 54              lsrb
2A41: 54              lsrb
2A42: 17              tba
2A43: 33              pulb
2A44: 8A 40           oraa   #$40
2A46: 8F              xgdx
2A47: 33              pulb
2A48: 37              pshb
2A49: BD 01 00        jsr    $0100
2A4C: BD 2C EE        jsr    $2CEE
2A4F: A6 00           ldaa   $00,x
2A51: 18 A7 0A        staa   $0A,y
2A54: 8C 7F FF        cpx    #$7FFF
2A57: 26 07           bne    $2A60
2A59: 8F              xgdx
2A5A: BD 27 55        jsr    $2755
2A5D: 84 3F           anda   #$3F
2A5F: 8F              xgdx
2A60: 08              inx
2A61: 18 08           iny
2A63: 5A              decb
2A64: 26 E9           bne    $2A4F
2A66: BD 2C F7        jsr    $2CF7
2A69: BD 03 73        jsr    $0373
2A6C: 18 CE 0C 00     ldy    #$0C00
2A70: 4F              clra
2A71: 33              pulb
2A72: C3 00 07        addd   #$0007
2A75: BD 03 2E        jsr    $032E
2A78: 18 38           puly
2A7A: 38              pulx
2A7B: 33              pulb
2A7C: 32              pula
2A7D: 39              rts
2A7E: 36              psha
2A7F: 37              pshb
2A80: BD 2D 20        jsr    $2D20
2A83: 7F 10 62        clr    $1062
2A86: BD 26 F6        jsr    $26F6
2A89: BD 2C 9B        jsr    $2C9B
2A8C: 24 10           bcc    $2A9E
2A8E: C6 15           ldab   #$15
2A90: BD 01 18        jsr    $0118
2A93: BD 27 08        jsr    $2708
2A96: BD 2C BA        jsr    $2CBA
2A99: 24 03           bcc    $2A9E
2A9B: CC FF FF        ldd    #$FFFF
2A9E: 97 10           staa   *$10
2AA0: D7 11           stab   *$11
2AA2: 86 40           ldaa   #$40
2AA4: B7 10 62        staa   $1062
2AA7: BD 26 F6        jsr    $26F6
2AAA: BD 2C 9B        jsr    $2C9B
2AAD: 24 10           bcc    $2ABF
2AAF: C6 0F           ldab   #$0F
2AB1: BD 01 18        jsr    $0118
2AB4: BD 27 08        jsr    $2708
2AB7: BD 2C BA        jsr    $2CBA
2ABA: 24 03           bcc    $2ABF
2ABC: CC FF FF        ldd    #$FFFF
2ABF: 97 12           staa   *$12
2AC1: D7 13           stab   *$13
2AC3: BD 20 3A        jsr    $203A
2AC6: 96 10           ldaa   *$10
2AC8: 81 FF           cmpa   #$FF
2ACA: 26 0E           bne    $2ADA
2ACC: 86 0F           ldaa   #$0F
2ACE: 97 1C           staa   *$1C
2AD0: BD 20 98        jsr    $2098
2AD3: 25 05           bcs    $2ADA
2AD5: 4F              clra
2AD6: 97 10           staa   *$10
2AD8: 97 11           staa   *$11
2ADA: BD 27 9B        jsr    $279B
2ADD: 33              pulb
2ADE: 32              pula
2ADF: 39              rts
2AE0: 36              psha
2AE1: A6 07           ldaa   $07,x
2AE3: 84 F0           anda   #$F0
2AE5: 81 40           cmpa   #$40
2AE7: 26 05           bne    $2AEE
2AE9: BD 2B 1B        jsr    $2B1B
2AEC: 20 2B           bra    $2B19
2AEE: 81 50           cmpa   #$50
2AF0: 26 02           bne    $2AF4
2AF2: 20 25           bra    $2B19
2AF4: 81 00           cmpa   #$00
2AF6: 26 05           bne    $2AFD
2AF8: BD 29 FC        jsr    $29FC
2AFB: 20 1C           bra    $2B19
2AFD: 81 80           cmpa   #$80
2AFF: 26 05           bne    $2B06
2B01: BD 28 BF        jsr    $28BF
2B04: 20 13           bra    $2B19
2B06: 81 E0           cmpa   #$E0
2B08: 27 03           beq    $2B0D
2B0A: 7E 2B 12        jmp    $2B12
2B0D: BD 29 07        jsr    $2907
2B10: 20 07           bra    $2B19
2B12: 81 D0           cmpa   #$D0
2B14: 26 03           bne    $2B19
2B16: BD 28 79        jsr    $2879
2B19: 32              pula
2B1A: 39              rts
2B1B: 36              psha
2B1C: 37              pshb
2B1D: 3C              pshx
2B1E: 18 3C           pshy
2B20: EC 07           ldd    $07,x
2B22: BD 2C 45        jsr    $2C45
2B25: BD 2D 20        jsr    $2D20
2B28: BD 2B 92        jsr    $2B92
2B2B: BD 27 9B        jsr    $279B
2B2E: 18 CE 0C 00     ldy    #$0C00
2B32: 86 FF           ldaa   #$FF
2B34: C6 80           ldab   #$80
2B36: 18 ED 03        std    $03,y
2B39: 4F              clra
2B3A: E6 06           ldab   $06,x
2B3C: C3 00 04        addd   #$0004
2B3F: 18 ED 05        std    $05,y
2B42: EC 07           ldd    $07,x
2B44: 18 ED 07        std    $07,y
2B47: A6 09           ldaa   $09,x
2B49: 18 A7 09        staa   $09,y
2B4C: E6 06           ldab   $06,x
2B4E: 37              pshb
2B4F: EC 08           ldd    $08,x
2B51: 84 3F           anda   #$3F
2B53: 8A 40           oraa   #$40
2B55: 8F              xgdx
2B56: BD 01 00        jsr    $0100
2B59: 33              pulb
2B5A: 37              pshb
2B5B: 4F              clra
2B5C: A6 00           ldaa   $00,x
2B5E: 18 A7 0A        staa   $0A,y
2B61: 01              nop
2B62: 01              nop
2B63: 01              nop
2B64: 01              nop
2B65: 01              nop
2B66: 01              nop
2B67: 01              nop
2B68: 01              nop
2B69: 01              nop
2B6A: 01              nop
2B6B: 08              inx
2B6C: 18 08           iny
2B6E: 5A              decb
2B6F: 26 EA           bne    $2B5B
2B71: BD 03 73        jsr    $0373
2B74: 18 CE 0C 00     ldy    #$0C00
2B78: 4F              clra
2B79: 33              pulb
2B7A: C3 00 07        addd   #$0007
2B7D: BD 03 2E        jsr    $032E
2B80: 18 38           puly
2B82: 38              pulx
2B83: 33              pulb
2B84: 32              pula
2B85: 39              rts
2B86: 3C              pshx
2B87: CE 10 00        ldx    #$1000
2B8A: 1D 6D 40        bclr   $6D,x #$40
2B8D: 1D 6A 40        bclr   $6A,x #$40
2B90: 38              pulx
2B91: 39              rts
2B92: 36              psha
2B93: B6 10 62        ldaa   $1062
2B96: 36              psha
2B97: 7F 10 62        clr    $1062
2B9A: 96 11           ldaa   *$11
2B9C: 81 BD           cmpa   #$BD
2B9E: 26 07           bne    $2BA7
2BA0: 86 FF           ldaa   #$FF
2BA2: B7 40 00        staa   $4000
2BA5: 20 14           bra    $2BBB
2BA7: 81 2A           cmpa   #$2A
2BA9: 26 07           bne    $2BB2
2BAB: 86 FF           ldaa   #$FF
2BAD: B7 40 00        staa   $4000
2BB0: 20 09           bra    $2BBB
2BB2: 81 A2           cmpa   #$A2
2BB4: 26 05           bne    $2BBB
2BB6: 86 FF           ldaa   #$FF
2BB8: B7 40 00        staa   $4000
2BBB: 4F              clra
2BBC: 8A 40           oraa   #$40
2BBE: B7 10 62        staa   $1062
2BC1: 96 13           ldaa   *$13
2BC3: 81 BD           cmpa   #$BD
2BC5: 26 07           bne    $2BCE
2BC7: 86 FF           ldaa   #$FF
2BC9: B7 40 00        staa   $4000
2BCC: 20 14           bra    $2BE2
2BCE: 81 2A           cmpa   #$2A
2BD0: 26 07           bne    $2BD9
2BD2: 86 FF           ldaa   #$FF
2BD4: B7 40 00        staa   $4000
2BD7: 20 09           bra    $2BE2
2BD9: 81 A2           cmpa   #$A2
2BDB: 26 05           bne    $2BE2
2BDD: 86 FF           ldaa   #$FF
2BDF: B7 40 00        staa   $4000
2BE2: 32              pula
2BE3: B7 10 62        staa   $1062
2BE6: 32              pula
2BE7: 39              rts
2BE8: BD 28 2E        jsr    $282E
2BEB: 39              rts
2BEC: 86 40           ldaa   #$40
2BEE: B7 10 25        staa   $1025      ; TFLG2
2BF1: 0E              cli
2BF2: 7A 00 1F        dec    $001F
2BF5: 26 1F           bne    $2C16
2BF7: 86 03           ldaa   #$03
2BF9: 97 1F           staa   *$1F
2BFB: BD 2B 86        jsr    $2B86
2BFE: BD 2C 8D        jsr    $2C8D
2C01: 07              tpa
2C02: BD 20 00        jsr    $2000
2C05: 06              tap
2C06: 25 0A           bcs    $2C12
2C08: 7A 00 1E        dec    $001E
2C0B: 26 03           bne    $2C10
2C0D: BD 01 43        jsr    $0143
2C10: 20 04           bra    $2C16
2C12: 86 04           ldaa   #$04
2C14: 97 1E           staa   *$1E
2C16: 3B              rti
2C17: 39              rts
2C18: 36              psha
2C19: 37              pshb
2C1A: B6 10 62        ldaa   $1062
2C1D: 36              psha
2C1E: 84 C0           anda   #$C0
2C20: 8A 01           oraa   #$01
2C22: B7 10 62        staa   $1062
2C25: C6 AA           ldab   #$AA
2C27: F7 55 55        stab   $5555
2C2A: 84 FE           anda   #$FE
2C2C: B7 10 62        staa   $1062
2C2F: C6 55           ldab   #$55
2C31: F7 6A AA        stab   $6AAA
2C34: 8A 01           oraa   #$01
2C36: B7 10 62        staa   $1062
2C39: 32              pula
2C3A: 33              pulb
2C3B: F7 55 55        stab   $5555
2C3E: B7 10 62        staa   $1062
2C41: 32              pula
2C42: 39              rts
2C43: 39              rts
2C44: 39              rts
2C45: 36              psha
2C46: 37              pshb
2C47: 05              asld
2C48: 05              asld
2C49: 84 3F           anda   #$3F
2C4B: 91 1C           cmpa   *$1C
2C4D: 23 0C           bls    $2C5B
2C4F: 90 1C           suba   *$1C
2C51: 4A              deca
2C52: 91 1D           cmpa   *$1D
2C54: 23 03           bls    $2C59
2C56: 14 0C 80        bset   $0C #$80
2C59: 8A 40           oraa   #$40
2C5B: B7 10 62        staa   $1062
2C5E: 33              pulb
2C5F: 32              pula
2C60: 39              rts
2C61: 36              psha
2C62: 37              pshb
2C63: 18 3C           pshy
2C65: 18 CE 0C 00     ldy    #$0C00
2C69: 86 F5           ldaa   #$F5
2C6B: C6 83           ldab   #$83
2C6D: 18 ED 03        std    $03,y
2C70: 4F              clra
2C71: 18 A7 05        staa   $05,y
2C74: 96 10           ldaa   *$10
2C76: D6 11           ldab   *$11
2C78: 18 ED 06        std    $06,y
2C7B: 96 12           ldaa   *$12
2C7D: D6 13           ldab   *$13
2C7F: 18 ED 08        std    $08,y
2C82: CC 00 06        ldd    #$0006
2C85: BD 03 2E        jsr    $032E
2C88: 18 38           puly
2C8A: 33              pulb
2C8B: 32              pula
2C8C: 39              rts
2C8D: 36              psha
2C8E: B6 10 00        ldaa   $1000
2C91: 85 02           bita   #$02
2C93: 27 03           beq    $2C98
2C95: 0D              sec
2C96: 20 01           bra    $2C99
2C98: 0D              sec
2C99: 32              pula
2C9A: 39              rts
2C9B: 81 01           cmpa   #$01
2C9D: 26 07           bne    $2CA6
2C9F: C1 2A           cmpb   #$2A
2CA1: 26 03           bne    $2CA6
2CA3: 0C              clc
2CA4: 20 13           bra    $2CB9
2CA6: 81 89           cmpa   #$89
2CA8: 26 0E           bne    $2CB8
2CAA: C1 BD           cmpb   #$BD
2CAC: 27 04           beq    $2CB2
2CAE: C1 A2           cmpb   #$A2
2CB0: 26 03           bne    $2CB5
2CB2: 0C              clc
2CB3: 20 01           bra    $2CB6
2CB5: 0D              sec
2CB6: 20 01           bra    $2CB9
2CB8: 0D              sec
2CB9: 39              rts
2CBA: 81 1F           cmpa   #$1F
2CBC: 26 18           bne    $2CD6
2CBE: C1 D5           cmpb   #$D5
2CC0: 26 03           bne    $2CC5
2CC2: 0C              clc
2CC3: 20 0F           bra    $2CD4
2CC5: C1 DA           cmpb   #$DA
2CC7: 26 03           bne    $2CCC
2CC9: 0C              clc
2CCA: 20 08           bra    $2CD4
2CCC: C1 5B           cmpb   #$5B
2CCE: 26 03           bne    $2CD3
2CD0: 0C              clc
2CD1: 20 01           bra    $2CD4
2CD3: 0D              sec
2CD4: 20 01           bra    $2CD7
2CD6: 0D              sec
2CD7: 39              rts
2CD8: 36              psha
2CD9: 96 1C           ldaa   *$1C
2CDB: 81 0F           cmpa   #$0F
2CDD: 23 03           bls    $2CE2
2CDF: 0D              sec
2CE0: 20 0A           bra    $2CEC
2CE2: 96 1D           ldaa   *$1D
2CE4: 81 00           cmpa   #$00
2CE6: 23 03           bls    $2CEB
2CE8: 0D              sec
2CE9: 20 01           bra    $2CEC
2CEB: 0C              clc
2CEC: 32              pula
2CED: 39              rts
2CEE: 3C              pshx
2CEF: CE 10 00        ldx    #$1000
2CF2: 1C 5C 01        bset   $5C,x #$01
2CF5: 38              pulx
2CF6: 39              rts
2CF7: 3C              pshx
2CF8: CE 10 00        ldx    #$1000
2CFB: 1D 5C 01        bclr   $5C,x #$01
2CFE: 38              pulx
2CFF: 39              rts
2D00: 36              psha
2D01: 3C              pshx
2D02: 07              tpa
2D03: 0F              sei
2D04: CE 10 00        ldx    #$1000
2D07: 1D 60 20        bclr   $60,x #$20
2D0A: 1D 60 10        bclr   $60,x #$10
2D0D: 1C 60 20        bset   $60,x #$20
2D10: BD 00 A5        jsr    $00A5
2D13: 06              tap
2D14: 38              pulx
2D15: 32              pula
2D16: 39              rts
2D17: 3C              pshx
2D18: CE 10 00        ldx    #$1000
2D1B: 1D 35 0F        bclr   $35,x #$0F
2D1E: 38              pulx
2D1F: 39              rts
2D20: 3C              pshx
2D21: CE 10 00        ldx    #$1000
2D24: 1D 60 10        bclr   $60,x #$10
2D27: 1D 60 20        bclr   $60,x #$20
2D2A: 1C 60 10        bset   $60,x #$10
2D2D: 38              pulx
2D2E: BD 00 A5        jsr    $00A5
2D31: 39              rts
2D32: 06              tap
2D33: 40              nega
2D34: 20 58           bra    $2D8E
2D36: 58              aslb
2D37: 58              aslb
2D38: 58              aslb
2D39: 58              aslb
2D3A: 58              aslb
2D3B: 58              aslb
2D3C: 58              aslb
2D3D: 58              aslb
2D3E: 58              aslb
2D3F: 58              aslb
2D40: 58              aslb
2D41: 58              aslb
2D42: 58              aslb
2D43: 58              aslb
2D44: 58              aslb
2D45: 58              aslb
2D46: 58              aslb
2D47: 58              aslb
2D48: 58              aslb
2D49: 58              aslb
2D4A: 58              aslb
2D4B: 58              aslb
2D4C: 58              aslb
2D4D: 20 4A           bra    $2D99
2D4F: 65              .byte $65
2D50: 64 69           lsr    $69,x
2D52: 2D 43           blt    $2D97
2D54: 6F 6D           clr    $6D,x
2D56: 6C 69           inc    $69,x
2D58: 6E 6B           jmp    $6B,x
2D5A: 20 00           bra    $2D5C
2D5C: 4F              clra
2D5D: 44              lsra
2D5E: 48              asla
2D5F: 4F              clra
2D60: 7E 2D 5C        jmp    $2D5C
2D63: 86 70           ldaa   #$70
2D65: B7 40 00        staa   $4000
2D68: BD 01 00        jsr    $0100
2D6B: B6 40 00        ldaa   $4000
2D6E: BD 03 73        jsr    $0373
2D71: 4D              tsta
2D72: 2A EF           bpl    $2D63
2D74: 84 78           anda   #$78
2D76: 39              rts
2D77: 36              psha
2D78: 37              pshb
2D79: 3C              pshx
2D7A: 18 3C           pshy
2D7C: EC 07           ldd    $07,x
2D7E: 84 3F           anda   #$3F
2D80: 8A 40           oraa   #$40
2D82: 8F              xgdx
2D83: 18 8F           xgdy
2D85: 18 EC 04        ldd    $04,y
2D88: 83 00 03        subd   #$0003
2D8B: 37              pshb
2D8C: 04              lsrd
2D8D: 04              lsrd
2D8E: 04              lsrd
2D8F: 04              lsrd
2D90: 04              lsrd
2D91: 04              lsrd
2D92: 04              lsrd
2D93: 5D              tstb
2D94: 27 0B           beq    $2DA1
2D96: 37              pshb
2D97: CC 00 80        ldd    #$0080
2D9A: BD 2E 89        jsr    $2E89
2D9D: 33              pulb
2D9E: 5A              decb
2D9F: 20 F3           bra    $2D94
2DA1: 4F              clra
2DA2: 33              pulb
2DA3: C4 7F           andb   #$7F
2DA5: 27 03           beq    $2DAA
2DA7: BD 2E 89        jsr    $2E89
2DAA: 18 38           puly
2DAC: 38              pulx
2DAD: 33              pulb
2DAE: 32              pula
2DAF: 39              rts
2DB0: 36              psha
2DB1: 37              pshb
2DB2: 3C              pshx
2DB3: 18 3C           pshy
2DB5: EC 07           ldd    $07,x
2DB7: 84 3F           anda   #$3F
2DB9: 8A 40           oraa   #$40
2DBB: 8F              xgdx
2DBC: 18 8F           xgdy
2DBE: 18 EC 04        ldd    $04,y
2DC1: 83 00 03        subd   #$0003
2DC4: 36              psha
2DC5: 37              pshb
2DC6: 18 A6 09        ldaa   $09,y
2DC9: BD 27 EA        jsr    $27EA
2DCC: 8C 7F FF        cpx    #$7FFF
2DCF: 26 07           bne    $2DD8
2DD1: 8F              xgdx
2DD2: BD 27 55        jsr    $2755
2DD5: 84 3F           anda   #$3F
2DD7: 8F              xgdx
2DD8: 08              inx
2DD9: 18 08           iny
2DDB: 33              pulb
2DDC: 32              pula
2DDD: 83 00 01        subd   #$0001
2DE0: 26 E2           bne    $2DC4
2DE2: 18 38           puly
2DE4: 38              pulx
2DE5: 33              pulb
2DE6: 32              pula
2DE7: 39              rts
2DE8: 36              psha
2DE9: 37              pshb
2DEA: 3C              pshx
2DEB: 18 3C           pshy
2DED: EC 07           ldd    $07,x
2DEF: 84 3F           anda   #$3F
2DF1: 8A 40           oraa   #$40
2DF3: 8F              xgdx
2DF4: 18 8F           xgdy
2DF6: 18 EC 04        ldd    $04,y
2DF9: 83 00 03        subd   #$0003
2DFC: 37              pshb
2DFD: 4D              tsta
2DFE: 27 0B           beq    $2E0B
2E00: 36              psha
2E01: CC 01 00        ldd    #$0100
2E04: BD 2E 89        jsr    $2E89
2E07: 32              pula
2E08: 4A              deca
2E09: 20 F3           bra    $2DFE
2E0B: 4F              clra
2E0C: 33              pulb
2E0D: 5D              tstb
2E0E: 27 03           beq    $2E13
2E10: BD 2E 89        jsr    $2E89
2E13: 18 38           puly
2E15: 38              pulx
2E16: 33              pulb
2E17: 32              pula
2E18: 39              rts
2E19: 36              psha
2E1A: 37              pshb
2E1B: 3C              pshx
2E1C: 18 3C           pshy
2E1E: EC 07           ldd    $07,x
2E20: 84 3F           anda   #$3F
2E22: 8A 40           oraa   #$40
2E24: 8F              xgdx
2E25: 18 8F           xgdy
2E27: 18 EC 04        ldd    $04,y
2E2A: 83 00 03        subd   #$0003
2E2D: 36              psha
2E2E: 37              pshb
2E2F: 04              lsrd
2E30: 4D              tsta
2E31: 27 0B           beq    $2E3E
2E33: 36              psha
2E34: CC 02 00        ldd    #$0200
2E37: BD 2E 89        jsr    $2E89
2E3A: 32              pula
2E3B: 4A              deca
2E3C: 20 F3           bra    $2E31
2E3E: 33              pulb
2E3F: 32              pula
2E40: 84 01           anda   #$01
2E42: 26 04           bne    $2E48
2E44: C1 00           cmpb   #$00
2E46: 27 03           beq    $2E4B
2E48: BD 2E 89        jsr    $2E89
2E4B: 18 38           puly
2E4D: 38              pulx
2E4E: 33              pulb
2E4F: 32              pula
2E50: 39              rts
2E51: 36              psha
2E52: 37              pshb
2E53: 3C              pshx
2E54: 18 3C           pshy
2E56: EC 07           ldd    $07,x
2E58: 84 3F           anda   #$3F
2E5A: 8A 40           oraa   #$40
2E5C: 8F              xgdx
2E5D: 18 8F           xgdy
2E5F: 18 EC 04        ldd    $04,y
2E62: 83 00 03        subd   #$0003
2E65: 36              psha
2E66: 37              pshb
2E67: 18 A6 09        ldaa   $09,y
2E6A: BD 28 18        jsr    $2818
2E6D: 8C 7F FF        cpx    #$7FFF
2E70: 26 07           bne    $2E79
2E72: 8F              xgdx
2E73: BD 27 55        jsr    $2755
2E76: 84 3F           anda   #$3F
2E78: 8F              xgdx
2E79: 08              inx
2E7A: 18 08           iny
2E7C: 33              pulb
2E7D: 32              pula
2E7E: 83 00 01        subd   #$0001
2E81: 26 E2           bne    $2E65
2E83: 18 38           puly
2E85: 38              pulx
2E86: 33              pulb
2E87: 32              pula
2E88: 39              rts
2E89: 36              psha
2E8A: 37              pshb
2E8B: 3C              pshx
2E8C: 18 3C           pshy
2E8E: BD 27 B1        jsr    $27B1
2E91: 36              psha
2E92: 18 A6 09        ldaa   $09,y
2E95: A7 00           staa   $00,x
2E97: 08              inx
2E98: 18 08           iny
2E9A: 32              pula
2E9B: 83 00 01        subd   #$0001
2E9E: 26 F1           bne    $2E91
2EA0: C6 14           ldab   #$14
2EA2: BD 01 18        jsr    $0118
2EA5: 18 38           puly
2EA7: 38              pulx
2EA8: 33              pulb
2EA9: 32              pula
2EAA: BD 01 00        jsr    $0100
2EAD: 36              psha
2EAE: 18 A6 09        ldaa   $09,y
2EB1: A1 00           cmpa   $00,x
2EB3: 27 03           beq    $2EB8
2EB5: 14 0C 80        bset   $0C #$80
2EB8: 08              inx
2EB9: 18 08           iny
2EBB: 32              pula
2EBC: 83 00 01        subd   #$0001
2EBF: 26 EC           bne    $2EAD
2EC1: BD 03 73        jsr    $0373
2EC4: 39              rts
2EC5: 39              rts
2EC6: 3C              pshx
2EC7: EC 07           ldd    $07,x
2EC9: 1A 83 02 00     cpd    #$0200
2ECD: 24 11           bcc    $2EE0
2ECF: 18 8F           xgdy
2ED1: EC 04           ldd    $04,x
2ED3: 83 00 03        subd   #$0003
2ED6: 37              pshb
2ED7: C6 09           ldab   #$09
2ED9: 3A              abx
2EDA: 33              pulb
2EDB: BD 30 5C        jsr    $305C
2EDE: 20 12           bra    $2EF2
2EE0: 83 02 00        subd   #$0200
2EE3: 18 8F           xgdy
2EE5: EC 04           ldd    $04,x
2EE7: 83 00 03        subd   #$0003
2EEA: 37              pshb
2EEB: C6 09           ldab   #$09
2EED: 3A              abx
2EEE: 33              pulb
2EEF: BD 2F A3        jsr    $2FA3
2EF2: 38              pulx
2EF3: 39              rts
2EF4: 36              psha
2EF5: 37              pshb
2EF6: BD 2D 20        jsr    $2D20
2EF9: EC 06           ldd    $06,x
2EFB: BD 2C 45        jsr    $2C45
2EFE: D6 0C           ldab   *$0C
2F00: C5 80           bitb   #$80
2F02: 26 46           bne    $2F4A
2F04: B6 10 62        ldaa   $1062
2F07: 85 40           bita   #$40
2F09: 26 04           bne    $2F0F
2F0B: 96 11           ldaa   *$11
2F0D: 20 02           bra    $2F11
2F0F: 96 13           ldaa   *$13
2F11: 81 BD           cmpa   #$BD
2F13: 26 05           bne    $2F1A
2F15: BD 2D B0        jsr    $2DB0
2F18: 20 30           bra    $2F4A
2F1A: 81 2A           cmpa   #$2A
2F1C: 26 05           bne    $2F23
2F1E: BD 2D B0        jsr    $2DB0
2F21: 20 27           bra    $2F4A
2F23: 81 A2           cmpa   #$A2
2F25: 26 05           bne    $2F2C
2F27: BD 2E 51        jsr    $2E51
2F2A: 20 1E           bra    $2F4A
2F2C: 81 D5           cmpa   #$D5
2F2E: 26 05           bne    $2F35
2F30: BD 2D 77        jsr    $2D77
2F33: 20 15           bra    $2F4A
2F35: 81 DA           cmpa   #$DA
2F37: 26 05           bne    $2F3E
2F39: BD 2D E8        jsr    $2DE8
2F3C: 20 0C           bra    $2F4A
2F3E: 81 5B           cmpa   #$5B
2F40: 26 05           bne    $2F47
2F42: BD 2E 19        jsr    $2E19
2F45: 20 03           bra    $2F4A
2F47: 14 0C 80        bset   $0C #$80
2F4A: BD 27 9B        jsr    $279B
2F4D: BD 2B 92        jsr    $2B92
2F50: 33              pulb
2F51: 32              pula
2F52: 39              rts
2F53: 36              psha
2F54: 18 3C           pshy
2F56: 3C              pshx
2F57: A6 06           ldaa   $06,x
2F59: 1A EE 07        ldy    $07,x
2F5C: 36              psha
2F5D: 18 3C           pshy
2F5F: BD 25 D7        jsr    $25D7
2F62: BD 2E C6        jsr    $2EC6
2F65: 96 0C           ldaa   *$0C
2F67: 85 80           bita   #$80
2F69: 26 06           bne    $2F71
2F6B: BD 25 E3        jsr    $25E3
2F6E: BD 2E F4        jsr    $2EF4
2F71: 18 38           puly
2F73: 32              pula
2F74: 38              pulx
2F75: 1A EF 07        sty    $07,x
2F78: A7 06           staa   $06,x
2F7A: 18 38           puly
2F7C: 32              pula
2F7D: 39              rts
2F7E: 36              psha
2F7F: 37              pshb
2F80: 3C              pshx
2F81: 18 3C           pshy
2F83: 36              psha
2F84: CC 00 01        ldd    #$0001
2F87: 18 30           tsy
2F89: 18 8F           xgdy
2F8B: 8F              xgdx
2F8C: 18 8F           xgdy
2F8E: 18 8C 02 00     cpy    #$0200
2F92: 24 05           bcc    $2F99
2F94: BD 30 5C        jsr    $305C
2F97: 20 03           bra    $2F9C
2F99: BD 2F A3        jsr    $2FA3
2F9C: 32              pula
2F9D: 18 38           puly
2F9F: 38              pulx
2FA0: 33              pulb
2FA1: 32              pula
2FA2: 39              rts
2FA3: 18 8F           xgdy
2FA5: 37              pshb
2FA6: 16              tab
2FA7: 4F              clra
2FA8: 05              asld
2FA9: 05              asld
2FAA: 8A 80           oraa   #$80
2FAC: B7 10 62        staa   $1062
2FAF: 54              lsrb
2FB0: 54              lsrb
2FB1: 17              tba
2FB2: 33              pulb
2FB3: 8A 40           oraa   #$40
2FB5: C5 3F           bitb   #$3F
2FB7: 18 8F           xgdy
2FB9: 26 44           bne    $2FFF
2FBB: 1A 83 00 40     cpd    #$0040
2FBF: 26 3E           bne    $2FFF
2FC1: 37              pshb
2FC2: 0F              sei
2FC3: BD 2D 00        jsr    $2D00
2FC6: BD 2C EE        jsr    $2CEE
2FC9: A6 00           ldaa   $00,x
2FCB: 18 A7 00        staa   $00,y
2FCE: 08              inx
2FCF: 18 08           iny
2FD1: 5A              decb
2FD2: 26 F5           bne    $2FC9
2FD4: BD 2C F7        jsr    $2CF7
2FD7: BD 27 89        jsr    $2789
2FDA: 0E              cli
2FDB: C6 0A           ldab   #$0A
2FDD: BD 01 18        jsr    $0118
2FE0: 33              pulb
2FE1: BD 01 00        jsr    $0100
2FE4: BD 2C EE        jsr    $2CEE
2FE7: 09              dex
2FE8: 18 09           dey
2FEA: 18 A6 00        ldaa   $00,y
2FED: A1 00           cmpa   $00,x
2FEF: 27 03           beq    $2FF4
2FF1: 14 0C 80        bset   $0C #$80
2FF4: 5A              decb
2FF5: 26 F0           bne    $2FE7
2FF7: BD 2C F7        jsr    $2CF7
2FFA: BD 03 73        jsr    $0373
2FFD: 20 5C           bra    $305B
2FFF: 36              psha
3000: 37              pshb
3001: BD 01 00        jsr    $0100
3004: BD 2C EE        jsr    $2CEE
3007: 18 A6 00        ldaa   $00,y
300A: BD 2C F7        jsr    $2CF7
300D: BD 03 73        jsr    $0373
3010: A1 00           cmpa   $00,x
3012: 27 18           beq    $302C
3014: A6 00           ldaa   $00,x
3016: 0F              sei
3017: BD 2D 00        jsr    $2D00
301A: BD 2C EE        jsr    $2CEE
301D: 18 A7 00        staa   $00,y
3020: BD 2C F7        jsr    $2CF7
3023: BD 27 89        jsr    $2789
3026: 0E              cli
3027: C6 0A           ldab   #$0A
3029: BD 01 18        jsr    $0118
302C: BD 01 00        jsr    $0100
302F: BD 2C EE        jsr    $2CEE
3032: 18 A6 00        ldaa   $00,y
3035: BD 2C F7        jsr    $2CF7
3038: BD 03 73        jsr    $0373
303B: A1 00           cmpa   $00,x
303D: 27 03           beq    $3042
303F: 14 0C 80        bset   $0C #$80
3042: 18 8F           xgdy
3044: 1A 83 7F FF     cpd    #$7FFF
3048: 26 05           bne    $304F
304A: 7C 10 62        inc    $1062
304D: 84 3F           anda   #$3F
304F: 18 8F           xgdy
3051: 08              inx
3052: 18 08           iny
3054: 33              pulb
3055: 32              pula
3056: 83 00 01        subd   #$0001
3059: 26 A4           bne    $2FFF
305B: 39              rts
305C: 18 8F           xgdy
305E: C3 FE 00        addd   #$FE00
3061: 18 8F           xgdy
3063: 36              psha
3064: A6 00           ldaa   $00,x
3066: 18 A1 00        cmpa   $00,y
3069: 27 0C           beq    $3077
306B: BD 2D 17        jsr    $2D17
306E: BD 23 EB        jsr    $23EB
3071: BD 27 B9        jsr    $27B9
3074: BD 27 92        jsr    $2792
3077: 08              inx
3078: 18 08           iny
307A: 32              pula
307B: 83 00 01        subd   #$0001
307E: 26 E3           bne    $3063
3080: 39              rts
3081: 36              psha
3082: 3C              pshx
3083: B6 10 62        ldaa   $1062
3086: 84 C0           anda   #$C0
3088: B7 10 62        staa   $1062
308B: CE 40 00        ldx    #$4000
308E: 4F              clra
308F: BD 27 EA        jsr    $27EA
3092: 08              inx
3093: 8C 80 00        cpx    #$8000
3096: 26 F6           bne    $308E
3098: 7C 10 62        inc    $1062
309B: B6 10 62        ldaa   $1062
309E: 84 3F           anda   #$3F
30A0: 81 10           cmpa   #$10
30A2: 26 E7           bne    $308B
30A4: 38              pulx
30A5: 32              pula
30A6: 39              rts
30A7: FF FF FF        stx    $FFFF
30AA: FF FF FF        stx    $FFFF
30AD: FF FF FF        stx    $FFFF
30B0: FF FF FF        stx    $FFFF
30B3: FF FF FF        stx    $FFFF
30B6: FF FF FF        stx    $FFFF
30B9: FF FF FF        stx    $FFFF
30BC: FF FF FF        stx    $FFFF
30BF: FF FF FF        stx    $FFFF
30C2: FF FF FF        stx    $FFFF
30C5: FF FF FF        stx    $FFFF
30C8: FF FF FF        stx    $FFFF
30CB: FF FF FF        stx    $FFFF
30CE: FF FF FF        stx    $FFFF
30D1: FF FF FF        stx    $FFFF
30D4: FF FF FF        stx    $FFFF
30D7: FF FF FF        stx    $FFFF
30DA: FF FF FF        stx    $FFFF
30DD: FF FF FF        stx    $FFFF
30E0: FF FF FF        stx    $FFFF
30E3: FF FF FF        stx    $FFFF
30E6: FF FF FF        stx    $FFFF
30E9: FF FF FF        stx    $FFFF
30EC: FF FF FF        stx    $FFFF
30EF: FF FF FF        stx    $FFFF
30F2: FF FF FF        stx    $FFFF
30F5: FF FF FF        stx    $FFFF
30F8: FF FF FF        stx    $FFFF
30FB: FF FF FF        stx    $FFFF
30FE: FF FF FF        stx    $FFFF
3101: FF FF FF        stx    $FFFF
3104: FF FF FF        stx    $FFFF
3107: FF FF FF        stx    $FFFF
310A: FF FF FF        stx    $FFFF
310D: FF FF FF        stx    $FFFF
3110: FF FF FF        stx    $FFFF
3113: FF FF FF        stx    $FFFF
3116: FF FF FF        stx    $FFFF
3119: FF FF FF        stx    $FFFF
311C: FF FF FF        stx    $FFFF
311F: FF FF FF        stx    $FFFF
3122: FF FF FF        stx    $FFFF
3125: FF FF FF        stx    $FFFF
3128: FF FF FF        stx    $FFFF
312B: FF FF FF        stx    $FFFF
312E: FF FF FF        stx    $FFFF
3131: FF FF FF        stx    $FFFF
3134: FF FF FF        stx    $FFFF
3137: FF FF FF        stx    $FFFF
313A: FF FF FF        stx    $FFFF
313D: FF FF FF        stx    $FFFF
3140: FF FF FF        stx    $FFFF
3143: FF FF FF        stx    $FFFF
3146: FF FF FF        stx    $FFFF
3149: FF FF FF        stx    $FFFF
314C: FF FF FF        stx    $FFFF
314F: FF FF FF        stx    $FFFF
3152: FF FF FF        stx    $FFFF
3155: FF FF FF        stx    $FFFF
3158: FF FF FF        stx    $FFFF
315B: FF FF FF        stx    $FFFF
315E: FF FF FF        stx    $FFFF
3161: FF FF FF        stx    $FFFF
3164: FF FF FF        stx    $FFFF
3167: FF FF FF        stx    $FFFF
316A: FF FF FF        stx    $FFFF
316D: FF FF FF        stx    $FFFF
3170: FF FF FF        stx    $FFFF
3173: FF FF FF        stx    $FFFF
3176: FF FF FF        stx    $FFFF
3179: FF FF FF        stx    $FFFF
317C: FF FF FF        stx    $FFFF
317F: FF FF FF        stx    $FFFF
3182: FF FF FF        stx    $FFFF
3185: FF FF FF        stx    $FFFF
3188: FF FF FF        stx    $FFFF
318B: FF FF FF        stx    $FFFF
318E: FF FF FF        stx    $FFFF
3191: FF FF FF        stx    $FFFF
3194: FF FF FF        stx    $FFFF
3197: FF FF FF        stx    $FFFF
319A: FF FF FF        stx    $FFFF
319D: FF FF FF        stx    $FFFF
31A0: FF FF FF        stx    $FFFF
31A3: FF FF FF        stx    $FFFF
31A6: FF FF FF        stx    $FFFF
31A9: FF FF FF        stx    $FFFF
31AC: FF FF FF        stx    $FFFF
31AF: FF FF FF        stx    $FFFF
31B2: FF FF FF        stx    $FFFF
31B5: FF FF FF        stx    $FFFF
31B8: FF FF FF        stx    $FFFF
31BB: FF FF FF        stx    $FFFF
31BE: FF FF FF        stx    $FFFF
31C1: FF FF FF        stx    $FFFF
31C4: FF FF FF        stx    $FFFF
31C7: FF FF FF        stx    $FFFF
31CA: FF FF FF        stx    $FFFF
31CD: FF FF FF        stx    $FFFF
31D0: FF FF FF        stx    $FFFF
31D3: FF FF FF        stx    $FFFF
31D6: FF FF FF        stx    $FFFF
31D9: FF FF FF        stx    $FFFF
31DC: FF FF FF        stx    $FFFF
31DF: FF FF FF        stx    $FFFF
31E2: FF FF FF        stx    $FFFF
31E5: FF FF FF        stx    $FFFF
31E8: FF FF FF        stx    $FFFF
31EB: FF FF FF        stx    $FFFF
31EE: FF FF FF        stx    $FFFF
31F1: FF FF FF        stx    $FFFF
31F4: FF FF FF        stx    $FFFF
31F7: FF FF FF        stx    $FFFF
31FA: FF FF FF        stx    $FFFF
31FD: FF FF FF        stx    $FFFF
```
