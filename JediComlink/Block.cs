using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JediComlink
{
    public abstract class Block
    {
        public int Id { get; protected set; }

        public string Description { get; protected set; }

        public int Length { get; set; }

        public Block Parent { get; set; }

        public Codeplug Codeplug { get; set; }

        /// <summary>
        /// Returns actual length of contents, which is block length minus block type and checksum bytes
        /// </summary>
        public virtual int ContentsLength
        {
            get
            {
                return Length - 1;
            }
        }

        public virtual Span<Byte> Contents
        {
            get
            {
                return new Span<byte>(Codeplug.Contents, StartAddress + 2, ContentsLength);
            }
        }

        public List<Block> Children { get; set; }

        public int Level { get; set; }

        public int StartAddress { get; set; }

        public int EndAddress { get; set; }

        public bool LongChecksum { get; set; }

        protected Block() { }

        public Block(Block parent, int vector)
        {
            Codeplug = parent.Codeplug;
            Parent = parent;
            Level = parent.Level + 1;

            StartAddress = parent.Contents[vector]  * 0x100 + parent.Contents[vector + 1];
            Length = Codeplug.Contents[StartAddress];
            EndAddress = StartAddress + Length;
            Id = Codeplug.Contents[StartAddress + 1];
        }
        
        public string GetStringContents(int offset, int length)
        {
            var value = "";
            foreach (var c in Contents.Slice(offset, length))
            {
                if (c == 0x00) break;
                value += Convert.ToChar(c);

            }
            return value;
        }

        public string GetTextHeader()
        {
            var s = new String(' ', (Parent?.Level).GetValueOrDefault() * 2);
            return Environment.NewLine + s + $"Block {Id:X2} Length {Length} Starting At {StartAddress:X4}" + Environment.NewLine +
                    s + "---------------------------";
        }
           
        public abstract override string ToString();

        protected string FormatHex(byte[] data)
        {
            var sb = new StringBuilder();
            var l = 1;
            foreach (var b in data)
            {
                sb.Append(b.ToString("X2"));
                if (l < data.Length) sb.Append(l++ % 16 == 0 ? "\n" : " ");
            }
            return sb.ToString();
        }

        protected int GetDigits(byte nibbles)
        {
            return (nibbles >> 4) * 10 + (nibbles & 0x0f);
        }
    }

}
