using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block4A : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x4A; }
        public override string Description { get => "Trunk Configuration"; }

        #region Propeties
        public Block3F Block3F { get; set; }
        public Block41 Block41 { get; set; }
        public Block4B Block4B { get; set; }
        public Block57 Block57 { get; set; }
        public Block74 Block74 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 03 CD 03 E3  03 E9 05 00   41 01 04 00  65 04 84 00
        1: 00 30 0A 95  80 00 00 00   00 4A 3C 14  14 00 00 00
        2: 00 00 00 00
        */

        private const int BLOCK_3F_VECTOR = 0x00;
        private const int BLOCK_41_VECTOR = 0x02;
        private const int BLOCK_57_VECTOR = 0x04;
        private const int BLOCK_4B_VECTOR = 0x0A;
        private const int BLOCK_74_VECTOR = 0x0D;
        #endregion

        public Block4A() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block3F = Deserialize<Block3F>(Contents, BLOCK_3F_VECTOR, codeplugContents);
            Block41 = Deserialize<Block41>(Contents, BLOCK_41_VECTOR, codeplugContents);
            Block57 = Deserialize<Block57>(Contents, BLOCK_57_VECTOR, codeplugContents);
            Block4B = Deserialize<Block4B>(Contents, BLOCK_4B_VECTOR, codeplugContents);
            Block74 = Deserialize<Block74>(Contents, BLOCK_74_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block3F, BLOCK_3F_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block41, BLOCK_41_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block57, BLOCK_57_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block4B, BLOCK_4B_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block74, BLOCK_74_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
