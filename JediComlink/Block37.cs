using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block37 : BlockLong
    {
        public override int Id { get => 0x37; }
        public override string Description { get => "Zone Chan Text Vector"; }

        #region Propeties
        public Block38 Block38 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 04 D6
        */

        private const int BLOCK_38_VECTOR = 0x01;
        #endregion

        public Block37(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block38 = new Block38(this, BLOCK_38_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block38.ToString());

            return sb.ToString();
        }
    }
}
