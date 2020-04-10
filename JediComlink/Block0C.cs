using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block0C : Block
    {
        public override int Id { get => 0x0C; }
        public override string Description { get => "Unknown"; }

        #region Propeties
        public Block30 Block30 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 67 4C E7 A7  7A 02 00 22   01 86 00 1E  01 0E 00 20
        1: 01 AE 00 22  01 38 00 1E   00 D8 00 20  01 58
        */

        private const int BLOCK_30_VECTOR = 0x05;
        #endregion

        public Block0C(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block30 = new Block30(this, BLOCK_30_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block30.ToString());

            return sb.ToString();
        }
    }
}
