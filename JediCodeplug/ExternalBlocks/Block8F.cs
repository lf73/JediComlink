using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block8F : BlockLong
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override byte Id { get => 0x8F; }
        public override string Description { get => "Status List"; }

        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 08 53 54  53 20 31 20   53 54 53 20  32 20 53 54
        1: 53 20 33 20  53 54 53 20   34 20 53 54  53 20 35 20
        2: 53 54 53 20  36 20 53 54   53 20 37 20  53 54 53 20
        3: 38 20 00
        */

        #endregion

        public Block8F() { }

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
