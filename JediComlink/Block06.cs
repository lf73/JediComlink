using System;
using System.Text;

namespace JediComlink
{
    public class Block06 : Block
    {
        public override int Id { get => 0x06; }
        public override string Description { get => "Softpot Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 7A 00 91  01 01
        */

        private const int BLOCK_07_VECTOR = 0x00; //01
        private const int BLOCK_08_VECTOR = 0x02; //03
        private const int BLOCK_0A_VECTOR = 0x04; //05
        #endregion

        #region Propeties
        public Block07 Block07 { get; set; }

        public Block08 Block08 { get; set; }

        public Block0A Block0A { get; set; }
        #endregion



        public Block06(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block07 = new Block07(this, BLOCK_07_VECTOR, codeplugContents);
            Block08 = new Block08(this, BLOCK_08_VECTOR, codeplugContents);
            Block0A = new Block0A(this, BLOCK_0A_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var s = new String(' ', Level * 2);
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(s + $"Block 07 Vector: {Block07?.Address:X4}");
            sb.AppendLine(s + $"Block 08 Vector: {Block08?.Address:X4}");
            sb.AppendLine(s + $"Block 0A Vector: {Block0A?.Address:X4}");

            sb.AppendLine(Block07.ToString());
            sb.AppendLine(Block08.ToString());
            sb.AppendLine(Block0A.ToString());

            return sb.ToString();
        }
    }
}
