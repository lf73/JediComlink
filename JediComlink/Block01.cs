using System;
using System.Text;

namespace JediComlink
{
    public class Block01 : Block
    {
        public override int Id { get => 0x01; }
        public override string Description { get => "Internal Radio"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 02 00 34 36  36 41 57 41   32 38 36 37  48 30 31 55
        1: 43 44 36 50  57 31 42 4E   00 00 00 00  00 0F 02 00
        2: 00 01 00 00  00 3D 01 99   00 00 00 00  01 DC 01 00
        3: 03 22 BB C8  89 54 02 69   30 B0 
        */

        private const int EXTERNAL_CODEPLUG_VECTOR = 0x00; //01
        private const int SERIAL = 0x02; //03 04 05 06 07 08 09 0A 0B
        private const int MODEL = 0x0C; //0D 0E 0F 10 11 12 13 14 15 16 17 18 19 1A 1B
        private const int UNKNOWN1 = 0x1C; //1D
        private const int INTERNAL_CODEPLUG_SIZE = 0x1E; //1F
        private const int UNKNOWN2 = 0x20; //21 22 23
        private const int BLOCK_02_VECTOR = 0x24; //25
        private const int BLOCK_56_VECTOR = 0x26; //27
        private const int UNKNOWN3 = 0x28; //29 2A 2B
        private const int BLOCK_10_VECTOR = 0x2C; //2D
        private const int UNKNOWN4 = 0x2e; //2F
        private const int AUTH_CODE = 0x30; //31 32 33 34 35 36 37 38 39
        #endregion

        #region Propeties
        public int ExternalCodeplugVector
    {
            get => Contents[EXTERNAL_CODEPLUG_VECTOR] * 0x100 + Contents[EXTERNAL_CODEPLUG_VECTOR + 1];
            set
            {
                if (value < 0 || value > 0xFFFF) throw new ArgumentException("Out of range 0x0000 to 0xFFFF");
                Contents[EXTERNAL_CODEPLUG_VECTOR] = (byte)(value / 0x100);
                Contents[EXTERNAL_CODEPLUG_VECTOR + 1] = (byte)(value % 0x100);
            }
        }

        public string Serial
        {
            get => GetStringContents(SERIAL, 10);
            //set => XYZ = value; //TODO
        }

        public string Model
        {
            get => GetStringContents(MODEL, 16);
            //set => XYZ = value; //TODO
        }
        
        public byte[] Unknown1 //TODO  Maybe code plug version?
        {
            get => Contents.Slice(UNKNOWN1, 2).ToArray();
            //set => XYZ = value; //TODO
        }
        public int InternalCodeplugSize
        {
            get => Contents[INTERNAL_CODEPLUG_SIZE] * 0x100 + Contents[INTERNAL_CODEPLUG_SIZE + 1];
            set
            {
                if (value < 0 || value > 0xFFFF) throw new ArgumentException("Out of range 0x0000 to 0xFFFF");
                Contents[INTERNAL_CODEPLUG_SIZE] = (byte)(value / 0x100);
                Contents[INTERNAL_CODEPLUG_SIZE + 1] = (byte)(value % 0x100);
            }
        }

        public byte[] Unknown2
        {
            get => Contents.Slice(UNKNOWN2, 4).ToArray();
            //set => XYZ = value; //TODO
        }

        public Block02 Block02 { get; set; }
        
        public Block56 Block56 { get; set; }
        public byte[] Unknown3
        {
            get => Contents.Slice(UNKNOWN3, 4).ToArray();
            //set => XYZ = value; //TODO
        }

        public Block10 Block10 { get; set; }

        public byte[] Unknown4
        {
            get => Contents.Slice(UNKNOWN4, 2).ToArray();
            //set => XYZ = value; //TODO
        }

        public byte[] AuthCode
        {
            get => Contents.Slice(AUTH_CODE, 10).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

 

        public Block01(Codeplug codeplug, byte[] codeplugContents)
        {
            Codeplug = codeplug;
            Level = 1;

            StartAddress = 0x0000;
            var length = codeplugContents[StartAddress];
            Contents = codeplugContents.AsSpan().Slice(StartAddress + 2, length - 1);

            Block02 = new Block02(this, BLOCK_02_VECTOR, codeplugContents);
            Block56 = new Block56(this, BLOCK_56_VECTOR, codeplugContents);
            Block10 = new Block10(this, BLOCK_10_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine($"External Codeplug Vector: {ExternalCodeplugVector:X4}");
            sb.AppendLine($"Serial: {Serial}");
            sb.AppendLine($"Model: {Model}");
            sb.AppendLine($"Unknown1 Bytes: {FormatHex(Unknown1)}");
            sb.AppendLine($"Internal Codeplug Size: {InternalCodeplugSize}");
            sb.AppendLine($"Unknown2 Bytes: {FormatHex(Unknown2)}");
            sb.AppendLine($"Block 02 Vector: {Block02?.StartAddress:X4}");
            sb.AppendLine($"Block 56 Vector: {Block56?.StartAddress:X4}");
            sb.AppendLine($"Unknown3 Bytes: {FormatHex(Unknown3)}");
            sb.AppendLine($"Block 10 Vector: {Block10?.StartAddress:X4}");
            sb.AppendLine($"Unknown4 Bytes: {FormatHex(Unknown4)}");
            sb.AppendLine($"Auth Code: {FormatHex(AuthCode)}");

            sb.AppendLine(Block02.ToString());
            sb.AppendLine(Block56.ToString());
            sb.AppendLine(Block10.ToString());
            return sb.ToString();
        }
    }
}
