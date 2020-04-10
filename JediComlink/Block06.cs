using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block06 : Block
    {
        public override int Id { get => 0x06; }
        public override string Description { get => "Softpot Vector"; }

        #region Propeties
        public Block07 Block07 { get; set; }
        public Block08 Block08 { get; set; }
        public Block0A Block0A { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 7A 00 91  01 01
        */

        private const int BLOCK_07_VECTOR = 0x00;
        private const int BLOCK_08_VECTOR = 0x02;
        private const int BLOCK_0A_VECTOR = 0x04;
        #endregion

        public Block06(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block07 = new Block07(this, BLOCK_07_VECTOR, codeplugContents);
            Block08 = new Block08(this, BLOCK_08_VECTOR, codeplugContents);
            Block0A = new Block0A(this, BLOCK_0A_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block07.ToString());
            sb.AppendLine(Block08.ToString());
            sb.AppendLine(Block0A.ToString());

            return sb.ToString();
        }
    }
}
