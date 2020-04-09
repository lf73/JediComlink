using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block31 : Block
    {
        #region Propeties
        public Block90 Block90 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 14 03 28  1E 10 41 60   00 02 04 08  00 05 00 00
        1: 06 00 00 05  00 00 05 00   28 09 00 00  0A 0A 00 01
        2: 01 01 00 00  00 00 00 28   00 00 00 00  00 00 00 00
        3: 00 00 00 00  00 00
        */

        private const int BLOCK_90_VECTOR = 0x02;
        #endregion

        public Block31(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x31;
            Description = "Radio Wide";

            Block90 = new Block90(this, BLOCK_90_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block90.ToString());

            return sb.ToString();
        }
    }
}
