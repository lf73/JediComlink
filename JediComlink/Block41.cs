using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block41 : Block
    {
        #region Propeties
        public Block0A Block0A { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 01 37
        */

        private const int BLOCK_0A_VECTOR = 0x00;
        #endregion

        public Block41(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x41;
            Description = "Menu Item";
            LongChecksum = false;

            Block0A = new Block0A(this, BLOCK_0A_VECTOR);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block0A.ToString());

            return sb.ToString();
        }
    }
}
