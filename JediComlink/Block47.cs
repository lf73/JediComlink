using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block47 : Block
    {
        public override int Id { get => 0x47; }
        public override string Description { get => "MDC System Vector"; }

        #region Propeties
        public Block48 Block48 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 04 35
        */

        private const int BLOCK_48_VECTOR = 0x01;
        #endregion

        public Block47(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block48 = new Block48(this, BLOCK_48_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block48.ToString());

            return sb.ToString();
        }
    }
}
