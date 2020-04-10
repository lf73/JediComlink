using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block30 : Block
    {
        public override int Id { get => 0x30; }
        public override string Description { get => "External Radio"; }

        #region Propeties
        public byte[] Unknown1 { get; set; }
        public string Serial { get; set; }
        public string Model { get; set; }
        public DateTime TimeStamp { get; set; }
        public byte[] Unknown3 { get; set; }
        public int CodeplugSize { get; set; }
        public Block31 Block31 { get; set; }
        public Block3D Block3D { get; set; }
        public Block36 Block36 { get; set; }
        public Block55 Block55 { get; set; }
        public Block54 Block54 { get; set; }
        public Block51 Block51 { get; set; }
        public Block Block33 { get; set; }
        public Block39 Block39 { get; set; }
        public Block3B Block3B { get; set; }
        public Block34 Block34 { get; set; }
        public Block35 Block35 { get; set; }
        public Block3C Block3C { get; set; }
        public Block73 Block73 { get; set; }
        public byte[] Unknown4 { get; set; }
        //public BlockB5 BlockB5 { get; set; }
        public byte[] Unknown5 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 3E 00 34 33  32 43 44 4E   30 30 30 32  48 30 31 4B
        1: 44 44 39 50  57 31 42 4E   00 00 00 00  11 02 24 14
        2: 42 03 00 1C  07 71 02 D1   03 0A 04 B7  05 1F 06 65
        3: 06 74 06 E4  07 16 08 36   08 3E 08 82  09 82 09 9B
        4: 00 00 00 00  00 00 00 00   00 00 09 AB  00 00
        */

        private const int BLOCK_31_VECTOR = 0x26;
        private const int BLOCK_34_VECTOR = 0x38;
        private const int BLOCK_35_VECTOR = 0x3A;
        private const int BLOCK_36_VECTOR = 0x2A;
        private const int BLOCK_39_VECTOR = 0x34;
        private const int BLOCK_3B_VECTOR = 0x36;
        private const int BLOCK_3C_VECTOR = 0x3C;
        private const int BLOCK_3D_VECTOR = 0x28;
        private const int BLOCK_51_VECTOR = 0x30;
        private const int BLOCK_54_VECTOR = 0x2E;
        private const int BLOCK_55_VECTOR = 0x2C;
        private const int BLOCK_73_VECTOR = 0x3E;
        #endregion

        public Block30(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Unknown1 = Contents.Slice(0x00, 2).ToArray();
            Serial = GetStringContents(0x02, 10);
            Model = GetStringContents(0x0C, 16);
            TimeStamp = new DateTime(2000 + GetDigits(Contents[0x1c]),
                                    GetDigits(Contents[0x1d]),
                                    GetDigits(Contents[0x1e]),
                                    GetDigits(Contents[0x1f]),
                                    GetDigits(Contents[0x20]),
                                    0);
            Unknown3 = Contents.Slice(0x21, 3).ToArray();
            CodeplugSize = Contents[0x24] * 0x100 + Contents[0x25];
            Block31 = new Block31(this, BLOCK_31_VECTOR, codeplugContents);
            Block3D = new Block3D(this, BLOCK_3D_VECTOR, codeplugContents);
            Block36 = new Block36(this, BLOCK_36_VECTOR, codeplugContents);
            Block55 = new Block55(this, BLOCK_55_VECTOR, codeplugContents);
            Block54 = new Block54(this, BLOCK_54_VECTOR, codeplugContents);
            Block51 = new Block51(this, BLOCK_51_VECTOR, codeplugContents);
            //Block33 = new Block33(this, 0x32);
            Block39 = new Block39(this, BLOCK_39_VECTOR, codeplugContents);
            Block3B = new Block3B(this, BLOCK_3B_VECTOR, codeplugContents);
            Block34 = new Block34(this, BLOCK_34_VECTOR, codeplugContents);
            Block35 = new Block35(this, BLOCK_35_VECTOR, codeplugContents);
            Block3C = new Block3C(this, BLOCK_3C_VECTOR, codeplugContents);
            Block73 = new Block73(this, BLOCK_73_VECTOR, codeplugContents);
            Unknown4 = Contents.Slice(0x40, 10).ToArray();
            //BlockB5 = new BlockB5(this, 0x4A);
            Unknown5 = Contents.Slice(0x4C, 2).ToArray();
        }

        public override string ToString()
        {
            var s = new String(' ', Level * 2);
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(s + $"Unknown Bytes: {FormatHex(Unknown1)}");
            sb.AppendLine(s + $"Serial: {Serial}");
            sb.AppendLine(s + $"Model: {Model}");
            sb.AppendLine(s + $"Codeplug Time: {TimeStamp}");
            sb.AppendLine(s + $"Unknown Bytes: {FormatHex(Unknown3)}");
            sb.AppendLine(s + $"Code Plug Size {CodeplugSize}");
            sb.AppendLine(Block31.ToString());
            sb.AppendLine(Block3D.ToString());
            sb.AppendLine(Block36.ToString());
            sb.AppendLine(Block55.ToString());
            sb.AppendLine(Block54.ToString());
            sb.AppendLine(Block51.ToString());
            sb.AppendLine(Block33?.ToString());
            sb.AppendLine(Block39.ToString());
            sb.AppendLine(Block3B.ToString());
            sb.AppendLine(Block34.ToString());
            sb.AppendLine(Block35.ToString());
            sb.AppendLine(Block3C.ToString());
            sb.AppendLine(Block73.ToString());
            sb.AppendLine(s + $"Unknown Bytes: {FormatHex(Unknown4)}");
            //sb.AppendLine(BlockB5.ToString());
            sb.AppendLine(s + $"Unknown Bytes: {FormatHex(Unknown5)}");
            return sb.ToString();
        }
    }
}
