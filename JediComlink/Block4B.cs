using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block4B : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x4B; }
        public override string Description { get => "Trunk System Vector"; }

        #region Propeties
        public Block5A Block5A { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 04 06
        */

        private const int BLOCK_5A_VECTOR = 0x01;
        #endregion

        public Block4B() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = GetContents(codeplugContents, address);
            Block5A = Deserialize<Block5A>(Contents, BLOCK_5A_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block5A.ToString());

            return sb.ToString();
        }
    }
}
