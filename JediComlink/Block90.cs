using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block90 : Block
    {
        #region Propeties
        public Block91 Block91 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 03 3E 10 01  02 03 04 05   06 07 08 09  0A 0B 0C 0D
        1: 0E 0F 10
        */

        private const int BLOCK_91_VECTOR = 0x00;
        #endregion

        public Block90(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x90;
            Description = "Message List Vector";

            Block91 = new Block91(this, BLOCK_91_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block91.ToString());

            return sb.ToString();
        }
    }
}
