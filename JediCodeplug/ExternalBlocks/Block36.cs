using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block36 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x36; }
        public override string Description { get => "Display & Menu"; }

        #region Propeties
        public Block37 Block37 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 04 CE 00 06  14 0A 03 28   28 07 20 00  08 00 00 00
        1: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        2: 00 00 00 00  00 00 00
        */

        private const int BLOCK_37_VECTOR = 0x00;
        #endregion

        public Block36() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block37 = Deserialize<Block37>(Contents, BLOCK_37_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block37, BLOCK_37_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
