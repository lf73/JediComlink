using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block36 : Block
    {
        public override int Id { get => 0x36; }
        public override string Description { get => "Display & Menu"; }

        #region Propeties
        public Block37 Block37 { get; set; }
        public Block90 Block90 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 04 CE 00 06  14 0A 03 28   28 07 20 00  08 00 00 00
        1: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        2: 00 00 00 00  00 00 00
        */

        private const int BLOCK_37_VECTOR = 0x00;
        private const int BLOCK_90_VECTOR = 0x06;
        #endregion

        public Block36(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block37 = new Block37(this, BLOCK_37_VECTOR, codeplugContents);
            Block90 = new Block90(this, BLOCK_90_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block37.ToString());
            sb.AppendLine(Block90.ToString());

            return sb.ToString();
        }
    }
}
