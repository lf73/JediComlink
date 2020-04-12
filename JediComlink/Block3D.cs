using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block3D : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x3D; }
        public override string Description { get => "Signaling Vector"; }

        #region Propeties
        public Block Block3E { get; set; }
        public byte[] UnknownPointer1 { get; set; }
        public Block Block4A { get; set; }
        public byte[] UnknownPointer2 { get; set; }
        public byte[] UnknownPointer3 { get; set; }
        public byte[] UnknownPointer4 { get; set; }
        public Block BlockA0 { get; set; }
        #endregion

        public Block3D() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = GetContents(codeplugContents, address);
            Block3E = Deserialize<Block3E>(Contents, 0x00, codeplugContents);
            UnknownPointer1 = Contents.Slice(0x02, 2).ToArray();
            Block4A = Deserialize<Block4A>(Contents, 0x04, codeplugContents);
            UnknownPointer2 = Contents.Slice(0x06, 2).ToArray();
            UnknownPointer3 = Contents.Slice(0x08, 2).ToArray();
            UnknownPointer4 = Contents.Slice(0x0A, 2).ToArray();
            BlockA0 = Deserialize<BlockA0>(Contents, 0x0C, codeplugContents);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(Block3E.ToString());
            sb.AppendLine($"Unknown Pointer: {FormatHex(UnknownPointer1)}");
            sb.AppendLine(Block4A.ToString());
            sb.AppendLine($"Unknown Pointer: {FormatHex(UnknownPointer2)}");
            sb.AppendLine($"Unknown Pointer: {FormatHex(UnknownPointer3)}");
            sb.AppendLine($"Unknown Pointer: {FormatHex(UnknownPointer4)}");
            sb.AppendLine(BlockA0.ToString());
            return sb.ToString();
        }
    }
}
