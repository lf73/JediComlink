using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block58 : Block
    {
        #region Propeties
        public Block59 Block59 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 04 49 02 FF  FF FF FF FF   FF
        */

        private const int BLOCK_59_VECTOR = 0x00;
        #endregion

        public Block58(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x58;
            Description = "Trunk Call List Vector or CD: ASTRO25 Trunk Call List Vector";

            Block59 = new Block59(this, BLOCK_59_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block59.ToString());

            return sb.ToString();
        }
    }
}
