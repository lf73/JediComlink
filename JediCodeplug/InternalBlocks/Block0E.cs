using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block0E : Block
    {
        public override int Id { get => 0x0E; }
        public override string Description { get => "Test Channel Table"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 03 22 1F  4A 08 C2 24   EA 0E FE 2B  16 1F 42 1F
        1: 4A 24 E2 24  EA 2B 1E 2B   16
        */

        // Byte 0 is number of channels
        // TT TT RR RR  Transmit/Receive
        // TTTT * 0.00625 + 801

        private const int UNKNOWN1 = 0x00; // to 18
        #endregion

        #region Propeties
        [DisplayName("Unknown Byte Value 1")]
        public byte[] Unknown1 { get; set; }
        #endregion

        public Block0E() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            Unknown1 = contents.Slice(UNKNOWN1, 0x12).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[Unknown1.Length].AsSpan();
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
