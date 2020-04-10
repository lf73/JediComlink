using System;
using System.Text;

namespace JediComlink
{
    public class Block04 : Block
    {
        public override int Id { get => 0x04; }
        public override string Description { get => "HWConfig Conv"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 32 22 12 00  04 08 0C 14   1C 20 28
        */

        private const int UNKNOWN1 = 0x00; //01 02 03 04 05 06 07 08 09 0A
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 11).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion


        public Block04(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
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
