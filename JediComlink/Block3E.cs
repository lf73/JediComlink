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

        public Block3E(Block parent, int vector, byte[] codeplugContents) : base(parent, vector, codeplugContents)
        {
            Block3F = new Block3F(this, 0x00, codeplugContents);
            Block41 = new Block41(this, 0x02, codeplugContents);
            UnknownBytes1 = Contents.Slice(0x04, 3).ToArray();
            Block42 = new Block42(this, 0x07, codeplugContents);
            UnknownBytes2 = Contents.Slice(0x09, 3).ToArray();
            Block8E = new Block8E(this, 0x0C, codeplugContents);
            Block90 = new Block90(this, 0x0E, codeplugContents);
        }

        public override string ToString()
        {
            var s = new String(' ', Level * 2);
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(s + Block3F.ToString());
            sb.AppendLine(s + Block41.ToString());
            sb.AppendLine(s + $"Unknown Bytes: {FormatHex(UnknownBytes1)}");
            sb.AppendLine(s + Block42.ToString());
            sb.AppendLine(s + $"Unknown Bytes: {FormatHex(UnknownBytes2)}");
            sb.AppendLine(s + Block8E.ToString());
            sb.AppendLine(s + Block90.ToString());
            return sb.ToString();
        }
    }
}
