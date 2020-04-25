using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block09 : Block
    {
        public override byte Id { get => 0x09; }
        public override string Description { get => "Softpot Interpol"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 35 E8 32 AF  00 00 4F 50   50 50 3F 2C  28
        */
        private const int CONTENTS_LENGTH = 0xD;
        private const int UNKNOWN1 = 0x00;
        private const int UNKNOWN2 = 0x01;
        private const int TX_DEV_BALANCE = 0x02;
        private const int TX_DEV_LIMIT = 0x03;
        private const int RX_BANDPASS = 0x04;
        private const int UNKNOWN3 = 0x05;
        private const int TX_POWER_HIGH = 0x06;
        private const int UNKNOWN4 = 0x07;
        private const int UNKNOWN5 = 0x08;
        private const int TX_POWER_LOW = 0x09;
        private const int RX_SQ_ATT_12_5 = 0x0A;
        private const int RX_SQ_ATT_20_0 = 0x0B;
        private const int RX_SQ_ATT_25_0 = 0x0C;
        #endregion

        #region Propeties
        [DisplayName("Unknown Bytes Value 1")]
        public byte Unknown1 { get; set; }

        [DisplayName("Unknown Bytes Value 2")]
        public byte Unknown2 { get; set; }

        [DisplayName("TX Deviation Balance Compensation")]
        public byte TxDeviationBalanceCompensation { get; set; }

        [DisplayName("TX Deviation Limit")]
        public byte TxDeviationLimit { get; set; }

        [DisplayName("RX Front End Bandpass Filter")]
        public byte RxFrontEndBandpassFilter { get; set; }

        [DisplayName("Unknown Bytes Value 3")]
        public byte Unknown3 { get; set; }

        [DisplayName("TX Power High")]
        public byte TxPowerHigh { get; set; }

        [DisplayName("Unknown Bytes Value 4")]
        public byte Unknown4 { get; set; }

        [DisplayName("Unknown Bytes Value 5")]
        public byte Unknown5 { get; set; }

        [DisplayName("TX Power Low")]
        public byte TxPowerLow { get; set; }

        [DisplayName("Squelch Attenuator 12.5Khz")]
        public byte RxSquelchAttenuator12k5 { get; set; }  //Unsed on 800mhz

        [DisplayName("Squelch Attenuator 20khz")]
        public byte RxSquelchAttenuator20k0 { get; set; }

        [DisplayName("Squelch Attenuator 25/30Khz")]
        public byte RxSquelchAttenuator25k0 { get; set; }
        #endregion

        public Block09() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);

            Unknown1 = contents[UNKNOWN1];
            Unknown2 = contents[UNKNOWN2];
            TxDeviationBalanceCompensation = contents[TX_DEV_BALANCE];
            TxDeviationLimit = contents[TX_DEV_LIMIT];
            RxFrontEndBandpassFilter = contents[RX_BANDPASS];
            Unknown3 = contents[UNKNOWN3];
            TxPowerHigh = contents[TX_POWER_HIGH];
            Unknown4 = contents[UNKNOWN4];
            Unknown5 = contents[UNKNOWN5];
            TxPowerLow = contents[TX_POWER_LOW];
            RxSquelchAttenuator12k5 = contents[RX_SQ_ATT_12_5];
            RxSquelchAttenuator20k0 = contents[RX_SQ_ATT_20_0];
            RxSquelchAttenuator25k0 = contents[RX_SQ_ATT_25_0];
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            contents[UNKNOWN1] = Unknown1;
            contents[UNKNOWN2] = Unknown2;
            contents[TX_DEV_BALANCE] = TxDeviationBalanceCompensation;
            contents[TX_DEV_LIMIT] = TxDeviationLimit;
            contents[RX_BANDPASS] = RxFrontEndBandpassFilter;
            contents[UNKNOWN3] = Unknown3;
            contents[TX_POWER_HIGH] = TxPowerHigh;
            contents[UNKNOWN4] = Unknown4;
            contents[UNKNOWN5] = Unknown5;
            contents[TX_POWER_LOW] = TxPowerLow;
            contents[RX_SQ_ATT_12_5] = RxSquelchAttenuator12k5;
            contents[RX_SQ_ATT_20_0] = RxSquelchAttenuator20k0;
            contents[RX_SQ_ATT_25_0] = RxSquelchAttenuator25k0;
            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
