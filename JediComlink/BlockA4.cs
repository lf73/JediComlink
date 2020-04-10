using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class BlockA4 : Block
    {
        public override int Id { get => 0xA4; }
        public override string Description { get => "Unknown"; }

        #region Propeties
        public BlockA5 BlockA5 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 04 85
        */

        private const int BLOCK_A5_VECTOR = 0x01;
        #endregion

        public BlockA4(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            BlockA5 = new BlockA5(this, BLOCK_A5_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(BlockA5.ToString());

            return sb.ToString();
        }
    }
}
