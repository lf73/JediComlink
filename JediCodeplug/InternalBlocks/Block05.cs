using System;
using System.Text;

namespace JediCodeplug
{
    public class Block05 : Block
    {
        public override int Id { get => 0x05; }
        public override string Description { get => "HWConfig Secure"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 3D 46 11  1E 00 1D
        */

        private const int CONTENTS_LENGTH = 7;
        private const int UNKNOWN1 = 0x00;
        private const int MIC_GAIN = 0x01;
        private const int UNKNOWN2 = 0x02;
        private const int UNKNOWN3 = 0x03;
        private const int UNKNOWN4 = 0x04;
        private const int UNKNOWN5 = 0x05;
        private const int UNKNOWN6 = 0x06;
        #endregion

        #region Propeties
        public int Unknown1 { get; set; }
        public int UnknownMicBits { get; set; }
        public int InternalMicPreAmpGain { get; set; } //0 to 7
        public int ExternalMicPreAmpGain { get; set; } //0 to 7
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }



        #endregion

        public Block05() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);

            Unknown1 = contents[UNKNOWN1];
            UnknownMicBits = contents[MIC_GAIN] & 0b11000000; //Most likely these bits are never set.
            InternalMicPreAmpGain = (contents[MIC_GAIN] & 0b00111000) >> 3;
            ExternalMicPreAmpGain = contents[MIC_GAIN] & 0b00000111;
            Unknown2 = contents[UNKNOWN2];
            Unknown3 = contents[UNKNOWN3];
            Unknown4 = contents[UNKNOWN4];
            Unknown5 = contents[UNKNOWN5];
            Unknown6 = contents[UNKNOWN6];
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            contents[UNKNOWN1] = (byte)Unknown1;
            contents[MIC_GAIN] = (byte)(UnknownMicBits & 0b11000000);
            contents[MIC_GAIN] |= (byte)((InternalMicPreAmpGain & 0b111) << 3);
            contents[MIC_GAIN] |= (byte)(ExternalMicPreAmpGain & 0b111);
            contents[UNKNOWN2] = (byte)Unknown2;
            contents[UNKNOWN3] = (byte)Unknown3;
            contents[UNKNOWN4] = (byte)Unknown4;
            contents[UNKNOWN5] = (byte)Unknown5;
            contents[UNKNOWN6] = (byte)Unknown6;

            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
