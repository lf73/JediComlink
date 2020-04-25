using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block3D : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override byte Id { get => 0x3D; }
        public override string Description { get => "Signaling Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 03 1B 04 26  00 00 00 00   00 00 00 00  04 72
        */

        private const int BLOCK_3E_VECTOR = 0x00;
        private const int BLOCK_44_VECTOR = 0x02;
        private const int BLOCK_4A_VECTOR = 0x04;
        private const int BLOCK_A0_VECTOR = 0x0C;
        #endregion

        #region Propeties
        public Block3E Block3E { get; set; }
        public Block44 Block44 { get; set; }
        public Block4A Block4A { get; set; }
        public byte[] UnknownPointer2 { get; set; }
        public byte[] UnknownPointer3 { get; set; }
        public byte[] UnknownPointer4 { get; set; }
        public BlockA0 BlockA0 { get; set; }
        #endregion

        public Block3D() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block3E = Deserialize<Block3E>(Contents, BLOCK_3E_VECTOR, codeplugContents);
            Block44 = Deserialize<Block44>(Contents, BLOCK_44_VECTOR, codeplugContents);
            Block4A = Deserialize<Block4A>(Contents, BLOCK_4A_VECTOR, codeplugContents);
            UnknownPointer2 = Contents.Slice(0x06, 2).ToArray();
            UnknownPointer3 = Contents.Slice(0x08, 2).ToArray();
            UnknownPointer4 = Contents.Slice(0x0A, 2).ToArray();
            BlockA0 = Deserialize<BlockA0>(Contents, BLOCK_A0_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block3E, BLOCK_3E_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block44, BLOCK_44_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block4A, BLOCK_4A_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(BlockA0, BLOCK_A0_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
