using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block37 : BlockLong //Block Vector Array
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x37; }
        public override string Description { get => "Zone Chan Text Vector"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 04 D6
        */

        private const int BLOCK_38_VECTOR = 0x01;
        #endregion

        #region Propeties
        public List<Block38> Block38List { get; set; } = new List<Block38>();
        #endregion

        public Block37() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);

            for (int i = 0; i < Contents[0]; i++)
            {
                Block38List.Add(Deserialize<Block38>(Contents, i * 2 + 1, codeplugContents));
            }
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;

            int i = 0;
            foreach (var block in Block38List)
            {
                nextAddress = SerializeChild(block, i * 2 + 1, codeplugContents, nextAddress, contents);
                i++;
            }

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
