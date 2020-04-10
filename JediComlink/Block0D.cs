using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block0D : Block
    {
        public override int Id { get => 0x0D; }
        public override string Description { get => "HWConfig MDC"; }

        #region Propeties
        public Block4B Block4B { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 12 05 07 04  23 23 03 04   00 00 00 00  AA AA AA AA
        1: 01 06
        */

        private const int BLOCK_4B_VECTOR = 0x07;
        #endregion

        public Block0D(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block4B = new Block4B(this, BLOCK_4B_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block4B.ToString());

            return sb.ToString();
        }
    }
}
