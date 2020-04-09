using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block59 : BlockLong
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 01 43 41  4C 20 31 20
        */

        #endregion

        public Block59(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x59;
            Description = "Trunk Call List";

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
