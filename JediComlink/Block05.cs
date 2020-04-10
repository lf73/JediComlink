using System;
using System.Text;

namespace JediComlink
{
    public class Block05 : Block
    {
        public override int Id { get => 0x05; }
        public override string Description { get => "HWConfig Secure"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 3D 46 11  1E 00 1D
        */

        private const int UNKNOWN1 = 0x00; //01 02 03 04 05 06
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 7).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion


        public Block05(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
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
