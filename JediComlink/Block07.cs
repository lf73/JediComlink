using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block07 : Block
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 59 12 11 18  15 00 70 00   48 4D 4D 4D  03 86 06 8C
        1: CB 00 6E 51
        */

        #endregion

        public Block07(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x07;
            Description = "Softpot Radio";

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
