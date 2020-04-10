using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block4A : Block
    {
        public override int Id { get => 0x4A; }
        public override string Description { get => "Trunk Configuration"; }

        #region Propeties
        public Block3F Block3F { get; set; }
        public Block41 Block41 { get; set; }
        public Block4B Block4B { get; set; }
        public Block57 Block57 { get; set; }
        public Block74 Block74 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 03 CD 03 E3  03 E9 05 00   41 01 04 00  65 04 84 00
        1: 00 30 0A 95  80 00 00 00   00 4A 3C 14  14 00 00 00
        2: 00 00 00 00
        */

        private const int BLOCK_3F_VECTOR = 0x00;
        private const int BLOCK_41_VECTOR = 0x02;
        private const int BLOCK_4B_VECTOR = 0x0A;
        private const int BLOCK_57_VECTOR = 0x04;
        private const int BLOCK_74_VECTOR = 0x0D;
        #endregion

        public Block4A(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block3F = new Block3F(this, BLOCK_3F_VECTOR, codeplugContents);
            Block41 = new Block41(this, BLOCK_41_VECTOR, codeplugContents);
            Block4B = new Block4B(this, BLOCK_4B_VECTOR, codeplugContents);
            Block57 = new Block57(this, BLOCK_57_VECTOR, codeplugContents);
            Block74 = new Block74(this, BLOCK_74_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block3F.ToString());
            sb.AppendLine(Block41.ToString());
            sb.AppendLine(Block4B.ToString());
            sb.AppendLine(Block57.ToString());
            sb.AppendLine(Block74.ToString());

            return sb.ToString();
        }
    }
}
