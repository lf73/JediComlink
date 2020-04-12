using System;
using System.Text;

namespace JediComlink
{
    public class Block06 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x06; }
        public override string Description { get => "Softpot Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 7A 00 91  01 01
        */

        private const int BLOCK_07_VECTOR = 0x00; //01
        private const int BLOCK_08_VECTOR = 0x02; //03
        private const int BLOCK_0A_VECTOR = 0x04; //05
        #endregion

        #region Propeties
        public Block07 Block07 { get; set; }

        public Block08 Block08 { get; set; }

        public Block0A Block0A { get; set; }
        #endregion

        public Block06() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = GetContents(codeplugContents, address);
            Block07 = Deserialize<Block07>(Contents, BLOCK_07_VECTOR, codeplugContents);
            Block08 = Deserialize<Block08>(Contents, BLOCK_08_VECTOR, codeplugContents);
            Block0A = Deserialize<Block0A>(Contents, BLOCK_0A_VECTOR, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            sb.AppendLine(Block07.ToString());
            sb.AppendLine(Block08.ToString());
            sb.AppendLine(Block0A.ToString());

            return sb.ToString();
        }
    }
}
