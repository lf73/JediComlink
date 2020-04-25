using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class BlockA0 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override byte Id { get => 0xA0; }
        public override string Description { get => "Aux Signalling"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 04 7F 04 98  04 A4 00 00   04 AC
        */

        private const int BLOCK_A4_VECTOR = 0x00;
        private const int BLOCK_A1_VECTOR = 0x02;
        private const int BLOCK_A3_VECTOR = 0x04;
        private const int BLOCK_A9_VECTOR = 0x08;
        #endregion

        #region Propeties
        public BlockA4 BlockA4 { get; set; }
        public BlockA1 BlockA1 { get; set; }
        public BlockA3 BlockA3 { get; set; }
        public BlockA9 BlockA9 { get; set; }

        #endregion

        public BlockA0() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            BlockA4 = Deserialize<BlockA4>(Contents, BLOCK_A4_VECTOR, codeplugContents);
            BlockA1 = Deserialize<BlockA1>(Contents, BLOCK_A1_VECTOR, codeplugContents);
            BlockA3 = Deserialize<BlockA3>(Contents, BLOCK_A3_VECTOR, codeplugContents);
            BlockA9 = Deserialize<BlockA9>(Contents, BLOCK_A9_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(BlockA4, BLOCK_A4_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(BlockA1, BLOCK_A1_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(BlockA3, BLOCK_A3_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(BlockA9, BLOCK_A9_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
