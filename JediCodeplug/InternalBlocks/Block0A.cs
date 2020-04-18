using System;
using System.Collections.Generic;
using System.Text;

namespace JediCodeplug
{
    public class Block0A : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x0A; }
        public override string Description { get => "Softpot B/W Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 0C 01 13  01 1A 01 21  //TODO ??There's no count byte.. Is this fixed?
        */
        #endregion

        #region Propeties
        public List<Block0B> Block0BList { get; set; } = new List<Block0B>();
        #endregion

        public Block0A() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            for (int i = 0; i < 4; i++) //4 Hardcoded for number of elements. This one is not dynamic like others?
            {
                Block0BList.Add(Deserialize<Block0B>(Contents, i * 2, codeplugContents));
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;

            int i = 0;
            foreach (var block0B in Block0BList)
            {
                nextAddress = SerializeChild(block0B, i * 2, codeplugContents, nextAddress, contents);
                i++;
            }

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
