using System;
using System.Text;

namespace JediCodeplug
{
    public class Block0F : Block
    {
        public override int Id { get => 0x0F; }
        public override string Description { get => "AD Switch Level"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 27 10 08 FF  FF 00 27 10   46 FF FF 00
        */
        private const int CONTENTS_LENGTH = 0xC;
        private const int UNKNOWN1 = 0x00; //01 02 03 04 05
        private const int TX_VCO = 0x06; //07
        private const int UNKNOWN2 = 0x08;
        private const int RX_VCO = 0x09; //0A
        private const int UNKNOWN3 = 0x0B;

        #endregion

        #region Propeties
        public byte[] Unknown1 { get; set; }
        public decimal TxVcoCrossover { get; set; } //Unsed on 800mhz
        public byte Unknown2 { get; set; }
        public decimal RxVcoCrossover { get; set; } //Unsed on 800mhz
        public byte Unknown3 { get; set; }
        #endregion

        public Block0F() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            Unknown1 = contents.Slice(UNKNOWN1, 6).ToArray();
            TxVcoCrossover = MapFrequency(contents.Slice(TX_VCO, 2));
            Unknown2 = contents[UNKNOWN2];
            RxVcoCrossover = MapFrequency(contents.Slice(RX_VCO, 2));
            Unknown3 = contents[UNKNOWN3];
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            Unknown1.CopyTo(contents.Slice(UNKNOWN1, 13));
            MapFrequency(TxVcoCrossover).CopyTo(contents.Slice(TX_VCO, 2));
            contents[UNKNOWN2] = Unknown2;
            MapFrequency(RxVcoCrossover).CopyTo(contents.Slice(RX_VCO, 2));
            contents[UNKNOWN3] = Unknown3;
            return Serializer(codeplugContents, address, contents) + address;
        }

        private static Span<byte> MapFrequency(decimal freq)
        {
            byte msb = 0;
            byte lsb = 0;

            return new byte[] { msb, lsb };
        }

        private static decimal MapFrequency(Span<byte> freq)
        {
            decimal offset = freq[0] * 0x100 + freq[1];
            return 103.0m + (offset * .005m);
        }
    }
}
