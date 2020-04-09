using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block51 : Block
    {
        #region Propeties
        public Block52 Block52 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 08 29 68 03  14 06 00 00   06 0C 23 0B  0A 06 01 00
        1: 00
        */

        private const int BLOCK_52_VECTOR = 0x00;
        #endregion

        public Block51(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x51;
            Description = "Scan Configuration";

            Block52 = new Block52(this, BLOCK_52_VECTOR);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block52.ToString());

            return sb.ToString();
        }
    }
}
