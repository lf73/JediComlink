using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block02 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x02; }
        public override string Description { get => "HWConfig Radio"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 71 01 28  01 5E 10 00   00 07 21 3F  8A 60 FF 00
        1: 28 28 00 8E  4B 00 03 07   0F AB BC 02  18 64 28 01
        2: 01 7F 00 93  00 07 FF 02   28 28 02 01  8E 49 67 50
        3: 65
        */

        private const int BLOCK_06_VECTOR = 0x00; //01
        private const int BLOCK_03_VECTOR = 0x02; //03
        private const int BLOCK_0C_VECTOR = 0x04; //05
        private const int UNKNOWN1 = 0x06; //07 08 09 0A 0B 0C 0D 0E 0F 10 11 12 13 14 15 16 17 18 19 1A 1B 1C 1D 1E 1F
        private const int BLOCK_0F_VECTOR = 0x20; //21
        private const int UNKNOWN2 = 0x22; //23 24 25 26 27 28 29 2A
        private const int BLOCK_11_VECTOR = 0x2B; //2C
        private const int UNKNOWN3 = 0x2D; //2E 2F 30
        #endregion

        #region Propeties
        public Block06 Block06 { get; set; }

        public Block03 Block03 { get; set; }

        public Block0C Block0C { get; set; }

        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 26).ToArray();
            //set => XYZ = value; //TODO
        }

        public Block0F Block0F { get; set; }

        public byte[] Unknown2
        {
            get => Contents.Slice(UNKNOWN2, 9).ToArray();
            //set => XYZ = value; //TODO
        }

        public Block11 Block11 { get; set; }
        public byte[] Unknown3
        {
            get => Contents.Slice(UNKNOWN3, 3).ToArray();
            //set => XYZ = value; //TODO
        }

        #endregion

        public Block02() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = GetContents(codeplugContents, address);
            Block06 = Deserialize<Block06>(Contents, BLOCK_06_VECTOR, codeplugContents);
            Block03 = Deserialize<Block03>(Contents, BLOCK_03_VECTOR, codeplugContents);
            Block0C = Deserialize<Block0C>(Contents, BLOCK_0C_VECTOR, codeplugContents);
            Block0F = Deserialize<Block0F>(Contents, BLOCK_0F_VECTOR, codeplugContents);
            Block11 = Deserialize<Block11>(Contents, BLOCK_11_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine($"Unknown1 Bytes: {FormatHex(Unknown1)}");
            sb.AppendLine($"Unknown2 Bytes: {FormatHex(Unknown2)}");
            sb.AppendLine($"Unknown3 Bytes: {FormatHex(Unknown3)}");

            sb.AppendLine(Block06?.ToString());
            sb.AppendLine(Block03?.ToString());
            sb.AppendLine(Block0C?.ToString());
            sb.AppendLine(Block0F?.ToString());
            sb.AppendLine(Block11?.ToString());
            return sb.ToString();
        }
    }
}
