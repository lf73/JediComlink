using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class BlockDE : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override byte Id { get => 0xDE; }
        public override string Description { get => "Trunk II FS Channel Rebanded"; }

        #region Propeties
            #endregion

            #region Definition
            /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
            0: 1F FF F0 00  00 01 00 10
            */

            #endregion

        public BlockDE() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
