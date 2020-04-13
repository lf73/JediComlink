using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block08 : Block //Block Vector Array
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x08; }
        public override string Description { get => "Softpot Interpol Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 00 A1 00  B1 00 C1 00   D1 00 E1 00  F1
        */

        #endregion

        #region Propeties
        public List<Block09> Block09List { get; set; } = new List<Block09>();
        #endregion

        public Block08() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);

            for (int i = 0; i < Contents[0]; i++)
            {
                Block09List.Add(Deserialize<Block09>(Contents, i * 2 + 1, codeplugContents));
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;

            int i = 0;
            foreach (var block in Block09List)
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
            sb.AppendLine($"Block 09 Count: {Block09List.Count}");

            foreach (var block in Block09List)
            {
                sb.AppendLine(block.ToString());
            }

            return sb.ToString();
        }
    }
}
