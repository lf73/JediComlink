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
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x47; }
        public override string Description { get => "MDC System Vector"; }
        
        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 04 35
        */

        private const int BLOCK_48_VECTOR = 0x01;
        #endregion

        #region Propeties
        public List<Block48> Block48List { get; set; } = new List<Block48>();
        #endregion

        public Block47() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
            for (int i = 0; i < Contents[0]; i++)
            {
                Block48List.Add(Deserialize<Block48>(Contents, i * 2 + 1, codeplugContents));
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;

            int i = 0;
            foreach (var block in Block48List)
            {
                nextAddress = SerializeChild(block, i * 2 + 1, codeplugContents, nextAddress, contents);
                i++;
            }

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());
            sb.AppendLine($"Block 48 Count: {Block48List.Count}");

            foreach (var block in Block48List)
            {
                sb.AppendLine(block.ToString());
            }

            return sb.ToString();
        }
    }
}