using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block05 : Block
    {
        #region Propeties
        public Block02 Block02 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 3D 46 11  1E 00 1D
        */

        private const int BLOCK_02_VECTOR = 0x00;
        #endregion

        public Block05(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x05;
            Description = "HWConfig Secure";
            LongChecksum = false;

            Block02 = new Block02(this, BLOCK_02_VECTOR);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block02.ToString());

            return sb.ToString();
        }
    }
}
