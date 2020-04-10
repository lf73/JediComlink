using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block44 : Block
    {
        public override int Id { get => 0x44; }
        public override string Description { get => "MDC Configuration"; }

        #region Propeties
        public Block47 Block47 { get; set; }
        public Block92 Block92 { get; set; }
        public Block9F Block9F { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 04 2F 04 55  04 5B
        */

        private const int BLOCK_47_VECTOR = 0x00;
        private const int BLOCK_92_VECTOR = 0x04;
        private const int BLOCK_9F_VECTOR = 0x02;
        #endregion

        public Block44(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block47 = new Block47(this, BLOCK_47_VECTOR, codeplugContents);
            Block92 = new Block92(this, BLOCK_92_VECTOR, codeplugContents);
            Block9F = new Block9F(this, BLOCK_9F_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block47.ToString());
            sb.AppendLine(Block92.ToString());
            sb.AppendLine(Block9F.ToString());

            return sb.ToString();
        }
    }
}
