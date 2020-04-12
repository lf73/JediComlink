using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block62 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x62; }
        public override string Description { get => "Unknown"; }

        #region Propeties
        public Block58 Block58 { get; set; }
        public Block63 Block63 { get; set; }
        public Block67 Block67 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 02 0E 01 07  E4 22 00 00   04 3D 00 00  07 F3 00 00
        1: 00 00 00 00  00 00 00 00   00 00 00 00  00
        */

        private const int BLOCK_58_VECTOR = 0x08;
        private const int BLOCK_63_VECTOR = 0x0C;
        private const int BLOCK_67_VECTOR = 0x03;
        #endregion

        public Block62() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block58 = Deserialize<Block58>(Contents, BLOCK_58_VECTOR, codeplugContents);
            Block63 = Deserialize<Block63>(Contents, BLOCK_63_VECTOR, codeplugContents);
            Block67 = Deserialize<Block67>(Contents, BLOCK_67_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block58, BLOCK_58_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block63, BLOCK_63_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block67, BLOCK_67_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block58.ToString());
            sb.AppendLine(Block63.ToString());
            sb.AppendLine(Block67.ToString());

            return sb.ToString();
        }
    }
}
