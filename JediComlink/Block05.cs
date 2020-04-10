using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block05 : Block
    {
        public override int Id { get => 0x05; }
        public override string Description { get => "HWConfig Secure"; }

        #region Propeties
        public Block02 Block02 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 3D 46 11  1E 00 1D
        */

        private const int BLOCK_02_VECTOR = 0x00;
        #endregion

        public Block05(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block02 = new Block02(this, BLOCK_02_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block02.ToString());

            return sb.ToString();
        }
    }
}
