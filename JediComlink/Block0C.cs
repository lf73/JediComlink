using System;
using System.Text;

namespace JediComlink
{
    public class Block0C : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x0C; }
        public override string Description { get => "Unknown"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 67 4C E7 A7  7A 02 00 22   01 86 00 1E  01 0E 00 20
        1: 01 AE 00 22  01 38 00 1E   00 D8 00 20  01 58
        */

        private const int UNKNOWN1 = 0x00; //thru 0x1D
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 30).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

        public Block0C() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = GetContents(codeplugContents, address);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine($"Unknown1 Bytes: {FormatHex(Unknown1)}");

            return sb.ToString();
        }
    }
}
