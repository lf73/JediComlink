using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block0E : Block
    {
        public override byte Id { get => 0x0E; }
        public override string Description { get => "Test Channel Table"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 03 22 1F  4A 08 C2 24   EA 0E FE 2B  16 1F 42 1F
        1: 4A 24 E2 24  EA 2B 1E 2B   16
        */

        // Byte 0 is number of channels
        // TT TT RR RR  Transmit/Receive
        // TTTT * 0.00625 + 801

        private const int COUNT = 0x00;
        private const int UNKNOWN1 = 0x01; // to 18
        #endregion

        #region Propeties
        public byte FreqCount { get; set; } //todo replace with list of freqs

        [DisplayName("Unknown Byte Value 1")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown1 { get; set; }
        #endregion

        public Block0E() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            FreqCount = contents[COUNT];
            Unknown1 = contents.Slice(UNKNOWN1, FreqCount * 4).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[Unknown1.Length + 1].AsSpan();
            contents[COUNT] = FreqCount;
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            var nextAddress = Serializer(codeplugContents, address, contents) + address;

            if (nextAddress == 0x01F4)
            {
                //Keep From Fragmenting next block between Internal and External EEPROM
                codeplugContents[0x01FE] = 0xC0; // Unknown Byte.  Maybe doesn't matter
                nextAddress = 0x200;
                //TODO Maybe this can be moved to to superclass?
            }


            return nextAddress;
        }
    }
}
