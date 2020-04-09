using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block55 : BlockLong
    {
        #region Propeties
        public Block56 Block56 { get; set; }

        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 11 05 64 05  8A 05 B0 05   D6 05 FC 06  22 06 48 06
        1: 6E 06 94 06  BA 06 E0 07   06 07 2C 07  52 07 78 07
        2: 9E 07 C4
        */

        private const int BLOCK_56_VECTOR = 0x02;
        //private const int BLOCK_56_VECTOR = 0x04;
        //private const int BLOCK_56_VECTOR = 0x06;
        //private const int BLOCK_56_VECTOR = 0x08;
        //private const int BLOCK_56_VECTOR = 0x0A;
        //private const int BLOCK_56_VECTOR = 0x0C;
        //private const int BLOCK_56_VECTOR = 0x0E;
        //private const int BLOCK_56_VECTOR = 0x10;
        //private const int BLOCK_56_VECTOR = 0x12;
        //private const int BLOCK_56_VECTOR = 0x14;
        //private const int BLOCK_56_VECTOR = 0x16;
        //private const int BLOCK_56_VECTOR = 0x18;
        //private const int BLOCK_56_VECTOR = 0x1A;
        //private const int BLOCK_56_VECTOR = 0x1C;
        //private const int BLOCK_56_VECTOR = 0x1E;
        //private const int BLOCK_56_VECTOR = 0x20;
        #endregion

        public Block55(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x55;
            Description = "Personality Vector";
            LongChecksum = true;

            Block56 = new Block56(this, BLOCK_56_VECTOR);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block56.ToString());

            return sb.ToString();
        }
    }
}
