using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block42 : Block
    {
        #region Propeties
        public Block43 Block43 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 02 D4 01 00  00 00 00
        */

        private const int BLOCK_43_VECTOR = 0x00;
        #endregion

        public Block42(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x42;
            Description = "Unknown";

            Block43 = new Block43(this, BLOCK_43_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block43.ToString());

            return sb.ToString();
        }
    }
}
