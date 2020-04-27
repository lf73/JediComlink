using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block63 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override byte Id { get => 0x63; }
        public override string Description { get => "Trunk II TG"; }

        #region Propeties
        public Block65 Block65 { get; set; }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 1F FF F0 00  00 01 00 10
        */

        //The block size varies considerably.
        //Sometimes has a reference to Block 65.
        //Guess for now if first byte is 9F, Bytes 3/4 cotain vector to Block 65
        #endregion

        public Block63() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);

            if (Contents[0] == 0x9f) //TODO... Total Stab in the dark right now
            {
                Block65 = Deserialize<Block65>(Contents, 0x03, codeplugContents);
            }

        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;

            if (Contents[0] == 0x9f) //TODO... Total Stab in the dark right now
            {
                nextAddress = SerializeChild(Block65, 0x03, codeplugContents, nextAddress, contents);
            }

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }
    }
}
