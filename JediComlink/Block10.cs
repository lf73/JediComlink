using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block10 : Block
    {
        #region Propeties
        public string FlashCode { get; set; }

        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 19 07 10 19  7E F0 90 20   FB 00 80 40  00 00 40 00
        1: 00 00 00 80  00 00 00 00   50 30 06 00  00 04 00 00
        2: 00
        */

        private const int FLASHCODE = 0x1B;
        #endregion

        public Block10(Block parent, int vector) : base(parent, vector)
        {
            Id = 0x10;
            Description = "Feature Descriptor Block";
            LongChecksum = false;

            var flashCode = FormatHex(Contents.Slice(FLASHCODE, 6).ToArray()).Replace(" ", "");
            FlashCode = flashCode.Substring(0, 6) + '-' + flashCode.Substring(6);
        }

        public override string ToString()
        {
            var s = new String(' ', Level * 2);
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(s + $"FlashCode: {FlashCode}");
            return sb.ToString();
        }
    }
}
