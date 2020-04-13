using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block3E : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x3E; }
        public override string Description { get => "Conv Configuration"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 03 2E 03 44  54 0B 00 03   4A 14 00 00  03 61 03 A8
        */

        private const int BLOCK_3F_VECTOR = 0x00;
        private const int BLOCK_41_VECTOR = 0x02;
        private const int BLOCK_42_VECTOR = 0x07;
        private const int BLOCK_8E_VECTOR = 0x0C;
        private const int BLOCK_90_VECTOR = 0x0E;
        #endregion

        #region Propeties
        public Block Block3F { get; set; }
        public Block Block41 { get; set; }
        public byte[] UnknownBytes1 { get; set; }
        public Block Block42 { get; set; }
        public byte[] UnknownBytes2 { get; set; }
        public Block Block8E { get; set; }
        public Block Block90 { get; set; }
        #endregion

        public Block3E() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            Block3F = Deserialize<Block3F>(Contents, BLOCK_3F_VECTOR, codeplugContents);
            Block41 = Deserialize<Block41>(Contents, BLOCK_41_VECTOR, codeplugContents);
            UnknownBytes1 = Contents.Slice(0x04, 3).ToArray();
            Block42 = Deserialize<Block42>(Contents, BLOCK_42_VECTOR, codeplugContents);
            UnknownBytes2 = Contents.Slice(0x09, 3).ToArray();
            Block8E = Deserialize<Block8E>(Contents, BLOCK_8E_VECTOR, codeplugContents);
            Block90 = Deserialize<Block90>(Contents, BLOCK_90_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block3F, BLOCK_3F_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block41, BLOCK_41_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block42, BLOCK_42_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block8E, BLOCK_8E_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block90, BLOCK_90_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block3F.ToString());
            sb.AppendLine(Block41.ToString());
            sb.AppendLine($"Unknown Bytes: {FormatHex(UnknownBytes1)}");
            sb.AppendLine(Block42.ToString());
            sb.AppendLine($"Unknown Bytes: {FormatHex(UnknownBytes2)}");
            sb.AppendLine(Block8E.ToString());
            sb.AppendLine(Block90.ToString());
            return sb.ToString();
        }
    }
}
