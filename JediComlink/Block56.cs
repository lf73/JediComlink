using System;
using System.Text;

namespace JediComlink
{
    public class Block56 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x56; }
        public override string Description { get => "Conv or 62: Trunk/Test Mode Personality"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 25 95 64  05 CE 21 EE   1F 4A 00 10  10 10 08 60
        1: 80 20 00 1E  00 00 01 01   00 00 00 00  00 00 00 00
        2: 00 00 00
        */

        private const int UNKNOWN1 = 0x00; //TO 18
        private const int BLOCK_0E_VECTOR = 0x19; //1A
        private const int UNKNOWN2 = 0x1B; //1C 1D 1E 1F 20 21 22

        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 25).ToArray();
            //set => XYZ = value; //TODO
        }

        public Block0E Block0E { get; set; }

        public byte[] Unknown2
        {
            get => Contents.Slice(UNKNOWN2, 8).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion
        
        public Block56() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block0E = Deserialize<Block0E>(Contents, BLOCK_0E_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block0E, BLOCK_0E_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine($"Unknown1 Bytes: {FormatHex(Unknown1)}");
            sb.AppendLine($"Unknown2 Bytes: {FormatHex(Unknown2)}");

            sb.AppendLine(Block0E?.ToString());

            return sb.ToString();
        }
    }
}
