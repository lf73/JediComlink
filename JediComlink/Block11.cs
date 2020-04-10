using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block11 : Block
    {
        public override int Id { get => 0x11; }
        public override string Description { get => "AD Button Level"; }

        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 1F 30 70  85 CF 55 AA
        */

        #endregion

        public Block11(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
