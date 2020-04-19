using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block06 : Block
    {
        public override int Id { get => 0x06; }
        public override string Description { get => "Softpot Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 7A 00 91  01 01
        */

        private const int CONTENTS_LENGTH = 0x06;
        private const int BLOCK_07_VECTOR = 0x00; //01
        private const int BLOCK_08_VECTOR = 0x02; //03
        private const int BLOCK_0A_VECTOR = 0x04; //05
        #endregion

        #region Propeties
        public Block07 Block07 { get; set; }

        public Block08 Block08 { get; set; }

        public Block0A Block0A { get; set; }
        #endregion

        public Block06() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            Block07 = Deserialize<Block07>(contents, BLOCK_07_VECTOR, codeplugContents);
            Block08 = Deserialize<Block08>(contents, BLOCK_08_VECTOR, codeplugContents);
            Block0A = Deserialize<Block0A>(contents, BLOCK_0A_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            var nextAddress = address + CONTENTS_LENGTH + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block07, BLOCK_07_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block08, BLOCK_08_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block0A, BLOCK_0A_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
