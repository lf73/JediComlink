using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block91 : BlockLong
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override byte Id { get => 0x91; }
        public override string Description { get => "Message List"; }

        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 06 10 4D 53  47 20 31 20   4D 53 47 20  32 20 4D 53
        1: 47 20 33 20  4D 53 47 20   34 20 4D 53  47 20 35 20
        2: 4D 53 47 20  36 20 4D 53   47 20 37 20  4D 53 47 20
        3: 38 20 4D 53  47 20 39 20   4D 53 47 20  31 30 4D 53
        4: 47 20 31 31  4D 53 47 20   31 32 4D 53  47 20 31 33
        5: 4D 53 47 20  31 34 4D 53   47 20 31 35  4D 53 47 20
        6: 31 36 00
        */

        #endregion

        public Block91() { }

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
