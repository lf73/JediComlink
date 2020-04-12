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

        protected virtual int BlockSizeAdjustment { get => 3; }

        public abstract void Deserialize(byte[] codeplugContents, int address);

        public abstract int Serialize(byte[] codeplugContents, int address);

        public abstract override string ToString();



        protected static T Deserialize<T>(Span<byte> parentContents, int vector, byte[] codeplugContents) where T : Block, new()
        {
            var address = parentContents[vector] * 0x100 + parentContents[vector + 1];

            if (address == 0) return null;

            var x = new T();
            x.Deserialize(codeplugContents, address);
            return (T)x;
        }

        public virtual Span<byte> Deserializer(byte[] codeplugContents, int address)
        {
            var length = codeplugContents[address];
            return codeplugContents.AsSpan().Slice(address + 2, length - 1).ToArray();
            //TODO Set a new property such as HasValidChecksum  
        }

        protected virtual int Serializer(byte[] codeplugContents, int address, Span<byte> contents)
        {
            codeplugContents[address] = (byte)(contents.Length + 1);
            codeplugContents[address + 1] = (byte)Id;
            contents.CopyTo(codeplugContents.AsSpan(address + 2));

            int checksum = -0x55 + codeplugContents[address] + codeplugContents[address + 1];
            foreach (var b in contents)
            {
                checksum += b;
            }
            codeplugContents[address + contents.Length + 2] = (byte)(checksum &= 0xFF);
            return contents.Length + BlockSizeAdjustment; //Since adding the BlockSizeAdjustment may be able to eliminate the return.
        }

        protected int SerializeChild(Block child, int vector, byte[] codeplugContents, int address, Span<byte> contents)
        {
            if (child != null)
            {
                contents[vector] = (byte)(address / 0x100);
                contents[vector + 1] = (byte)(address % 0x100);
                return child.Serialize(codeplugContents, address);
            }
            else
            {
                return address;
            }
        }

        #region Helper Methods
        //TODO Consider moving these helper functions
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
        #endregion

    }
}
