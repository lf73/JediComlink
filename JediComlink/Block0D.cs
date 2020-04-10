using System;
using System.Text;

namespace JediComlink
{
    public class Block0D : Block
    {
        public override int Id { get => 0x0D; }
        public override string Description { get => "HWConfig MDC"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 12 05 07 04  23 23 03 04   00 00 00 00  AA AA AA AA
        1: 01 06
        */

        private const int UNKNOWN1 = 0x00; //01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F 10 11
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 18).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

        public Block0D(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
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
