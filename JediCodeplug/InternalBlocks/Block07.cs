using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block07 : Block
    {
        public override byte Id { get => 0x07; }
        public override string Description { get => "Softpot Radio"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 59 12 11 18  15 00 70 00   48 4D 4D 4D  03 86 06 8C
        1: CB 00 6E 51  XX
        */

        private const int CONTENTS_LENGTH = 0x14;  //SOMETIMES 0x15
        private const int REF_OSC = 0x00;
        private const int DTMF = 0x01;
        private const int HIGH_SPEED = 0x02;
        private const int MDC_1200 = 0x03;
        private const int UNKNOWN1 = 0x04;
        private const int TX_DEV_REF_12_5 = 0x05;
        private const int TX_DEV_REF_20_0 = 0x06;
        private const int UNKNOWN2 = 0x07; //08 09 0A 0B 0C 0D 0E 0F
        private const int RX_RATED_AUDIO = 0x10;
        private const int UNKNOWN3 = 0x11;
        private const int TX_SECURE_DEV = 0x12;
        private const int RX_SECURE_DISC = 0x13;
        private const int UNKNOWNEXTRABYTE = 0x14;
        #endregion

        #region Propeties
        [DisplayName("TX Reference Oscillator")]
        public byte TxReferenceOscillator { get; set; }

        [DisplayName("Signaling DTMF")]
        [Description("Range 0 to 31")]
        public byte SignalingDtmf { get; set; }

        [DisplayName("Signaling High Speed")]
        [Description("Range 0 to 31")]
        public byte SignalingHighSpeed { get; set; }

        [DisplayName("Signaling MDC1200")]
        [Description("Range 0 to 31")]
        public byte SignalingMdc1200 { get; set; }

        [DisplayName("Unknown Byte Value 1")]
        public byte Unknown1 { get; set; }

        [DisplayName("TX Deviation Reference 12.5khz")]
        [Description("Range 0 to 127. N/A on 800mhz")]
        public byte TxDeviationReference12k5 { get; set; }

        [DisplayName("TX Deviation Reference 20.0khz")]
        [Description("Range 0 to 127")]
        public byte TxDeviationReference20k0 { get; set; }

        [DisplayName("Unknown Byte Values 2")]
        public byte[] Unknown2 { get; set; }

        [DisplayName("RX Rated Audio Calibration")]
        public byte RxRatedAudioCalibration { get; set; }

        [DisplayName("Unknown Byte Value 3")]
        public byte Unknown3 { get; set; }

        [DisplayName("TX Secure Deviation")]
        [Description("Range 0 to 127")]
        public byte TxSecureDeviation { get; set; }

        [DisplayName("RX Secure Discriminator")]
        [Description("Range 0 to 127")]
        public byte RxSecureDiscriminator { get; set; }

        [DisplayName("Unknown Extra Byte Value")]
        [TypeConverter(typeof(HexByteValueTypeConverter))]
        public byte UnknownExtraByte { get; set; }

        [DisplayName("Has Extra Unknown Byte")]
        [Description("VHF Version returns an addition byte for some reason. To do: Undserstand why")]
        public bool HasExtraByte { get; set; }
        #endregion

        public Block07() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            TxReferenceOscillator = (byte)(~contents[REF_OSC]);
            SignalingDtmf = contents[DTMF];
            SignalingHighSpeed = contents[HIGH_SPEED];
            SignalingMdc1200 = contents[MDC_1200];
            Unknown1 = contents[UNKNOWN1];
            TxDeviationReference12k5 = contents[TX_DEV_REF_12_5];
            TxDeviationReference20k0 = contents[TX_DEV_REF_20_0];
            Unknown2 = contents.Slice(UNKNOWN2, 9).ToArray();
            RxRatedAudioCalibration = contents[RX_RATED_AUDIO];
            Unknown3 = contents[UNKNOWN3];
            TxSecureDeviation = contents[TX_SECURE_DEV];
            RxSecureDiscriminator = contents[RX_SECURE_DISC];

            if (contents.Length > UNKNOWNEXTRABYTE)
            {
                HasExtraByte = true;
                UnknownExtraByte = contents[UNKNOWNEXTRABYTE];
            }

        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH + (HasExtraByte ? 1 : 0)].AsSpan();
            contents[REF_OSC] = (byte)(~TxReferenceOscillator);
            contents[DTMF] = SignalingDtmf;
            contents[HIGH_SPEED] = SignalingHighSpeed;
            contents[MDC_1200] = SignalingMdc1200;
            contents[UNKNOWN1] = Unknown1;
            contents[TX_DEV_REF_12_5] = TxDeviationReference12k5;
            contents[TX_DEV_REF_20_0] = TxDeviationReference20k0;
            Unknown2.AsSpan().CopyTo(contents.Slice(UNKNOWN2));
            contents[RX_RATED_AUDIO] = RxRatedAudioCalibration;
            contents[UNKNOWN3] = Unknown3;
            contents[TX_SECURE_DEV] = TxSecureDeviation;
            contents[RX_SECURE_DISC] = RxSecureDiscriminator;
            if (HasExtraByte) contents[UNKNOWNEXTRABYTE] = UnknownExtraByte;

            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
