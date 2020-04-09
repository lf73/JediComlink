using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class GenericBlock : Block
    {
        public GenericBlock(Block parent, int vector) : base(parent, vector)
        {
            LongChecksum = false;
            Description = "Generic Block";
        }

        //public override void Load(byte[] codeplug, int offSet)
        //{
        //    Length = codeplug[offSet];

        //    if (Length < 0x02)
        //    {
        //        Length = Length * 0x100 + codeplug[offSet + 1];
        //        Id = codeplug[offSet + 2];
        //        LongChecksum = true;
        //        Bytes = new Span<byte>(codeplug, offSet, Length+4).ToArray();
        //    }
        //    else
        //    {
        //        Id = codeplug[offSet + 1];
        //        Bytes = new Span<byte>(codeplug, offSet, Length+2).ToArray();
        //    }

        //    StartAddress = offSet;
        //    EndAddress = offSet + Length;
        //}
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            var s = new String(' ', Level * 2);
            sb.AppendLine(GetTextHeader());
            sb.AppendLine(s + "Unknown Bytes");
            sb.AppendLine(FormatHexIndent(Contents.ToArray()));

            if (LongChecksum)
            {
                int checksum = -0x5555;
                foreach (var b in Contents)
                    checksum += b;
                checksum -= Contents[Length + 2];
                checksum -= Contents[Length + 3];
                checksum &= 0xFFFF;
                //sb.AppendLine(s + $"Checksum {checksum:X4}");
            }
            else
            {
                int checksum = -0x55;
                foreach (var b in Contents)
                    checksum += b;
                //checksum -= Contents[Length + 1];
                checksum &= 0xFF;
                //sb.AppendLine(s + $"Checksum {checksum:X2}");
            }


            return sb.ToString();
        }

        private string FormatHexIndent(byte[] data)
        {
            var sb = new StringBuilder();
            var s = new String(' ', Level * 2 + 3);
            sb.Append(s);
            var l = 1;
            foreach (var b in data)
            {
                sb.Append(b.ToString("X2"));
                if (l < data.Length) sb.Append(l++ % 16 == 0 ? "\n" + s : " ");
            }
            return sb.ToString();
        }


    }
}
