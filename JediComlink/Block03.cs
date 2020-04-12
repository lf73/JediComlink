using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block03 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x03; }
        public override string Description { get => "HWConfig Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 31 01 3F  01 49
        */

        private const int BLOCK_04_VECTOR = 0x00; //01
        private const int BLOCK_05_VECTOR = 0x02; //03
        private const int BLOCK_0D_VECTOR = 0x04; //05
        #endregion

        #region Propeties
        public Block04 Block04 { get; set; }
        public Block05 Block05 { get; set; }
        public Block0D Block0D { get; set; }
        #endregion

        public Block03() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = GetContents(codeplugContents, address);
            Block04 = Deserialize<Block04>(Contents, BLOCK_04_VECTOR, codeplugContents);
            Block05 = Deserialize<Block05>(Contents, BLOCK_05_VECTOR, codeplugContents);
            Block0D = Deserialize<Block0D>(Contents, BLOCK_0D_VECTOR, codeplugContents);
        }
 
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            sb.AppendLine(Block04.ToString());
            sb.AppendLine(Block05.ToString());
            sb.AppendLine(Block0D.ToString());

            return sb.ToString();
        }
    }
}
