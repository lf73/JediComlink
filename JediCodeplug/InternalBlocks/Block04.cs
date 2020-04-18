using System;
using System.Text;

namespace JediCodeplug
{
    public class Block04 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x04; }
        public override string Description { get => "HWConfig Conv"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 32 22 12 00  04 08 0C 14   1C 20 28
        */

        private const int UNKNOWN1 = 0x00; //01 02 03 04 05 06 07 08 09 0A
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 11).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

        public Block04() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
