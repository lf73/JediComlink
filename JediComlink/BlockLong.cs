using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JediComlink
{
    public abstract class BlockLong : Block
    {
        protected override int BlockSizeAdjustment { get => 5; }

        public override Span<byte> Deserializer(byte[] codeplugContents, int address)
        {
            var length = codeplugContents[address] * 0x100 + codeplugContents[address + 1];
            return codeplugContents.AsSpan().Slice(address + 2, length - 1);
        }

        protected override int Serializer(byte[] codeplugContents, int address, Span<byte> contents)
        {
            codeplugContents[address] = (byte)(contents.Length + 1);
            //TODO set Lenght byte 2
            codeplugContents[address + 2] = (byte)Id;
            contents.CopyTo(codeplugContents.AsSpan(address + 3));

            int checksum = -0x55 + codeplugContents[address] + codeplugContents[address + 1];
            foreach (var b in contents)
                checksum += b;
            codeplugContents[address + contents.Length + 2] = (byte)(checksum &= 0xFF);
            return contents.Length + 4;
            //TODO
            //          int checksum = -0x5555;
            //          foreach (var b in Contents)
            //              checksum += b;
            //          checksum -= Contents[Length + 2];
            //          checksum -= Contents[Length + 3];
            //          checksum &= 0xFFFF;
            //          //sb.AppendLine($"Checksum {checksum:X4}");
            //      }

        }

    }
}
