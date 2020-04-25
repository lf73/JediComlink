using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block61 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override byte Id { get => 0x61; }
        public override string Description { get => "Unknown"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 03 20
        */

        private const int POST_BLOCK_DATA_SIZE = 0x28;

        #endregion

        #region Propeties
        [DisplayName("Unknown Post Block Data")]
        [Description("This data is stored outside of a normal block definition. Perhaps this is reserved space for the radio to store user settings?")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] PostBlockData { get; set; }
        #endregion


        public Block61() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            PostBlockData = codeplugContents.AsSpan().Slice(address + Contents.Length + BlockSizeAdjustment, POST_BLOCK_DATA_SIZE).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;

            PostBlockData.CopyTo(codeplugContents.AsSpan(nextAddress));
            nextAddress += PostBlockData.Length;

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
