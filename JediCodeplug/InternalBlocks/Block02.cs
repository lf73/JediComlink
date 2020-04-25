using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block02 : Block
    {
        public override byte Id { get => 0x02; }
        public override string Description { get => "Hardware Config Radio"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 71 01 28  01 5E 10 00   00 07 21 3F  8A 60 FF 00
        1: 28 28 00 8E  4B 00 03 07   0F AB BC 02  18 64 28 01
        2: 01 7F 00 93  00 07 FF 02   28 28 02 01  8E 49 67 50
        3: 65 XX
        */
        
        private const int CONTENTS_LENGTH = 0x31; //Or sometimes 0x32 for VHF
        private const int BLOCK_06_VECTOR = 0x00; //01
        private const int BLOCK_03_VECTOR = 0x02; //03
        private const int BLOCK_0C_VECTOR = 0x04; //05
        private const int UNKNOWN1 = 0x06; //07 08 09 0A 0B 0C 0D 0E 0F 10 11 12
        private const int MIC_GAIN = 0x13;
        private const int UNKNOWN2 = 0x14; //15 16 17 18 19 1A 1B 1C
        private const int RSSI = 0x1D;
        private const int UNKNOWN3 = 0x1E; //1F
        private const int BLOCK_0F_VECTOR = 0x20; //21
        private const int UNKNOWN4 = 0x22; //23 24 25 26 27 28 29 2A
        private const int BLOCK_11_VECTOR = 0x2B; //2C
        private const int UNKNOWN5 = 0x2D; //2E 2F 30
        private const int UNKNOWNEXTRABYTE = 0x31; //2E 2F 30
        #endregion

        #region Propeties

        public Block06 Block06 { get; set; }

        public Block03 Block03 { get; set; }

        public Block0C Block0C { get; set; }

        [DisplayName("Unknown Byte Values 1")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown1 { get; set; }

        [DisplayName("Internal Mic Pre-Amp Gain")]
        [Description("Range 0 to 7")]
        public int InternalMicPreAmpGain { get; set; }

        [DisplayName("External Mic Pre-Amp Gain")]
        [Description("Range 0 to 7")]
        public int ExternalMicPreAmpGain { get; set; }

        [DisplayName("Unknown Mic Bits")]
        [Description("Probably Not Used")]
        public int UnknownMicBits { get; set; }

        [DisplayName("Unknown Byte Values 2")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown2 { get; set; }

        [DisplayName("RSSI Alignment")]
        [Description("N/A on 800mhz Models")]
        public int RssiAlignment { get; set; }

        [DisplayName("Unknown Byte Values 3")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown3 { get; set; }

        public Block0F Block0F { get; set; }

        [DisplayName("Unknown Byte Values 4")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown4 { get; set; }

        public Block11 Block11 { get; set; }

        [DisplayName("Unknown Byte Values 5")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown5 { get; set; }

        [DisplayName("Unknown Extra Byte Value")]
        [TypeConverter(typeof(HexByteValueTypeConverter))]
        public byte UnknownExtraByte { get; set; }

        [DisplayName("Has Extra Unknown Byte")]
        [Description("VHF Version returns an addition byte for some reason. To do: Undserstand why")]
        public bool HasExtraByte { get; set; }
        #endregion

        public Block02() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);

            Unknown1 = contents.Slice(UNKNOWN1, 13).ToArray();
            Unknown2 = contents.Slice(UNKNOWN2, 9).ToArray();
            RssiAlignment = contents[RSSI];
            Unknown3 = contents.Slice(UNKNOWN3, 2).ToArray();
            InternalMicPreAmpGain = (contents[MIC_GAIN] & 0b11100000) >> 5;
            ExternalMicPreAmpGain = (contents[MIC_GAIN] & 0b00011100) >> 2;
            UnknownMicBits = contents[MIC_GAIN] & 0b11; //Seems to be set but not sure why.
            Unknown4 = contents.Slice(UNKNOWN4, 9).ToArray(); 
            Unknown5 = contents.Slice(UNKNOWN5, 4).ToArray();

            if(contents.Length > UNKNOWNEXTRABYTE)
            {
                HasExtraByte = true;
                UnknownExtraByte = contents[UNKNOWNEXTRABYTE];
            }

            Block06 = Deserialize<Block06>(contents, BLOCK_06_VECTOR, codeplugContents);
            Block03 = Deserialize<Block03>(contents, BLOCK_03_VECTOR, codeplugContents);
            Block0C = Deserialize<Block0C>(contents, BLOCK_0C_VECTOR, codeplugContents);
            Block0F = Deserialize<Block0F>(contents, BLOCK_0F_VECTOR, codeplugContents);
            Block11 = Deserialize<Block11>(contents, BLOCK_11_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH + (HasExtraByte ? 1 : 0)].AsSpan();
            Unknown1.CopyTo(contents.Slice(UNKNOWN1, 13));
            Unknown2.CopyTo(contents.Slice(UNKNOWN2, 9));
            contents[RSSI] = (byte)(RssiAlignment & 0b01111111);
            Unknown3.CopyTo(contents.Slice(UNKNOWN3, 2));
            contents[MIC_GAIN] = (byte)((InternalMicPreAmpGain & 0b111) << 5);
            contents[MIC_GAIN] |= (byte)((ExternalMicPreAmpGain & 0b111) << 2);
            contents[MIC_GAIN] |= (byte)(UnknownMicBits & 0b11);
            Unknown4.CopyTo(contents.Slice(UNKNOWN4, 9));
            Unknown5.CopyTo(contents.Slice(UNKNOWN5, 4));
            if (HasExtraByte) contents[UNKNOWNEXTRABYTE] = UnknownExtraByte;

            var nextAddress = address + contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block06, BLOCK_06_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block03, BLOCK_03_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block0C, BLOCK_0C_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block0F, BLOCK_0F_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block11, BLOCK_11_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
