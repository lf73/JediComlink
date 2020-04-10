using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block0F : Block
    {
        public override int Id { get => 0x0F; }
        public override string Description { get => "AD Switch Level"; }

        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 27 10 08 FF  FF 00 27 10   46 FF FF 00
        */

        #endregion

        public Block0F(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
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
