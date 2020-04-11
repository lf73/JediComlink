using System;
using System.Text;

namespace JediComlink
{
    public class Block0E : Block
    {
        public override int Id { get => 0x0E; }
        public override string Description { get => "Test Channel Table"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 03 22 1F  4A 08 C2 24   EA 0E FE 2B  16 1F 42 1F
        1: 4A 24 E2 24  EA 2B 1E 2B   16
        */

        // Byte 0 is number of channels
        // TT TT RR RR  Transmit/Receive
        // TTTT * 0.00625 + 801

        private const int UNKNOWN1 = 0x00; // to 18
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 25).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

        public Block0E(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
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
