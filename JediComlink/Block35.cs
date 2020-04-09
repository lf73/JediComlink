using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block35 : Block
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        1: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        2: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        3: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        4: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        5: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        6: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        7: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        8: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        9: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        A: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        B: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        C: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        D: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        E: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        F: 00 00 00 00  00 00 00 00   00 00 00 00  00
        */

        #endregion

        public Block35(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x35;
            Description = "Unknown";
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
