using System;
using System.Text;

namespace JediCodeplug
{
    public class Block11 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x11; }
        public override string Description { get => "AD Button Level"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 1F 30 70  85 CF 55 AA
        */

        private const int UNKNOWN1 = 0x00; //01 02 03 04 05 06 07
        #endregion

        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 8).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

        public Block11() { }

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
