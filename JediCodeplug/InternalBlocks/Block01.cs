using System;
using System.ComponentModel;
using System.Text;

namespace JediCodeplug
{
    public class Block01 : Block
    {
        public override int Id { get => 0x01; }
        public override string Description { get => "Internal Radio"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 02 00 34 36  36 41 57 41   32 38 36 37  48 30 31 55
        1: 43 44 36 50  57 31 42 4E   00 00 00 00  00 0F 02 00
        2: 00 01 00 00  00 3D 01 99   00 00 00 00  01 DC 01 00
        3: 03 22 BB C8  89 54 02 69   30 B0 
        */

        private const int CONTENTS_LENGTH = 0x3A;
        private const int EXTERNAL_CODEPLUG_VECTOR = 0x00; //01
        private const int SERIAL = 0x02; //03 04 05 06 07 08 09 0A 0B
        private const int MODEL = 0x0C; //0D 0E 0F 10 11 12 13 14 15 16 17 18 19 1A 1B
        private const int CODEPLUG_VERSION = 0x1C; //1D
        private const int INTERNAL_CODEPLUG_SIZE = 0x1E; //1F
        private const int UNKNOWN1 = 0x20; //21 22 23
        private const int BLOCK_02_VECTOR = 0x24; //25
        private const int BLOCK_56_VECTOR = 0x26; //27
        private const int UNKNOWN2 = 0x28; //29 2A 2B
        private const int BLOCK_10_VECTOR = 0x2C; //2D
        private const int UNKNOWN3 = 0x2e; //2F
        private const int AUTH_CODE = 0x30; //31 32 33 34 35 36 37 38 39
        #endregion

        #region Propeties
        [DisplayName("External Codeplug Vector")]
        public int ExternalCodeplugVector { get; set; }

        public string Serial { get; set; }

        public string Model { get; set; }

        [DisplayName("Codeplug Version")]
        public int CodeplugVersion { get; set; }

        [DisplayName("Internal Codeplug Size")]
        public int InternalCodeplugSize { get; set; }

        [DisplayName("Unknown Byte Values 1")]
        public byte[] Unknown1 { get; set; }

        public Block02 Block02 { get; set; }

        public Block56 Block56 { get; set; }

        [DisplayName("Unknown Byte Values 2")]
        public byte[] Unknown2 { get; set; }

        public Block10 Block10 { get; set; }

        [DisplayName("Unknown Byte Values 3")]
        public byte[] Unknown3 { get; set; }

        [DisplayName("Auth Code")]
        public byte[] AuthCode { get; set; }
        #endregion

        public Block01() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);

            ExternalCodeplugVector = contents[EXTERNAL_CODEPLUG_VECTOR] * 0x100 + contents[EXTERNAL_CODEPLUG_VECTOR + 1];
            Serial = GetStringContents(contents, SERIAL, 10);
            Model = GetStringContents(contents, MODEL, 16);
            CodeplugVersion = contents[CODEPLUG_VERSION] * 0x100 + contents[CODEPLUG_VERSION + 1];
            InternalCodeplugSize = contents[INTERNAL_CODEPLUG_SIZE] * 0x100 + contents[INTERNAL_CODEPLUG_SIZE + 1];
            Unknown1 = contents.Slice(UNKNOWN1, 4).ToArray();
            Block02 = Deserialize<Block02>(contents, BLOCK_02_VECTOR, codeplugContents);
            Block56 = Deserialize<Block56>(contents, BLOCK_56_VECTOR, codeplugContents);
            Unknown2 = contents.Slice(UNKNOWN2, 4).ToArray();
            Block10 = Deserialize<Block10>(contents, BLOCK_10_VECTOR, codeplugContents);
            Unknown3 = contents.Slice(UNKNOWN3, 2).ToArray();
            AuthCode = contents.Slice(AUTH_CODE, 10).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            var nextAddress = address + CONTENTS_LENGTH + BlockSizeAdjustment;
            contents[EXTERNAL_CODEPLUG_VECTOR] = (byte)(ExternalCodeplugVector / 0x100);
            contents[EXTERNAL_CODEPLUG_VECTOR + 1] = (byte)(ExternalCodeplugVector % 0x100);
            Encoding.ASCII.GetBytes(Serial).AsSpan().CopyTo(contents.Slice(SERIAL, 10));
            Encoding.ASCII.GetBytes(Model).AsSpan().CopyTo(contents.Slice(MODEL, 16));
            contents[CODEPLUG_VERSION] = (byte)(CodeplugVersion / 0x100);
            contents[CODEPLUG_VERSION + 1] = (byte)(CodeplugVersion % 0x100);
            contents[INTERNAL_CODEPLUG_SIZE] = (byte)(InternalCodeplugSize / 0x100);
            contents[INTERNAL_CODEPLUG_SIZE + 1] = (byte)(InternalCodeplugSize % 0x100);
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            nextAddress = SerializeChild(Block02, BLOCK_02_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block56, BLOCK_56_VECTOR, codeplugContents, nextAddress, contents);
            Unknown2.AsSpan().CopyTo(contents.Slice(UNKNOWN2));
            nextAddress = SerializeChild(Block10, BLOCK_10_VECTOR, codeplugContents, nextAddress, contents);
            Unknown3.AsSpan().CopyTo(contents.Slice(UNKNOWN3));
            AuthCode.AsSpan().CopyTo(contents.Slice(AUTH_CODE));
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
