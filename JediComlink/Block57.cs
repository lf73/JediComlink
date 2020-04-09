using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block57 : Block
    {
        #region Propeties
        public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        //public Block0A Block0A { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 10 01 01  01 01 01 01   01 01 01 01  01 01 01 01
        1: 01 01
        */

        private const int BLOCK_0A_VECTOR = 0x03;
        //private const int BLOCK_0A_VECTOR = 0x04;
        //private const int BLOCK_0A_VECTOR = 0x05;
        //private const int BLOCK_0A_VECTOR = 0x06;
        //private const int BLOCK_0A_VECTOR = 0x07;
        //private const int BLOCK_0A_VECTOR = 0x08;
        //private const int BLOCK_0A_VECTOR = 0x09;
        //private const int BLOCK_0A_VECTOR = 0x0A;
        //private const int BLOCK_0A_VECTOR = 0x0B;
        //private const int BLOCK_0A_VECTOR = 0x0C;
        //private const int BLOCK_0A_VECTOR = 0x0D;
        //private const int BLOCK_0A_VECTOR = 0x0E;
        //private const int BLOCK_0A_VECTOR = 0x0F;
        //private const int BLOCK_0A_VECTOR = 0x10;
        //private const int BLOCK_0A_VECTOR = 0x11;
        #endregion

        public Block57(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x57;
            Description = "Zone Chan TG Table";
            LongChecksum = true;

            Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
            //Block0A = new Block0A(this, BLOCK_0A_VECTOR);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block0A.ToString());


            return sb.ToString();
        }
    }
}
