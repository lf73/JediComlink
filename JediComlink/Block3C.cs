using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block3C : Block
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 0A 01 16  02 17 B0 30   B1 31 B3 33  40 02 41 00
        1: B2 32 B4 34  06 01
        */

        #endregion

        public Block3C(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x3C;
            Description = "Hardware Button Configuration";
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
