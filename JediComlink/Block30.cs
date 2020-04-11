using System;
using System.Text;

namespace JediComlink
{
    public class Block30 : Block
    {
        public override int Id { get => 0x30; }
        public override string Description { get => "External Radio"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 3E 00 34 33  32 43 44 4E   30 30 30 32  48 30 31 4B
        1: 44 44 39 50  57 31 42 4E   00 00 00 00  11 02 24 14
        2: 42 03 00 1C  07 71 02 D1   03 0A 04 B7  05 1F 06 65
        3: 06 74 06 E4  07 16 08 36   08 3E 08 82  09 82 09 9B
        4: 00 00 00 00  00 00 00 00   00 00 09 AB  00 00
        */

        private const int UNKNOWN1 = 0x00; //01
        private const int SERIAL = 0x02; //03 04 05 06 07 08 09 0A 0B
        private const int MODEL = 0x0C; //0D 0E 0F 10 11 12 13 14 15 16 17 18 19 1A 1B
        private const int TIMESTAMP = 0x1c; //1D 1E 1F 20
        private const int UNKNOWN2 = 0x21; //22 23
        private const int EXTERNAL_CODEPLUG_SIZE = 0x24; //25
        private const int BLOCK_31_VECTOR = 0x26; //27
        private const int BLOCK_3D_VECTOR = 0x28; //29
        private const int BLOCK_36_VECTOR = 0x2A; //2B
        private const int BLOCK_55_VECTOR = 0x2C; //2D
        private const int BLOCK_54_VECTOR = 0x2E; //2F
        private const int BLOCK_51_VECTOR = 0x30; //31
        private const int UNKNOWN3 = 0x32; //33
        private const int BLOCK_39_VECTOR = 0x34; //35
        private const int BLOCK_3B_VECTOR = 0x36; //37
        private const int BLOCK_34_VECTOR = 0x38; //39
        private const int BLOCK_35_VECTOR = 0x3A; //3B
        private const int BLOCK_3C_VECTOR = 0x3C; //3D
        private const int BLOCK_73_VECTOR = 0x3E; //3F
        private const int UNKNOWN4 = 0x40; //41 42 43 44 45 46 47 48 49 4A 4B
        private const int UNKNOWN5 = 0x4C; //4D
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 2).ToArray();
            //set => XYZ = value; //TODO
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

        public DateTime TimeStamp
        {
            get => new DateTime(2000 + GetDigits(Contents[TIMESTAMP]),
                                    GetDigits(Contents[TIMESTAMP + 1]),
                                    GetDigits(Contents[TIMESTAMP + 2]),
                                    GetDigits(Contents[TIMESTAMP + 3]),
                                    GetDigits(Contents[TIMESTAMP + 4]),
                                    0);
            //set => XYZ = value; //TODO
        }

        public byte[] Unknown2
        {
            get => Contents.Slice(UNKNOWN2, 3).ToArray();
            //set => XYZ = value; //TODO
        }
        public int ExternalCodeplugSize
        {
            get => Contents[EXTERNAL_CODEPLUG_SIZE] * 0x100 + Contents[EXTERNAL_CODEPLUG_SIZE + 1];
            set
            {
                if (value < 0 || value > 0xFFFF) throw new ArgumentException("Out of range 0x0000 to 0xFFFF");
                Contents[EXTERNAL_CODEPLUG_SIZE] = (byte)(value / 0x100);
                Contents[EXTERNAL_CODEPLUG_SIZE+1] = (byte)(value % 0x100);
            }
        }

        public Block31 Block31 { get; set; }
        public Block3D Block3D { get; set; }
        public Block36 Block36 { get; set; }
        public Block55 Block55 { get; set; }
        public Block54 Block54 { get; set; }
        public Block51 Block51 { get; set; }

        public byte[] Unknown3
        {
            get => Contents.Slice(UNKNOWN3, 3).ToArray();
            //set => XYZ = value; //TODO
        }

        public Block39 Block39 { get; set; }
        public Block3B Block3B { get; set; }
        public Block34 Block34 { get; set; }
        public Block35 Block35 { get; set; }
        public Block3C Block3C { get; set; }
        public Block73 Block73 { get; set; }

        public byte[] Unknown4
        {
            get => Contents.Slice(UNKNOWN4, 12).ToArray();
            //set => XYZ = value; //TODO
        }
        public byte[] Unknown5
        {
            get => Contents.Slice(UNKNOWN5, 2).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

        public Block30(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block31 = new Block31(this, BLOCK_31_VECTOR, codeplugContents);
            Block3D = new Block3D(this, BLOCK_3D_VECTOR, codeplugContents);
            Block36 = new Block36(this, BLOCK_36_VECTOR, codeplugContents);
            Block55 = new Block55(this, BLOCK_55_VECTOR, codeplugContents);
            Block54 = new Block54(this, BLOCK_54_VECTOR, codeplugContents);
            Block51 = new Block51(this, BLOCK_51_VECTOR, codeplugContents);
            Block39 = new Block39(this, BLOCK_39_VECTOR, codeplugContents);
            Block3B = new Block3B(this, BLOCK_3B_VECTOR, codeplugContents);
            Block34 = new Block34(this, BLOCK_34_VECTOR, codeplugContents);
            Block35 = new Block35(this, BLOCK_35_VECTOR, codeplugContents);
            Block3C = new Block3C(this, BLOCK_3C_VECTOR, codeplugContents);
            Block73 = new Block73(this, BLOCK_73_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var s = new String(' ', Level * 2);
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(s + $"Unknown1 Bytes: {FormatHex(Unknown1)}");
            sb.AppendLine(s + $"Serial: {Serial}");
            sb.AppendLine(s + $"Model: {Model}");
            sb.AppendLine(s + $"Codeplug Time: {TimeStamp}");
            sb.AppendLine(s + $"Unknown2 Bytes: {FormatHex(Unknown2)}");
            sb.AppendLine(s + $"External Codeplug Size: {ExternalCodeplugSize}");
            sb.AppendLine(s + $"Block 31 Vector: {Block31?.Address:X4}");
            sb.AppendLine(s + $"Block 3D Vector: {Block3D?.Address:X4}");
            sb.AppendLine(s + $"Block 36 Vector: {Block36?.Address:X4}");
            sb.AppendLine(s + $"Block 55 Vector: {Block55?.Address:X4}");
            sb.AppendLine(s + $"Block 54 Vector: {Block54?.Address:X4}");
            sb.AppendLine(s + $"Block 51 Vector: {Block51?.Address:X4}");
            sb.AppendLine(s + $"Unknown3 Bytes: {FormatHex(Unknown3)}");
            sb.AppendLine(s + $"Block 39 Vector: {Block39?.Address:X4}");
            sb.AppendLine(s + $"Block 3B Vector: {Block3B?.Address:X4}");
            sb.AppendLine(s + $"Block 34 Vector: {Block34?.Address:X4}");
            sb.AppendLine(s + $"Block 35 Vector: {Block35?.Address:X4}");
            sb.AppendLine(s + $"Block 3C Vector: {Block3C?.Address:X4}");
            sb.AppendLine(s + $"Block 73 Vector: {Block73?.Address:X4}");
            sb.AppendLine(s + $"Unknown4 Bytes: {FormatHex(Unknown4)}");
            sb.AppendLine(s + $"Unknown5 Bytes: {FormatHex(Unknown5)}");

            sb.AppendLine(Block31.ToString());
            sb.AppendLine(Block3D.ToString());
            sb.AppendLine(Block36.ToString());
            sb.AppendLine(Block55.ToString());
            sb.AppendLine(Block54.ToString());
            sb.AppendLine(Block51.ToString());
            sb.AppendLine(Block39.ToString());
            sb.AppendLine(Block3B.ToString());
            sb.AppendLine(Block34.ToString());
            sb.AppendLine(Block35.ToString());
            sb.AppendLine(Block3C.ToString());
            sb.AppendLine(Block73.ToString());
            return sb.ToString();
        }
    }
}
