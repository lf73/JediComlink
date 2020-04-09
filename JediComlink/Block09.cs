using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block09 : Block
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 35 E8 32 AF  00 00 4F 50   50 50 3F 2C  28
        */

        #endregion

        public Block09(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x09;
            Description = "Softpot Interpol";
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
