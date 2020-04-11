using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace JediComlink
{
    public abstract class Block
    {
        public abstract int Id { get; }

        public abstract string Description { get; }

        public Block Parent { get; set; }

        public Codeplug Codeplug { get; set; }

        private byte[] _contents;
        protected internal Span<Byte> Contents
        {
            get
            {
                return _contents.AsSpan();
            }
            set
            {
                _contents = value.ToArray();
            }
        }

        public int Level { get; set; }

        public int StartAddress { get; set; }

        protected Block() { }

        public Block(Block parent, int vector, byte[] codeplugContents)
        {
            Codeplug = parent.Codeplug;
            Parent = parent;
            Level = parent.Level + 1;

            StartAddress = parent.Contents[vector] * 0x100 + parent.Contents[vector + 1];
            var length = codeplugContents[StartAddress];
            Contents = codeplugContents.AsSpan().Slice(StartAddress + 2, length - 1);

            Codeplug.Children.Add(this);
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
            return Environment.NewLine + s + $"Block {Id:X2} Length {_contents.Length} Starting At {StartAddress:X4}    {Description}" + Environment.NewLine +
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

        protected virtual void UpdateContents()
        {

        }

        public byte[] GetBytes()
        {
            UpdateContents();
            byte[] bytes = new byte[Contents.Length + 3];
            bytes[0] = (byte)(Contents.Length + 1);
            bytes[1] = (byte)Id;
            for (int i = 0; i < Contents.Length; i++)
            {
                bytes[2 + i] = Contents[i];
            }

            int checksum = -0x55 + bytes[0] + bytes[1];
            foreach (var b in Contents)
                checksum += b;
            bytes[bytes.Length - 1] = (byte)(checksum &= 0xFF);

            return bytes;
        }
    }

    //ToDo  Add in Checksum
      //if (LongChecksum)
      //      {
      //          int checksum = -0x5555;
      //          foreach (var b in Contents)
      //              checksum += b;
      //          checksum -= Contents[Length + 2];
      //          checksum -= Contents[Length + 3];
      //          checksum &= 0xFFFF;
      //          //sb.AppendLine(s + $"Checksum {checksum:X4}");
      //      }
      //      else
      //      {
      //          int checksum = -0x55;
      //          foreach (var b in Contents)
      //              checksum += b;
      //          //checksum -= Contents[Length + 1];
      //          checksum &= 0xFF;
      //          //sb.AppendLine(s + $"Checksum {checksum:X2}");
      //      }



}
