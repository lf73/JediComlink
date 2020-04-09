using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block3F : Block
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 05 05 03 23  04 1D 60 18   61 23 62 23  26 26 26 48
        1: 26 26 26
        */

        #endregion

        public Block3F(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x3F;
            Description = "Control Head";

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
