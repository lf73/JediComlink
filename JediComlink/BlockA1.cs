using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class BlockA1 : Block
    {
        #region Propeties
        public BlockA2 BlockA2 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 04 9E
        */

        private const int BLOCK_A2_VECTOR = 0x01;
        #endregion

        public BlockA1(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0xA1;
            Description = "Singletone System Vector";

            BlockA2 = new BlockA2(this, BLOCK_A2_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(BlockA2.ToString());

            return sb.ToString();
        }
    }
}
