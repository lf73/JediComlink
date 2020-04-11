using System;
using System.Collections.Generic;
using System.Text;

namespace JediComlink
{
    public class Block0A : Block
    {
        public override int Id { get => 0x0A; }
        public override string Description { get => "Softpot B/W Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 0C 01 13  01 1A 01 21  //TODO ??There's no count byte.. Is this fixed?
        */
        #endregion

        #region Propeties
        public List<Block0B> Block0BList { get; set; } = new List<Block0B>();
        #endregion

        public Block0A(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            for (int i = 0; i < 4; i++) //4 Hardcoded for number of elements. This one is not dynamic like others?
            {
                Block0BList.Add(new Block0B(this, i * 2, codeplugContents));
            }
        }

        public override string ToString()
        {
            var s = new String(' ', Level * 2);
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(s + $"Block 0B Couunt: {Block0BList.Count}");
            foreach (var block09 in Block0BList)
            {
                sb.AppendLine(s + $"Block 0B Vector: {block09.Address:X4}");
            }

            foreach (var block09 in Block0BList)
            {
                sb.AppendLine(block09.ToString());
            }

            return sb.ToString();
        }
    }
}
