using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block5A : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x5A; }
        public override string Description { get => "Trunk System"; }

        #region Propeties
        public Block58 Block58 { get; set; }
        public Block61 Block61 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 02 1C 04 3D  04 56 95 64   1C 20 00 01  00 0C 00 00
        1: 00 00 00 00  00 00 00 00   00 00 04 64  00 00 00 01
        2: 00 26 01 26  01 26 01 26   01 00 00 00  00 00 00 00
        3: 00 00 00 05
        */

        private const int BLOCK_58_VECTOR = 0x02;
        private const int BLOCK_61_VECTOR = 0x04;
        #endregion

        public Block5A() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block58 = Deserialize<Block58>(Contents, BLOCK_58_VECTOR, codeplugContents);
            Block61 = Deserialize<Block61>(Contents, BLOCK_61_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block58, BLOCK_58_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block61, BLOCK_61_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
