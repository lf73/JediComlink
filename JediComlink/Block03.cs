using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block03 : Block
    {
        #region Propeties
        public Block04 Block04 { get; set; }
        public Block05 Block05 { get; set; }
        public Block0D Block0D { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 31 01 3F  01 49
        */

        private const int BLOCK_04_VECTOR = 0x00;
        private const int BLOCK_05_VECTOR = 0x02;
        private const int BLOCK_0D_VECTOR = 0x04;
        #endregion

        public Block03(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x03;
            Description = "HWConfig Vector";

            Block04 = new Block04(this, BLOCK_04_VECTOR);
            Block05 = new Block05(this, BLOCK_05_VECTOR);
            Block0D = new Block0D(this, BLOCK_0D_VECTOR);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block04.ToString());
            sb.AppendLine(Block05.ToString());
            sb.AppendLine(Block0D.ToString());

            return sb.ToString();
        }
    }
}
