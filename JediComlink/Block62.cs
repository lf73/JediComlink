using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block62 : Block
    {
        public override int Id { get => 0x62; }
        public override string Description { get => "Unknown"; }

        #region Propeties
        public Block58 Block58 { get; set; }
        public Block63 Block63 { get; set; }
        public Block67 Block67 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 02 0E 01 07  E4 22 00 00   04 3D 00 00  07 F3 00 00
        1: 00 00 00 00  00 00 00 00   00 00 00 00  00
        */

        private const int BLOCK_58_VECTOR = 0x08;
        private const int BLOCK_63_VECTOR = 0x0C;
        private const int BLOCK_67_VECTOR = 0x03;
        #endregion

        public Block62(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block58 = new Block58(this, BLOCK_58_VECTOR, codeplugContents);
            Block63 = new Block63(this, BLOCK_63_VECTOR, codeplugContents);
            Block67 = new Block67(this, BLOCK_67_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block58.ToString());
            sb.AppendLine(Block63.ToString());
            sb.AppendLine(Block67.ToString());

            return sb.ToString();
        }
    }
}
