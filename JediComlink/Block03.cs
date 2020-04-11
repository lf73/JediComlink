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
        public override int Id { get => 0x03; }
        public override string Description { get => "HWConfig Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 31 01 3F  01 49
        */

        private const int BLOCK_04_VECTOR = 0x00; //01
        private const int BLOCK_05_VECTOR = 0x02; //03
        private const int BLOCK_0D_VECTOR = 0x04; //05
        #endregion

        #region Propeties
        public Block04 Block04 { get; set; }
        public Block05 Block05 { get; set; }
        public Block0D Block0D { get; set; }
        #endregion

 
        public Block03(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block04 = new Block04(this, BLOCK_04_VECTOR, codeplugContents);
            Block05 = new Block05(this, BLOCK_05_VECTOR, codeplugContents);
            Block0D = new Block0D(this, BLOCK_0D_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var s = new String(' ', Level * 2);
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(s + $"Block 04 Vector: {Block04?.Address:X4}");
            sb.AppendLine(s + $"Block 05 Vector: {Block05?.Address:X4}");
            sb.AppendLine(s + $"Block 0D Vector: {Block0D?.Address:X4}");

            sb.AppendLine(Block04.ToString());
            sb.AppendLine(Block05.ToString());
            sb.AppendLine(Block0D.ToString());

            return sb.ToString();
        }
    }
}
