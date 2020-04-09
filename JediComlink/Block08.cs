using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block08 : Block
    {
        #region Propeties
        public Block09 Block09 { get; set; }
        //public Block09 Block09 { get; set; }
        //public Block09 Block09 { get; set; }
        //public Block09 Block09 { get; set; }
        //public Block09 Block09 { get; set; }
        //public Block09 Block09 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 00 A1 00  B1 00 C1 00   D1 00 E1 00  F1
        */

        private const int BLOCK_09_VECTOR = 0x01;
        //private const int BLOCK_09_VECTOR = 0x03;
        //private const int BLOCK_09_VECTOR = 0x05;
        //private const int BLOCK_09_VECTOR = 0x07;
        //private const int BLOCK_09_VECTOR = 0x09;
        //private const int BLOCK_09_VECTOR = 0x0B;
        #endregion

        public Block08(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x08;
            Description = "Softpot Interpol Vector";
            LongChecksum = false;

            Block09 = new Block09(this, BLOCK_09_VECTOR);

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block09.ToString());


            return sb.ToString();
        }
    }
}
