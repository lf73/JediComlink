using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block0C : Block
    {
        public override int Id { get => 0x0C; }
        public override string Description { get => "Unknown"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 67 4C E7 A7  7A 02 00 22   01 86 00 1E  01 0E 00 20
        1: 01 AE 00 22  01 38 00 1E   00 D8 00 20  01 58
        */

        private const int CONTENTS_LENGTH = 0x1E;
        private const int UNKNOWN1 = 0x00; //thru 0x1D
        #endregion

        #region Propeties
        [DisplayName("Unknown Byte Values 1")]
        public byte[] Unknown1 { get; set; }
        #endregion

        public Block0C() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            Unknown1 = contents.Slice(UNKNOWN1, 0x1E).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
