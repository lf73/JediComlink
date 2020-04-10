using System;
using System.Text;

namespace JediComlink
{
    public class Block56 : Block
    {
        public override int Id { get => 0x56; }
        public override string Description { get => "Conv or 62: Trunk/Test Mode Personality"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 25 95 64  05 CE 21 EE   1F 4A 00 10  10 10 08 60
        1: 80 20 00 1E  00 00 01 01   00 00 00 00  00 00 00 00
        2: 00 00 00
        */

        private const int UNKNOWN1 = 0x00; //TO 22
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 35).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

        public Block56(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
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
