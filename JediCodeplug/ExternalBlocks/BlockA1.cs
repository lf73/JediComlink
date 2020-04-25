using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class BlockA1 : Block //Block Vector Array
    {
        public override byte Id { get => 0xA1; }
        public override string Description { get => "Singletone System Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 04 9E
        */

        private const int COUNT = 0x00;
        private const int BLOCK_A2_VECTOR = 0x01;
        #endregion

        #region Propeties
        public List<BlockA2> BlockA2List { get; set; } = new List<BlockA2>();
        #endregion

        public BlockA1() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            for (int i = 0; i < contents[0]; i++)
            {
                BlockA2List.Add(Deserialize<BlockA2>(contents, i * 2 + 1, codeplugContents));
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contentsLength = 1 + (2 * BlockA2List.Count);
            var contents = new byte[contentsLength].AsSpan();
            var nextAddress = address + contentsLength + BlockSizeAdjustment;

            contents[COUNT] = (byte)BlockA2List.Count;

            int i = 0;
            foreach (var block in BlockA2List)
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
