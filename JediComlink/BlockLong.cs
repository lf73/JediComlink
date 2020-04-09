using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JediComlink
{
    public abstract class BlockLong : Block
    {
        /// <summary>
        /// Returns actual length of contents, which is block length minus block type and checksum bytes
        /// </summary>
        public override int ContentsLength
        {
            get
            {
                return Length - 1;
            }
        }
        public override Span<Byte> Contents
        {
            get
            {
                return new Span<byte>(Codeplug.Contents, StartAddress + 3, ContentsLength);
            }
        }

        public BlockLong(Block parent, int vector)
        {
            Codeplug = parent.Codeplug;
            Parent = parent;
            Level = parent.Level + 1;

            StartAddress = parent.Contents[vector]  * 0x100 + parent.Contents[vector + 1];
            Length = Codeplug.Contents[StartAddress] * 0x100 + Codeplug.Contents[StartAddress+1];
            EndAddress = StartAddress + Length;
            Id = Codeplug.Contents[StartAddress + 1];
        }
    }

}
