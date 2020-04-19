using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block03 : Block
    {
        public override int Id { get => 0x03; }
        public override string Description { get => "Hardware Config Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 31 01 3F  01 49
        */

        private const int CONTENTS_LENGTH = 0x06;
        private const int BLOCK_04_VECTOR = 0x00; //01
        private const int BLOCK_05_VECTOR = 0x02; //03
        private const int BLOCK_0D_VECTOR = 0x04; //05
        #endregion

        #region Propeties
        [Browsable(false)]
        public Block04 Block04 { get; set; }

        [Browsable(false)]
        public Block05 Block05 { get; set; }

        [Browsable(false)]
        public Block0D Block0D { get; set; }
        #endregion

        public Block03() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            contents = Deserializer(codeplugContents, address);
            Block04 = Deserialize<Block04>(contents, BLOCK_04_VECTOR, codeplugContents);
            Block05 = Deserialize<Block05>(contents, BLOCK_05_VECTOR, codeplugContents);
            Block0D = Deserialize<Block0D>(contents, BLOCK_0D_VECTOR, codeplugContents);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            var nextAddress = address + contents.Length + BlockSizeAdjustment;
            nextAddress = SerializeChild(Block04, BLOCK_04_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block05, BLOCK_05_VECTOR, codeplugContents, nextAddress, contents);
            nextAddress = SerializeChild(Block0D, BLOCK_0D_VECTOR, codeplugContents, nextAddress, contents);
            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
