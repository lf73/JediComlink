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
            Block3F = Deserialize<Block3F>(Contents, 0x00, codeplugContents);
            Block41 = Deserialize<Block41>(Contents, 0x02, codeplugContents);
            UnknownBytes1 = Contents.Slice(0x04, 3).ToArray();
            Block42 = Deserialize<Block42>(Contents, 0x07, codeplugContents);
            UnknownBytes2 = Contents.Slice(0x09, 3).ToArray();
            Block8E = Deserialize<Block8E>(Contents, 0x0C, codeplugContents);
            Block90 = Deserialize<Block90>(Contents, 0x0E, codeplugContents);
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
