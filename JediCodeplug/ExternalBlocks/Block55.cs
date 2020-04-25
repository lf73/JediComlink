using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block55 : BlockLong //Block Vector Array
    {
        public override byte Id { get => 0x55; }
        public override string Description { get => "Personality Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 11 05 64 05  8A 05 B0 05   D6 05 FC 06  22 06 48 06
        1: 6E 06 94 06  BA 06 E0 07   06 07 2C 07  52 07 78 07
        2: 9E 07 C4
        */

        private const int COUNT = 0x00;
        private const int BLOCK_56_VECTOR = 0x01;
        #endregion

        #region Propeties
        public List<Block> Block56or62List { get; set; } = new List<Block>();
        #endregion

        public Block55() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            for (int i = 0; i < contents[0]; i++)
            {
                var childAddress = contents[i * 2 + 1] * 0x100 + contents[i * 2 + 2];
                if (codeplugContents[childAddress + 1] == 0x56)
                {
                    Block56or62List.Add(Deserialize<Block56>(contents, i * 2 + 1, codeplugContents));
                }
                else
                {
                    Block56or62List.Add(Deserialize<Block62>(contents, i * 2 + 1, codeplugContents));
                }
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contentsLength = 1 + (2 * Block56or62List.Count);
            var contents = new byte[contentsLength].AsSpan();
            var nextAddress = address + contentsLength + BlockSizeAdjustment;

            contents[COUNT] = (byte)Block56or62List.Count;

            int i = 0;
            foreach (var block in Block56or62List)
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
