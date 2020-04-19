using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block04 : Block
    {
        public override int Id { get => 0x04; }
        public override string Description { get => "Hardware Config Conventional"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 32 22 12 00  04 08 0C 14   1C 20 28
        */
        
        private const int CONTENTS_LENGTH = 0x0B;
        private const int UNKNOWN1 = 0x00; //01 02 03 04 05 06 07 08 09 0A
        #endregion

        #region Propeties
        [DisplayName("Unknown Byte Values 1")]
        public byte[] Unknown1 { get; set; }
        #endregion

        public Block04() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            Unknown1 = contents.Slice(UNKNOWN1, 11).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
