using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block0E : Block
    {
        public override int Id { get => 0x0E; }
        public override string Description { get => "Test Channel Table"; }

        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 03 22 1F  4A 08 C2 24   EA 0E FE 2B  16 1F 42 1F
        1: 4A 24 E2 24  EA 2B 1E 2B   16
        */

        #endregion

        public Block0E(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
