using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block51 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override byte Id { get => 0x51; }
        public override string Description { get => "Scan Configuration"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 08 29 68 03  14 06 00 00   06 0C 23 0B  0A 06 01 00
        1: 00
        */

        private const int BLOCK_52_VECTOR = 0x00;

        #endregion

        #region Propeties
        public Block52 Block52 { get; set; }
        #endregion



        public Block51() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block52 = Deserialize<Block52>(Contents, BLOCK_52_VECTOR, codeplugContents);

        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block52, BLOCK_52_VECTOR, codeplugContents, nextAddress, contents);

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
