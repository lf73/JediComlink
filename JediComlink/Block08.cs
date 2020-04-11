using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block08 : Block
    {
        public override int Id { get => 0x08; }
        public override string Description { get => "Softpot Interpol Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 00 A1 00  B1 00 C1 00   D1 00 E1 00  F1
        */

        #endregion


        #region Propeties
        public List<Block09> Block09List { get; set; } = new List<Block09>();
        #endregion

 
        public Block08(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            for (int i = 0; i < Contents[0]; i++)
            {
                Block09List.Add(new Block09(this, i * 2 + 1, codeplugContents));
            }           
        }

        public override string ToString()
        {
            var s = new String(' ', Level * 2);
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(s + $"Block 09 Couunt: {Block09List.Count}");
            foreach (var block09 in Block09List)
            {
                sb.AppendLine(s + $"Block 09 Vector: {block09.StartAddress:X4}");
            }

            foreach (var block09 in Block09List)
            {
                sb.AppendLine(block09.ToString());
            }

            return sb.ToString();
        }
    }
}
