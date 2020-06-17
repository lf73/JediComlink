using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block3F : Block
    {
        public override byte Id { get => 0x3F; }
        public override string Description { get => "Control Head"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 05 05 03 23  04 1D 60 18   61 23 62 23  26 26 26 48
        1: 26 26 26
        */

        private const int CONTENTS_LENGTH = 0x13;
        private const int UNKNOWN1 = 0x00; //thru 0x12
        #endregion

        #region Propeties
        [DisplayName("Unknown Byte Values 1")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown1 { get; set; }
        #endregion

        public Block3F() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            Unknown1 = contents.Slice(UNKNOWN1, CONTENTS_LENGTH).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
