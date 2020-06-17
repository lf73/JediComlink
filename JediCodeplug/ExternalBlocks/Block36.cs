using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block36 : Block
    {
        public override byte Id { get => 0x36; }
        public override string Description { get => "Display & Menu"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 04 CE 00 06  14 0A 03 28   28 07 20 00  08 00 00 00
        1: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        2: 00 00 00 00  00 00 00
        */

        private const int CONTENTS_LENGTH = 0x27;
        private const int BLOCK_37_VECTOR = 0x00;
        private const int UNKNOWN1 = 0x02; //thru 0x26
        #endregion

        #region Propeties
        public Block37 Block37 { get; set; }

        [DisplayName("Unknown Byte Values 1")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown1 { get; set; }
        #endregion

        public Block36() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            Unknown1 = contents.Slice(UNKNOWN1, CONTENTS_LENGTH-2).ToArray();

            Block37 = Deserialize<Block37>(contents, BLOCK_37_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            var nextAddress = address + CONTENTS_LENGTH + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block37, BLOCK_37_VECTOR, codeplugContents, nextAddress, contents);
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
