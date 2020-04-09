using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block39 : Block
    {
        #region Propeties
        public Block3A Block3A { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 09 36 00 14  FF FF FF FF   FF FF FF FF  FF FF FF FF
        1: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        2: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        3: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        4: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        5: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        6: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        7: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        8: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        9: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        A: FF FF FF FF
        */

        private const int BLOCK_3A_VECTOR = 0x00;
        #endregion

        public Block39(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x39;
            Description = "Phone List Vector";

            Block3A = new Block3A(this, BLOCK_3A_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block3A.ToString());

            return sb.ToString();
        }
    }
}
