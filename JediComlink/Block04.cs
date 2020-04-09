using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block04 : Block
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 32 22 12 00  04 08 0C 14   1C 20 28
        */

        #endregion

        public Block04(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x04;
            Description = "HWConfig Conv";
            LongChecksum = false;

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
