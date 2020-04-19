using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block11 : Block
    {
        public override int Id { get => 0x11; }
        public override string Description { get => "AD Button Level"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 1F 30 70  85 CF 55 AA
        */

        private const int CONTENTS_LENGTH = 0x08;
        private const int UNKNOWN1 = 0x00; //01 02 03 04 05 06 07
        #endregion

        #region Propeties
        [DisplayName("Unknown Byte Values 1")]
        public byte[] Unknown1 { get; set; }
        #endregion

        public Block11() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            Unknown1 = contents.Slice(UNKNOWN1, 0x08).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
