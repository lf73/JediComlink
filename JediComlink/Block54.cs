using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block54 : BlockLong
    {
        public override int Id { get => 0x54; }
        public override string Description { get => "Zone Chan Assignment"; }

        #region Propeties
        public Block56 Block56 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 10 10 0F  0E 0D 0C 0B   0A 09 08 07  06 05 04 03
        1: 02 01
        */

        private const int BLOCK_56_VECTOR = 0x0C;
        #endregion

        public Block54(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block56 = new Block56(this, BLOCK_56_VECTOR, codeplugContents);
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
