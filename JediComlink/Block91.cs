using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block91 : BlockLong
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 10 4D 53  47 20 31 20   4D 53 47 20  32 20 4D 53
        1: 47 20 33 20  4D 53 47 20   34 20 4D 53  47 20 35 20
        2: 4D 53 47 20  36 20 4D 53   47 20 37 20  4D 53 47 20
        3: 38 20 4D 53  47 20 39 20   4D 53 47 20  31 30 4D 53
        4: 47 20 31 31  4D 53 47 20   31 32 4D 53  47 20 31 33
        5: 4D 53 47 20  31 34 4D 53   47 20 31 35  4D 53 47 20
        6: 31 36 00
        */

        #endregion

        public Block91(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x91;
            Description = "Message List";
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
