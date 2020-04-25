using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block47 : Block //Block Vector Array
    {
        public override byte Id { get => 0x47; }
        public override string Description { get => "MDC System Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 04 35
        */

        private const int COUNT = 0x00;
        private const int BLOCK_48_VECTOR = 0x01;
        #endregion

        #region Propeties
        public List<Block48> Block48List { get; set; } = new List<Block48>();
        #endregion

        public Block47() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            for (int i = 0; i < contents[0]; i++)
            {
                Block48List.Add(Deserialize<Block48>(contents, i * 2 + 1, codeplugContents));
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contentsLength = 1 + (2 * Block48List.Count);
            var contents = new byte[contentsLength].AsSpan();
            var nextAddress = address + contentsLength + BlockSizeAdjustment;

            contents[COUNT] = (byte)Block48List.Count;

            int i = 0;
            foreach (var block in Block48List)
            {
                contents[i * 2 + 1] = (byte)(nextAddress / 0x100);
                contents[i * 2 + 2] = (byte)(nextAddress % 0x100);

                nextAddress = SerializeChild(block, i * 2 + 1, codeplugContents, nextAddress, contents);
                i++;
            }

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
