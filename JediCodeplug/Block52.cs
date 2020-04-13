using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block52 : Block //Block Vector Array
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x52; }
        public override string Description { get => "Scan List Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 08 2F
        */

        private const int BLOCK_53_VECTOR = 0x01;
        #endregion

        #region Propeties
        public List<Block53> Block53List { get; set; } = new List<Block53>();
        #endregion

        public Block52() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            for (int i = 0; i < Contents[0]; i++)
            {
                Block53List.Add(Deserialize<Block53>(Contents, i * 2 + 1, codeplugContents));
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;

            int i = 0;
            foreach (var block in Block53List)
            {
                nextAddress = SerializeChild(block, i * 2 + 1, codeplugContents, nextAddress, contents);
                i++;
            }

            //TODO Not sure why this appears in at least two files.
            //Might be for User Configured Custom Scan Lists.
            byte[] rawData = {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x25, 0x33, 0x00, 0x00,
                0x01, 0x01, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x37,
                0x0A, 0x0D, 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x05, 0x00, 0x00, 0x20, 0xD0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0xF3, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            rawData.CopyTo(codeplugContents.AsSpan(nextAddress));
            nextAddress += rawData.Length;

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine($"Block 53 Count: {Block53List.Count}");

            foreach (var block in Block53List)
            {
                sb.AppendLine(block.ToString());
            }

            return sb.ToString();
        }
    }
}
