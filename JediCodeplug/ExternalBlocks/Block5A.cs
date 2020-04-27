using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block5A : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override byte Id { get => 0x5A; }
        public override string Description { get => "Trunk System"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 02 1C 04 3D  04 56 95 64   1C 20 00 01  00 0C 00 00
        1: 00 00 00 00  00 00 00 00   00 00 04 64  00 00 00 01
        2: 00 26 01 26  01 26 01 26   01 00 00 00  00 00 00 00
        3: 00 00 00 05
        */

        private const int BLOCK_58_VECTOR = 0x02;
        private const int BLOCK_61_VECTOR = 0x04;
        private const int DYNAMIC_TRUNK_VECTOR = 0x1A;
        private const int DYNAMIC_TRUNK_SIZE = 0x28;

        #endregion

        #region Propeties
        public Block58 Block58 { get; set; }
        public Block61 Block61 { get; set; }

        [Browsable(false)]
        public int DynamicTrunkVector { get; set; }

        [DisplayName("Dynamic Trunk")]
        [Description("This data is stored outside of a normal block definition. Perhaps this is reserved space for the radio to store user settings?")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] DynamicTrunk { get; set; }
        #endregion
        
        public Block5A() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block58 = Deserialize<Block58>(Contents, BLOCK_58_VECTOR, codeplugContents);
            Block61 = Deserialize<Block61>(Contents, BLOCK_61_VECTOR, codeplugContents);

            DynamicTrunkVector = Contents[DYNAMIC_TRUNK_VECTOR] * 0x100 + Contents[DYNAMIC_TRUNK_VECTOR + 1];
            if (DynamicTrunkVector != 0)
            {
                DynamicTrunk = codeplugContents.AsSpan().Slice(DynamicTrunkVector - 8, DYNAMIC_TRUNK_SIZE).ToArray();
                Debug.WriteLine($"Deserialize {DynamicTrunkVector - 8:X4} XX Dynamic Trunk Block - {String.Join(" ", Array.ConvertAll(DynamicTrunk, x => x.ToString("X2")))}");
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block58, BLOCK_58_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block61, BLOCK_61_VECTOR, codeplugContents, nextAddress, contents);

            if (DynamicTrunk != null)
            {
                Debug.WriteLine($"Serialize {nextAddress:X4} XX Dynamic Trunk Block - {String.Join(" ", Array.ConvertAll(DynamicTrunk, x => x.ToString("X2")))}");
                DynamicTrunkVector = nextAddress + 8;
                DynamicTrunk.CopyTo(codeplugContents.AsSpan(nextAddress));
                nextAddress += DynamicTrunk.Length;
            }
            else
            {
                DynamicTrunkVector = 0;
            }
            contents[DYNAMIC_TRUNK_VECTOR] = (byte)(DynamicTrunkVector / 0x100);
            contents[DYNAMIC_TRUNK_VECTOR + 1] = (byte)(DynamicTrunkVector % 0x100);

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
