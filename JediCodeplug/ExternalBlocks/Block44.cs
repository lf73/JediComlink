using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block44 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x44; }
        public override string Description { get => "MDC Configuration"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 04 2F 04 55  04 5B
        */

        private const int BLOCK_47_VECTOR = 0x00;
        private const int BLOCK_9F_VECTOR = 0x02;
        private const int BLOCK_92_VECTOR = 0x04;
        #endregion

        #region Propeties
        public Block47 Block47 { get; set; }
        public Block9F Block9F { get; set; }
        public Block92 Block92 { get; set; }
        #endregion

        public Block44() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block47 = Deserialize<Block47>(Contents, BLOCK_47_VECTOR, codeplugContents);
            Block9F = Deserialize<Block9F>(Contents, BLOCK_9F_VECTOR, codeplugContents);
            Block92 = Deserialize<Block92>(Contents, BLOCK_92_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block47, BLOCK_47_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block9F, BLOCK_9F_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block92, BLOCK_92_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
