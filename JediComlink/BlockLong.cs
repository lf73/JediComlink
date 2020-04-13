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
            return codeplugContents.AsSpan().Slice(address + 3, length - 1);
        }

        protected override int Serializer(byte[] codeplugContents, int address, Span<byte> contents)
        {
            var length = contents.Length + 1;
            codeplugContents[address] = (byte)(length / 0x100);
            codeplugContents[address + 1] = (byte)(length % 0x100);

            codeplugContents[address + 2] = (byte)Id;
            contents.CopyTo(codeplugContents.AsSpan(address + 3));

            int checksum = -0x5555 + codeplugContents[address] + codeplugContents[address + 1] + codeplugContents[address + 2];
            foreach (var b in contents)
            {
                checksum += b;
            }

            checksum &= 0xFFFF;

            codeplugContents[address + contents.Length + 3] = (byte)(checksum / 0x100);
            codeplugContents[address + contents.Length + 4] = (byte)(checksum % 0x100);
            return contents.Length + 5;
        }

    }
}
