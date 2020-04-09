using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block02 : Block
    {
        #region Propeties
        // public Block03 Block03 { get; set; }
        // public Block06 Block06 { get; set; }
        // public Block0A Block0A { get; set; }
        // public Block0C Block0C { get; set; }
        // public Block0F Block0F { get; set; }
        // public Block11 Block11 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 71 01 28  01 5E 10 00   00 07 21 3F  8A 60 FF 00
        1: 28 28 00 8E  4B 00 03 07   0F AB BC 02  18 64 28 01
        2: 01 7F 00 93  00 07 FF 02   28 28 02 01  8E 49 67 50
        3: 65
        */

        private const int BLOCK_03_VECTOR = 0x02;
        private const int BLOCK_06_VECTOR = 0x00;
        private const int BLOCK_0A_VECTOR = 0x1F;
        private const int BLOCK_0C_VECTOR = 0x04;
        private const int BLOCK_0F_VECTOR = 0x20;
        private const int BLOCK_11_VECTOR = 0x2B;


        #endregion

        public Block02(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x02;
            Description = "HWConfig Radio";
            LongChecksum = false;

            // Block03 = new Block03(this, BLOCK_03_VECTOR);
            // Block06 = new Block06(this, BLOCK_06_VECTOR);
            // Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            // Block0C = new Block0C(this, BLOCK_0C_VECTOR);
            // Block0F = new Block0F(this, BLOCK_0F_VECTOR);
            // Block11 = new Block11(this, BLOCK_11_VECTOR);
        }

        public override string ToString()
        {
            var s = new String(' ', Level * 2);
            var sb = new StringBuilder();

            // sb.AppendLine(Block03.ToString());
            // sb.AppendLine(Block06.ToString());
            // sb.AppendLine(Block0A.ToString());
            // sb.AppendLine(Block0C.ToString());
            // sb.AppendLine(Block0F.ToString());
            // sb.AppendLine(Block11.ToString());

            sb.AppendLine(GetTextHeader());
            return sb.ToString();
        }
    }
}
