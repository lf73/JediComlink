using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block31 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x31; }
        public override string Description { get => "Radio Wide"; }

        #region Propeties
        public Block90 Block90 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 14 03 28  1E 10 41 60   00 02 04 08  00 05 00 00
        1: 06 00 00 05  00 00 05 00   28 09 00 00  0A 0A 00 01
        2: 01 01 00 00  00 00 00 28   00 00 00 00  00 00 00 00
        3: 00 00 00 00  00 00
        */

        private const int BLOCK_90_VECTOR = 0x02;
        #endregion

        public Block31() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block90 = Deserialize<Block90>(Contents, BLOCK_90_VECTOR, codeplugContents);

        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block90, BLOCK_90_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block90.ToString());

            return sb.ToString();
        }
    }
}
