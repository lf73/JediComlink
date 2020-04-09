using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block3A : BlockLong
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 13 50 48  4E 20 31 20   50 48 4E 20  32 20 50 48
        1: 4E 20 33 20  50 48 4E 20   34 20 50 48  4E 20 35 20
        2: 50 48 4E 20  36 20 50 48   4E 20 37 20  50 48 4E 20
        3: 38 20 50 48  4E 20 39 20   50 48 4E 20  31 30 50 48
        4: 4E 20 31 31  50 48 4E 20   31 32 50 48  4E 20 31 33
        5: 50 48 4E 20  31 34 50 48   4E 20 31 35  50 48 4E 20
        6: 31 36 50 48  4E 20 31 37   50 48 4E 20  31 38 50 48
        7: 4E 20 31 39
        */

        #endregion

        public Block3A(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x3A;
            Description = "Phone List";

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
