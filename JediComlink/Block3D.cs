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
        #region Propeties
        public Block Block3E { get; set; }
        public byte[] UnknownPointer1 { get; set; }
        public Block Block4A { get; set; }
        public byte[] UnknownPointer2 { get; set; }
        public byte[] UnknownPointer3 { get; set; }
        public byte[] UnknownPointer4 { get; set; }
        public Block BlockA0 { get; set; }
        #endregion

        public Block3D(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Id = 0x3D;
            Description = "Signaling Vector";

            Block3E = new Block3E(this, 0x00, codeplugContents);
            UnknownPointer1 = Contents.Slice(0x02, 2).ToArray();
            Block4A = new Block4A(this, 0x04, codeplugContents);
            UnknownPointer2 = Contents.Slice(0x06, 2).ToArray();
            UnknownPointer3 = Contents.Slice(0x08, 2).ToArray();
            UnknownPointer4 = Contents.Slice(0x0A, 2).ToArray();
            BlockA0 = new BlockA0(this, 0x0C, codeplugContents);
        }

        public override string ToString()
        {
            var s = new String(' ', Level * 2);
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(s + Block3E.ToString());
            sb.AppendLine(s + $"Unknown Pointer: {FormatHex(UnknownPointer1)}");
            sb.AppendLine(s + Block4A.ToString());
            sb.AppendLine(s + $"Unknown Pointer: {FormatHex(UnknownPointer2)}");
            sb.AppendLine(s + $"Unknown Pointer: {FormatHex(UnknownPointer3)}");
            sb.AppendLine(s + $"Unknown Pointer: {FormatHex(UnknownPointer4)}");
            sb.AppendLine(s + BlockA0.ToString());
            return sb.ToString();
        }
    }
}
