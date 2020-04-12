using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JediComlink
{
    public abstract class BlockLong : Block
    {
        public override Span<byte> GetContents(byte[] codeplugContents, int address)
        {
            var length = codeplugContents[address] * 0x100 + codeplugContents[address + 1];
            return codeplugContents.AsSpan().Slice(address + 2, length - 1);
        }
    }
}
