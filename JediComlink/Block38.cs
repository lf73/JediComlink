using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block38 : BlockLong
    {
        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 10 53 43 45  52 54 20 53   43 45 52 54  44 2D 2D 2D
        1: 2D 2D 2D 38  54 43 39 37   44 38 54 43  39 36 44 38
        2: 54 43 39 35  44 38 54 43   39 34 44 38  54 43 39 34
        3: 20 38 54 43  39 33 44 38   54 43 39 33  20 38 54 43
        4: 39 32 44 38  54 43 39 32   20 38 54 43  39 31 44 38
        5: 54 43 39 31  20 38 43 4C   39 30 44 38  43 4C 39 30
        6: 20
        */

        #endregion

        public Block38(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x38;
            Description = "Zone Chan Text";

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
