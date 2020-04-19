using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block0B : Block
    {
        public override int Id { get => 0x0B; }
        public override string Description { get => "Softpot B/W"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 45 A4 0C 96
        */

        private const int CONTENTS_LENGTH = 0x04;
        private const int UNKNOWN1 = 0x00; //01 02 03
        #endregion

        #region Propeties
        [DisplayName("Unknown Byte Values 1")]
        public byte[] Unknown1 { get; set; }
        #endregion

        public Block0B() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            Unknown1 = contents.Slice(UNKNOWN1, 4).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
