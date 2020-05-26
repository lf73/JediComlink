using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JediCodeplug
{
    public abstract class Block
    {

        [DisplayName("Block Id")]
        [TypeConverter(typeof(HexByteValueTypeConverter))]
        public abstract byte Id { get; }

        public abstract string Description { get; }

        protected virtual int BlockSizeAdjustment { get => 3; }

        public abstract void Deserialize(byte[] codeplugContents, int address);

        public abstract int Serialize(byte[] codeplugContents, int address);

        public override string ToString() => $"Block {Id:X2} {Description}";

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
            var contents = codeplugContents.AsSpan().Slice(address + 2, length - 1).ToArray(); //The ToArray is to force a copy
            //TODO Set a new property such as HasValidChecksum  

            Debug.WriteLine($"Deserialize {address:X4} {Id:X2} {Description} - {String.Join(" ", Array.ConvertAll(contents, x => x.ToString("X2")))}");           
            return contents;
        }

        protected virtual int Serializer(byte[] codeplugContents, int address, Span<byte> contents)
        {
            Debug.WriteLine($"Serialize {address:X4} {Id:X2} {Description} -{String.Join(" ", Array.ConvertAll(contents.ToArray(), x => x.ToString("X2")))}");
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
                contents[vector] = 0;
                contents[vector + 1] = 0;
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

        protected string FormatHex(byte[] data)
        {
            var sb = new StringBuilder();
            //var l = 1;
            foreach (var b in data)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(" ");
                //if (l < data.Length) sb.Append(l++ % 16 == 0 ? "\n" : " ");
            }
            return sb.ToString();
        }

        protected string FormatBinaryValue(int val)
        {
            return Convert.ToString(val, 2).PadLeft(8, '0');
        }

        protected int GetDigits(byte nibbles)
        {
            return (nibbles >> 4) * 10 + (nibbles & 0x0f);
        }
        protected byte SetDigits(int number)
        {
            if (number < 0 || number > 99) throw new ArgumentOutOfRangeException("Expected range is 0 to 99", nameof(number));
            return (byte)(((number / 10) << 4) + (number % 10));
        }

        #endregion

        public string GetTextDump()
        {
            var sb = new StringBuilder();
            const string SEP = "--------------------------------------------------------------------------------------------";

            static string GetName(PropertyInfo p) => p.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? p.Name;

            static string FormatProperty(Object o) => o switch
            {
                byte byteVal => $"{byteVal:X2}",
                int intVal => $"{intVal:X2}",
                IEnumerable<byte> bytes => String.Join(" ", Array.ConvertAll(bytes.ToArray(), x => x.ToString("X2"))),
                null => "",
                _ => o.ToString(),
            };

            void Dump(Block block, int level = 0)
            {
                string spacer = new string(' ', (level++ * 4));
                sb.AppendLine($"{spacer}Block {block.Id:X2} {block.Description}");
                sb.AppendLine($"{spacer}{SEP}");
                spacer = new string(' ', (level * 4));
                foreach (var property in block.GetType().GetProperties()
                        .Where(x => x.Name != "Id"
                                && x.Name != "Description"
                                && x.Name != "Contents"
                                && !(x.GetValue(block) is IEnumerable<Object>)
                                && !(x.GetValue(block) is Block)))
                    sb.AppendLine($"{spacer}{GetName(property)}: {FormatProperty(property.GetValue(block))}");

                //Go through children
                foreach (var property in block.GetType().GetProperties())
                {
                    if (property.GetValue(block) is Block val)
                    {
                        Dump(val, level);
                    }
                    else if (property.GetValue(block) is IEnumerable<Block> blocks)
                    {
                        sb.AppendLine($"{spacer}{GetName(property)}: Contains {blocks.Count()} items");
                        sb.AppendLine($"{spacer}{SEP}");
                        level++;
                        foreach (var blockItem in blocks)
                            Dump(blockItem, level);
                        level--;
                    }
                }
                sb.AppendLine();
            }

            Dump(this);
            return sb.ToString();
        }
    }
}
