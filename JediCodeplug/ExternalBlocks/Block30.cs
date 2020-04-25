using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace JediCodeplug
{
    public class Block30 : Block
    {
        public override byte Id { get => 0x30; }
        public override string Description { get => "External Radio"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 3E 00 34 33  32 43 44 4E   30 30 30 32  48 30 31 4B
        1: 44 44 39 50  57 31 42 4E   00 00 00 00  11 02 24 14
        2: 42 03 00 1C  07 71 02 D1   03 0A 04 B7  05 1F 06 65
        3: 06 74 06 E4  07 16 08 36   08 3E 08 82  09 82 09 9B
        4: 00 00 00 00  00 00 00 00   00 00 09 AB  00 00
        */

        private const int CONTENTS_LENGTH = 0x4E;

        private const int UNKNOWN1 = 0x00; //01
        private const int SERIAL = 0x02; //03 04 05 06 07 08 09 0A 0B
        private const int MODEL = 0x0C; //0D 0E 0F 10 11 12 13 14 15 16 17 18 19 1A 1B
        private const int TIMESTAMP = 0x1c; //1D 1E 1F 20
        private const int PROGRAMMING_SOURCE = 0x21;
        private const int CODEPLUG_VERSION = 0x22; // 0x23
        private const int EXTERNAL_CODEPLUG_SIZE = 0x24; //25
        private const int BLOCK_31_VECTOR = 0x26; //27
        private const int BLOCK_3D_VECTOR = 0x28; //29
        private const int BLOCK_36_VECTOR = 0x2A; //2B
        private const int BLOCK_55_VECTOR = 0x2C; //2D
        private const int BLOCK_54_VECTOR = 0x2E; //2F
        private const int BLOCK_51_VECTOR = 0x30; //31
        private const int DYNAMIC_RADIO_VECTOR = 0x32; //33
        private const int BLOCK_39_VECTOR = 0x34; //35
        private const int BLOCK_3B_VECTOR = 0x36; //37
        private const int BLOCK_34_VECTOR = 0x38; //39
        private const int BLOCK_35_VECTOR = 0x3A; //3B
        private const int BLOCK_3C_VECTOR = 0x3C; //3D
        private const int BLOCK_73_VECTOR = 0x3E; //3F
        private const int UNKNOWN2 = 0x40; //41 42 43 44 45 46 47 48 49
        private const int DYNAMIC_MODE_SELECT_VECTOR = 0x4A;// 4B
        private const int UNKNOWN3 = 0x4C; //4D

        private const int DYNAMIC_RADIO_SIZE = 0x3A;
        private const int DYNAMIC_MODE_SELECT_SIZE = 0x4E;
        #endregion

        #region Propeties
        [DisplayName("Unknown Byte Values 1")]
        [Description("Values do not affect CPS but values are preserved by CPS. Maybe used by radio firmware?")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown1 { get; set; }

        public string Serial { get; set; }

        public string Model { get; set; }

        public DateTime TimeStamp { get; set; }

        [DisplayName("Programming Source")]
        public ProgrammingSources ProgrammingSource { get; set; }

        [DisplayName("External Codeplug Version")]
        [Description("CPS Version. 0x001D for R02.03.00. If value is higher, CPS will error that Codeplug is too new.")]
        [TypeConverter(typeof(HexIntValueTypeConverter))]
        public int ExternalCodeplugVersion { get; set; }

        [TypeConverter(typeof(HexIntValueTypeConverter))]
        public int ExternalCodeplugSize { get; set; }
 
        public Block31 Block31 { get; set; }
        public Block3D Block3D { get; set; }
        public Block36 Block36 { get; set; }
        public Block55 Block55 { get; set; }
        public Block54 Block54 { get; set; }
        public Block51 Block51 { get; set; }

        [TypeConverter(typeof(HexIntValueTypeConverter))]
        public int DynamicRadioVector { get; set; }

        public Block39 Block39 { get; set; }
        public Block3B Block3B { get; set; }
        public Block34 Block34 { get; set; }
        public Block35 Block35 { get; set; }
        public Block3C Block3C { get; set; }
        public Block73 Block73 { get; set; }

        [DisplayName("Unknown Byte Values 2")]
        [Description("Most likely not used. Values do not seem to affect CPS and are not preserved by CPS. CPS will reset to all to 0x00")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown2 { get; set; }

        [TypeConverter(typeof(HexIntValueTypeConverter))]
        public int DynamicModeSelectVector { get; set; }

        [DisplayName("Unknown Byte Values 3")]
        [Description("Most likely not used. Values do not seem to affect CPS and are not preserved by CPS. CPS will reset to all to 0x00")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown3 { get; set; }

        [DisplayName("Dynamic Radio")]
        [Description("This data is stored outside of a normal block definition. Perhaps this is reserved space for the radio to store user settings?")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] DynamicRadio { get; set; }

        [DisplayName("Dynamic Mode Select")]
        [Description("This data is stored outside of a normal block definition. Perhaps this is reserved space for the radio to store user settings?")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] DynamicModeSelect { get; set; }
        #endregion

        public Block30() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);

            Unknown1 = contents.Slice(UNKNOWN1, 2).ToArray();
            Serial = GetStringContents(contents, SERIAL, 10);
            Model = GetStringContents(contents, MODEL, 16);
            TimeStamp = new DateTime(2000 + GetDigits(contents[TIMESTAMP]),
                        GetDigits(contents[TIMESTAMP + 1]),
                        GetDigits(contents[TIMESTAMP + 2]),
                        GetDigits(contents[TIMESTAMP + 3]),
                        GetDigits(contents[TIMESTAMP + 4]),
                        0);
            if (typeof(ProgrammingSources).IsEnumDefined(contents[PROGRAMMING_SOURCE]))
            {
                ProgrammingSource = (ProgrammingSources)contents[PROGRAMMING_SOURCE];
            }
            else
            {
                ProgrammingSource = ProgrammingSources.CPS;
            }

            ExternalCodeplugVersion = contents[CODEPLUG_VERSION] * 0x100 + contents[CODEPLUG_VERSION + 1];
            ExternalCodeplugSize = contents[EXTERNAL_CODEPLUG_SIZE] * 0x100 + contents[EXTERNAL_CODEPLUG_SIZE + 1];
            Block31 = Deserialize<Block31>(contents, BLOCK_31_VECTOR, codeplugContents);
            Block3D = Deserialize<Block3D>(contents, BLOCK_3D_VECTOR, codeplugContents);
            Block36 = Deserialize<Block36>(contents, BLOCK_36_VECTOR, codeplugContents);
            Block55 = Deserialize<Block55>(contents, BLOCK_55_VECTOR, codeplugContents);
            Block54 = Deserialize<Block54>(contents, BLOCK_54_VECTOR, codeplugContents);
            Block51 = Deserialize<Block51>(contents, BLOCK_51_VECTOR, codeplugContents);
            DynamicRadioVector = contents[DYNAMIC_RADIO_VECTOR] * 0x100 + contents[DYNAMIC_RADIO_VECTOR + 1];
            if (DynamicRadioVector != 0)
            {
                DynamicRadio = codeplugContents.AsSpan().Slice(DynamicRadioVector - 8, DYNAMIC_RADIO_SIZE).ToArray();
                Debug.WriteLine($"Deserialize {DynamicRadioVector - 8:X4} Dynamic Mode Select Block  {String.Join(" ", Array.ConvertAll(DynamicRadio, x => x.ToString("X2")))}");
            }
            Block39 = Deserialize<Block39>(contents, BLOCK_39_VECTOR, codeplugContents);
            Block3B = Deserialize<Block3B>(contents, BLOCK_3B_VECTOR, codeplugContents);
            Block34 = Deserialize<Block34>(contents, BLOCK_34_VECTOR, codeplugContents);
            Block35 = Deserialize<Block35>(contents, BLOCK_35_VECTOR, codeplugContents);
            Block3C = Deserialize<Block3C>(contents, BLOCK_3C_VECTOR, codeplugContents);
            Block73 = Deserialize<Block73>(contents, BLOCK_73_VECTOR, codeplugContents);
            Unknown2 = contents.Slice(UNKNOWN2, 10).ToArray();
            DynamicModeSelectVector = contents[DYNAMIC_MODE_SELECT_VECTOR] * 0x100 + contents[DYNAMIC_MODE_SELECT_VECTOR + 1];
            if (DynamicModeSelectVector != 0)
            {
                DynamicModeSelect = codeplugContents.AsSpan().Slice(DynamicModeSelectVector - 8, DYNAMIC_MODE_SELECT_SIZE).ToArray();
                Debug.WriteLine($"Deserialize {DYNAMIC_MODE_SELECT_VECTOR - 8:X4} Dynamic Mode Select Block  {String.Join(" ", Array.ConvertAll(DynamicModeSelect, x => x.ToString("X2")))}");
            }
            Unknown3 = contents.Slice(UNKNOWN3, 2).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            var nextAddress = address + CONTENTS_LENGTH + BlockSizeAdjustment;
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            Encoding.ASCII.GetBytes(Serial).AsSpan().CopyTo(contents.Slice(SERIAL, 10));
            Encoding.ASCII.GetBytes(Model).AsSpan().CopyTo(contents.Slice(MODEL, 16));
            contents[TIMESTAMP] = SetDigits(TimeStamp.Year - 2000);
            contents[TIMESTAMP + 1] = SetDigits(TimeStamp.Month);
            contents[TIMESTAMP + 2] = SetDigits(TimeStamp.Day);
            contents[TIMESTAMP + 3] = SetDigits(TimeStamp.Hour);
            contents[TIMESTAMP + 4] = SetDigits(TimeStamp.Minute);
            contents[PROGRAMMING_SOURCE] = (byte)ProgrammingSource;
            contents[CODEPLUG_VERSION] = (byte)(ExternalCodeplugVersion / 0x100);
            contents[CODEPLUG_VERSION + 1] = (byte)(ExternalCodeplugVersion % 0x100);
            //External Code Block Size set at end, since size is not yet known.
            nextAddress = SerializeChild(Block31, BLOCK_31_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block3D, BLOCK_3D_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block36, BLOCK_36_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block55, BLOCK_55_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block54, BLOCK_54_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block51, BLOCK_51_VECTOR, codeplugContents, nextAddress, contents);
            if (DynamicRadio != null)
            {
                Debug.WriteLine($"Serialize {nextAddress:X4} Dynamic Radio Block  {String.Join(" ", Array.ConvertAll(DynamicRadio, x => x.ToString("X2")))}");
                DynamicRadioVector = nextAddress + 8;
                DynamicRadio.CopyTo(codeplugContents.AsSpan(nextAddress));
                nextAddress += DynamicRadio.Length;
            }
            else
            {
                DynamicRadioVector = 0;
            }
            contents[DYNAMIC_RADIO_VECTOR] = (byte)(DynamicRadioVector / 0x100);
            contents[DYNAMIC_RADIO_VECTOR + 1] = (byte)(DynamicRadioVector % 0x100);
            nextAddress = SerializeChild(Block39, BLOCK_39_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block3B, BLOCK_3B_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block34, BLOCK_34_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block35, BLOCK_35_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block3C, BLOCK_3C_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block73, BLOCK_73_VECTOR, codeplugContents, nextAddress, contents);
            Unknown2.AsSpan().CopyTo(contents.Slice(UNKNOWN2));
            if (DynamicModeSelect != null)
            {
                Debug.WriteLine($"Serialize {nextAddress:X4} Dynamic Mode Select Block  {String.Join(" ", Array.ConvertAll(DynamicModeSelect, x => x.ToString("X2")))}");
                DynamicModeSelectVector = nextAddress + 8;
                DynamicModeSelect.CopyTo(codeplugContents.AsSpan(nextAddress));
                nextAddress += DynamicModeSelect.Length;
            }
            else
            {
                DynamicModeSelectVector = 0;
            }
            contents[DYNAMIC_MODE_SELECT_VECTOR] = (byte)(DynamicModeSelectVector / 0x100);
            contents[DYNAMIC_MODE_SELECT_VECTOR + 1] = (byte)(DynamicModeSelectVector % 0x100);
            Unknown3.AsSpan().CopyTo(contents.Slice(UNKNOWN3));

            ExternalCodeplugSize = nextAddress - address;
            contents[EXTERNAL_CODEPLUG_SIZE] = (byte)(ExternalCodeplugSize / 0x100);
            contents[EXTERNAL_CODEPLUG_SIZE + 1] = (byte)(ExternalCodeplugSize % 0x100);

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
