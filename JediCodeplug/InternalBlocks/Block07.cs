using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block07 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x07; }
        public override string Description { get => "Softpot Radio"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 59 12 11 18  15 00 70 00   48 4D 4D 4D  03 86 06 8C
        1: CB 00 6E 51
        */

        private const int CONTENTS_LENGTH = 0x14;
        private const int REF_OSC = 0x00;
        private const int DTMF = 0x01;
        private const int HIGH_SPEED = 0x02;
        private const int MDC_1200 = 0x03;
        //private const int REF_OSC = 0x04;
        private const int TX_DEV_REF_12_5 = 0x05;
        private const int TX_DEV_REF_20_0 = 0x06;
        //private const int REF_OSC = 0x07;
        //private const int REF_OSC = 0x08;
        //private const int REF_OSC = 0x09;
        //private const int REF_OSC = 0x0A;
        //private const int REF_OSC = 0x0B;
        //private const int REF_OSC = 0x0C;
        //private const int REF_OSC = 0x0D;
        //private const int REF_OSC = 0x0E;
        //private const int REF_OSC = 0x0F;
        private const int RX_RATED_AUDIO = 0x10;
        //private const int REF_OSC = 0x11;
        private const int TX_SECURE_DEV = 0x12;
        private const int RX_SECURE_DISC = 0x13;
        #endregion


        #region Propeties

        public int TxReferenceOscillator { get; set; }
        public int SignalingDtmf { get; set; } //0 to 31
        public int SignalingHighSpeed { get; set; } //0 to 31
        public int SignalingMdc1200 { get; set; } //0 to 31
        public int TxDeviationReference12k5 { get; set; } //0 to 127  //Unsed on 800mhz
        public int TxDeviationReference20k0 { get; set; } //0 to 127
        public int RxRatedAudioCalibration { get; set; }
        public int TxSecureDeviation { get; set; } //0 to 127
        public int RxSecureDiscriminator { get; set; } //0 to 127

        #endregion



        public Block07() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            TxReferenceOscillator = (byte)(~Contents[REF_OSC]);
            SignalingDtmf = Contents[DTMF];
            SignalingHighSpeed = Contents[HIGH_SPEED];
            SignalingMdc1200 = Contents[MDC_1200];
            TxDeviationReference12k5 = Contents[TX_DEV_REF_12_5]; //N/A on 800mhz
            TxDeviationReference20k0 = Contents[TX_DEV_REF_20_0];
            RxRatedAudioCalibration = Contents[RX_RATED_AUDIO];
            TxSecureDeviation = Contents[TX_SECURE_DEV];
            RxSecureDiscriminator = Contents[RX_SECURE_DISC];
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            return Serializer(codeplugContents, address, contents) + address;
        }

        //public override string ToString()
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendLine(GetTextHeader());
        //    sb.AppendLine($"TX Reference Oscillator: {TxReferenceOscillator}");
        //    sb.AppendLine($"Signaling DTMF: {SignalingDtmf}");
        //    sb.AppendLine($"Signaling High Speed: {SignalingHighSpeed}");
        //    sb.AppendLine($"Signaling MDC1200: {SignalingMdc1200}");
        //    sb.AppendLine($"TX Deviation Reference 12.5khz: {TxDeviationReference12k5}");
        //    sb.AppendLine($"TX Deviation Reference 20.0khz: {TxDeviationReference20k0}");
        //    sb.AppendLine($"RX Rated Audio Calibration: {RxRatedAudioCalibration}");
        //    sb.AppendLine($"TX Secure Deviation: {TxSecureDeviation}");
        //    sb.AppendLine($"RX Secure Discriminator: {RxSecureDiscriminator}");
        //    //sb.AppendLine($"Unknown1 Bytes: {FormatHex(Unknown1)}");

        //    return sb.ToString();
        //}
    }
}
