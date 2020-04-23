using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block0D : Block
    {
        public override int Id { get => 0x0D; }
        public override string Description { get => "Hardware Config MDC"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 12 05 07 04  23 23 03 04   00 00 00 00  AA AA AA AA
        1: 01 06
        */

        private const int CONTENTS_LENGTH = 0x12;
        private const int UNKNOWN1 = 0x00; //01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F 10 11
        #endregion

        #region Propeties
        [DisplayName("Unknown Byte Values 1")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown1 { get; set; }
        #endregion

        public Block0D() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            Unknown1 = contents.Slice(UNKNOWN1, 0x12).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
