using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block08 : Block //Block Vector Array
    {
        public override int Id { get => 0x08; }
        public override string Description { get => "Softpot Interpol Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 00 A1 00  B1 00 C1 00   D1 00 E1 00  F1
        */

        private const int COUNT = 0x00;
        #endregion

        #region Propeties
        [BrowsableAttribute(false)]
        public List<Block09> Block09List { get; set; } = new List<Block09>();
        #endregion

        public Block08() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);

            for (int i = 0; i < contents[COUNT]; i++)
            {
                Block09List.Add(Deserialize<Block09>(contents, i * 2 + 1, codeplugContents));
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contentsLength = 1 + (2 * Block09List.Count);
            var contents = new byte[contentsLength].AsSpan();
            var nextAddress = address + contentsLength + BlockSizeAdjustment;

            int i = 0;
            foreach (var block in Block09List)
            {
                nextAddress = SerializeChild(block, i * 2 + 1, codeplugContents, nextAddress, contents);
                i++;
            }

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
