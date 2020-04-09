using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block34 : Block
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        1: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        2: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        3: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        4: 00
        */

        #endregion

        public Block34(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x34;
            Description = "Bus Option Vector";

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
