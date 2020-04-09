using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block0A : Block
    {
        #region Propeties
        public Block0B Block0B { get; set; }
        //public Block0B Block0B { get; set; }
        //public Block0B Block0B { get; set; }
        //public Block0B Block0B { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 0C 01 13  01 1A 01 21
        */

        private const int BLOCK_0B_VECTOR = 0x00;
        //private const int BLOCK_0B_VECTOR = 0x02;
        //private const int BLOCK_0B_VECTOR = 0x04;
        //private const int BLOCK_0B_VECTOR = 0x06;
        #endregion

        public Block0A(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x0A;
            Description = "Softpot B/W Vector";

            Block0B = new Block0B(this, BLOCK_0B_VECTOR);
            //Block0B = new Block0B(this, BLOCK_0B_VECTOR);
            //Block0B = new Block0B(this, BLOCK_0B_VECTOR);
            //Block0B = new Block0B(this, BLOCK_0B_VECTOR);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block0B.ToString());
 
            return sb.ToString();
        }
    }
}
