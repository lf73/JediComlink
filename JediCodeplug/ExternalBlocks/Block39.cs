using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block39 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x39; }
        public override string Description { get => "Phone List Vector"; }

        #region Propeties
        public Block3A Block3A { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 09 36 00 14  FF FF FF FF   FF FF FF FF  FF FF FF FF
        1: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        2: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        3: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        4: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        5: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        6: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        7: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        8: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        9: FF FF FF FF  FF FF FF FF   FF FF FF FF  FF FF FF FF
        A: FF FF FF FF
        */

        private const int BLOCK_3A_VECTOR = 0x00;
        #endregion

        public Block39() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block3A = Deserialize<Block3A>(Contents, BLOCK_3A_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block3A, BLOCK_3A_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
