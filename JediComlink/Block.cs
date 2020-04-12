using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace JediComlink
{
    public abstract class Block
    {
        public abstract int Id { get; }

        public abstract string Description { get; }

        protected Block() { }

        //public Block(Block parent, int vector, byte[] codeplugContents)
        //{
        //    var Address = parent.Contents[vector] * 0x100 + parent.Contents[vector + 1];
        //    var length = codeplugContents[Address];
        //    Contents = codeplugContents.AsSpan().Slice(Address + 2, length - 1);
        //}

        public string GetStringContents(Span<byte> Contents, int offset, int length)
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
            return Environment.NewLine + $"Block {Id:X2}  {Description}" + Environment.NewLine +
                    "---------------------------";
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

        public byte[] Serialize()
        {
            //byte[] bytes = new byte[Contents.Length + 3];
            //bytes[0] = (byte)(Contents.Length + 1);
            //bytes[1] = (byte)Id;
            //for (int i = 0; i < Contents.Length; i++)
            //{
            //    bytes[2 + i] = Contents[i];
            //}

            //int checksum = -0x55 + bytes[0] + bytes[1];
            //foreach (var b in Contents)
            //    checksum += b;
            //bytes[bytes.Length - 1] = (byte)(checksum &= 0xFF);

            //return bytes;
            return null;
        }

        public abstract void Deserialize(byte[] codeplugContents, int address);

        public virtual Span<byte> GetContents(byte[] codeplugContents, int address)
        {
            var length = codeplugContents[address];
            return codeplugContents.AsSpan().Slice(address + 2, length - 1).ToArray();
        }

        protected static T Deserialize<T>(Span<byte> parentContents, int vector, byte[] codeplugContents) where T : Block, new()
        {
            var address = parentContents[vector] * 0x100 + parentContents[vector + 1];

            if (address == 0) return null;

            var x = new T();
            x.Deserialize(codeplugContents, address);
            return (T)x;
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
      //          //sb.AppendLine($"Checksum {checksum:X4}");
      //      }
      //      else
      //      {
      //          int checksum = -0x55;
      //          foreach (var b in Contents)
      //              checksum += b;
      //          //checksum -= Contents[Length + 1];
      //          checksum &= 0xFF;
      //          //sb.AppendLine($"Checksum {checksum:X2}");
      //      }



}
