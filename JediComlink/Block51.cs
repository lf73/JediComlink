using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block51 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x51; }
        public override string Description { get => "Scan Configuration"; }

        #region Propeties
        public Block52 Block52 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 08 29 68 03  14 06 00 00   06 0C 23 0B  0A 06 01 00
        1: 00
        */

        private const int BLOCK_52_VECTOR = 0x00;
        #endregion

        public Block51() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = GetContents(codeplugContents, address);
            Block52 = Deserialize<Block52>(Contents, BLOCK_52_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block52.ToString());

            return sb.ToString();
        }
    }
}
