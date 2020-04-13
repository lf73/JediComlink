using System;
using System.Text;

namespace JediCodeplug
{
    public class Block05 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x05; }
        public override string Description { get => "HWConfig Secure"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 3D 46 11  1E 00 1D
        */

        private const int UNKNOWN1 = 0x00; //01 02 03 04 05 06
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 7).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

        public Block05() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            return Serializer(codeplugContents, address, contents) + address;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine($"Unknown1 Bytes: {FormatHex(Unknown1)}");

            return sb.ToString();
        }
    }
}
