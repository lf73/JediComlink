using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block43 : Block
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 01 4D 50  4C 20 31 20
        */

        #endregion

        public Block43(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x43;
            Description = "Unknown";
            LongChecksum = true;

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
