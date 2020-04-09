using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class BlockXX : Block
    {
        #region Propeties
        public Block BlockYY { get; set; }

        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        */

        private const int XXXXX = 0x00;
        #endregion

        public BlockXX(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x99;
            Description = "XXXXX";
            LongChecksum = false;


        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());



            return sb.ToString();
        }
    }
}
