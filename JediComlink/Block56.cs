using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block56 : Block
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 25 95 64  05 CE 21 EE   1F 4A 00 10  10 10 08 60
        1: 80 20 00 1E  00 00 01 01   00 00 00 00  00 00 00 00
        2: 00 00 00
        */

        #endregion

        public Block56(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x56;
            Description = "Conv or 62: Trunk/Test Mode Personality";
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
