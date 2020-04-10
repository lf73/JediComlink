using System;
using System.Text;

namespace JediComlink
{
    public class Block0B : Block
    {
        public override int Id { get => 0x0B; }
        public override string Description { get => "Softpot B/W"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 45 A4 0C 96
        */

        private const int UNKNOWN1 = 0x00; //01 02 03
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 4).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

        public Block0B(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {

        }

        public override string ToString()
        {
            var s = new String(' ', Level * 2);
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(s + $"Unknown1 Bytes: {FormatHex(Unknown1)}");

            return sb.ToString();
        }
    }
}
