using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JediComlink
{
    public abstract class BlockLong : Block
    {
        public BlockLong(Block parent, int vector, byte[] codeplugContents)
        {
            Codeplug = parent.Codeplug;
            Parent = parent;
            Level = parent.Level + 1;

            StartAddress = parent.Contents[vector] * 0x100 + parent.Contents[vector + 1];
            var length = codeplugContents[StartAddress] * 0x100 + codeplugContents[StartAddress + 1];
            Contents = codeplugContents.AsSpan().Slice(StartAddress + 3, length - 1);

            Codeplug.Children.Add(this);
        }
    }

}
