using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block4B : Block //Block Vector Array
    {
        public override byte Id { get => 0x4B; }
        public override string Description { get => "Trunk System Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 04 06
        */

        private const int COUNT = 0x00;
        private const int BLOCK_5A_VECTOR = 0x01;
        #endregion

        #region Propeties
        public List<Block5A> Block5AList { get; set; } = new List<Block5A>();
        #endregion

        public Block4B() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            for (int i = 0; i < contents[0]; i++)
            {
                Block5AList.Add(Deserialize<Block5A>(contents, i * 2 + 1, codeplugContents));
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contentsLength = 1 + (2 * Block5AList.Count);
            var contents = new byte[contentsLength].AsSpan();
            var nextAddress = address + contentsLength + BlockSizeAdjustment;

            contents[COUNT] = (byte)Block5AList.Count;

            int i = 0;
            foreach (var block in Block5AList)
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
