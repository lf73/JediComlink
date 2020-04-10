using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block52 : Block
    {
        public override int Id { get => 0x52; }
        public override string Description { get => "Scan List Vector"; }

        #region Propeties
        public Block53 Block53 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 08 2F
        */

        private const int BLOCK_53_VECTOR = 0x01;
        #endregion

        public Block52(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block53 = new Block53(this, BLOCK_53_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block53.ToString());

            return sb.ToString();
        }
    }
}
