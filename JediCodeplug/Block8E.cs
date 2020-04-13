using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block8E : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x8E; }
        public override string Description { get => "Status List Vector"; }

        #region Propeties
        public Block8F Block8F { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 02 F0 00 08  01 02 03 04   05 06 07 08
        */

        private const int BLOCK_8F_VECTOR = 0x00;
        #endregion

        public Block8E() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block8F = Deserialize<Block8F>(Contents, BLOCK_8F_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block8F, BLOCK_8F_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block8F.ToString());

            return sb.ToString();
        }
    }
}
