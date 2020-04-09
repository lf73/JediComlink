using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class BlockA0 : Block
    {
        #region Propeties
        public BlockA9 BlockA9 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 00 00 00  00 00 00 00   04 99
        */

        private const int BLOCK_A9_VECTOR = 0x08;
        #endregion

        public BlockA0(Block parent, int vector) : base(parent, vector)
        {
            Id = 0xA0;
            Description = "Aux Signalling";

            BlockA9 = new BlockA9(this, BLOCK_A9_VECTOR);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(BlockA9.ToString());

            return sb.ToString();
        }
    }
}
