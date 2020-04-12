using System;
using System.Text;

namespace JediComlink
{
    public class Block0F : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x0F; }
        public override string Description { get => "AD Switch Level"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 27 10 08 FF  FF 00 27 10   46 FF FF 00
        */

        private const int UNKNOWN1 = 0x00; //01 02 03 04 05 06 07 08 09 0A 0B
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 12).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

        public Block0F() { }

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
