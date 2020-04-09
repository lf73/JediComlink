using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block5A : Block
    {
        #region Propeties
        public Block58 Block58 { get; set; }
        public Block61 Block61 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 02 1C 04 3D  04 56 95 64   1C 20 00 01  00 0C 00 00
        1: 00 00 00 00  00 00 00 00   00 00 04 64  00 00 00 01
        2: 00 26 01 26  01 26 01 26   01 00 00 00  00 00 00 00
        3: 00 00 00 05
        */

        private const int BLOCK_58_VECTOR = 0x02;
        private const int BLOCK_61_VECTOR = 0x04;
        #endregion

        public Block5A(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x5A;
            Description = "Trunk System";

            Block58 = new Block58(this, BLOCK_58_VECTOR, codeplugContents);
            Block61 = new Block61(this, BLOCK_61_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block58.ToString());
            sb.AppendLine(Block61.ToString());

            return sb.ToString();
        }
    }
}
