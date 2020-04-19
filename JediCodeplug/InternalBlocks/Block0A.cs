using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block0A : Block
    {
        public override int Id { get => 0x0A; }
        public override string Description { get => "Softpot B/W Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 0C 01 13  01 1A 01 21  //TODO ??There's no count byte.. Is this fixed?
        */
        #endregion

        #region Propeties
        public Block0B[] Block0BList { get; } = new Block0B[4];
        #endregion

        public Block0A() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            for (int i = 0; i < 4; i++) //4 Hardcoded for number of elements. This one is not dynamic like others?
            {
                Block0BList[i] = (Deserialize<Block0B>(contents, i * 2, codeplugContents));
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contentsLength = 2 * Block0BList.Length;
            var contents = new byte[contentsLength].AsSpan();
            var nextAddress = address + contentsLength + BlockSizeAdjustment;
            
            for (int i = 0; i < Block0BList.Length; i++)
            {
                nextAddress = SerializeChild(Block0BList[i], i * 2, codeplugContents, nextAddress, contents);
            }

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
