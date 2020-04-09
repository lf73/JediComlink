using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block92 : Block
    {
        #region Propeties
        public Block10 Block10 { get; set; }
        public Block93 Block93 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 04 65 02 00  00 00 00
        */

        private const int BLOCK_10_VECTOR = 0x02;
        private const int BLOCK_93_VECTOR = 0x00;
        #endregion

        public Block92(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x92;
            Description = "MDC Call List Vector";

            Block10 = new Block10(this, BLOCK_10_VECTOR, codeplugContents);
            Block93 = new Block93(this, BLOCK_93_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block10.ToString());
            sb.AppendLine(Block93.ToString());

            return sb.ToString();
        }
    }
}
