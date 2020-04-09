using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block48 : Block
    {
        #region Propeties
        public Block03 Block03 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 B4 0A 0A  00 01 00 00   00 00 00 00  3C 03 05 15
        1: 15 09 05 01  01 01 3C 00   00 15 01 00  00
        */

        private const int BLOCK_03_VECTOR = 0x15;
        #endregion

        public Block48(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x48;
            Description = "MDC System";

            Block03 = new Block03(this, BLOCK_03_VECTOR);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block03.ToString());

            return sb.ToString();
        }
    }
}
