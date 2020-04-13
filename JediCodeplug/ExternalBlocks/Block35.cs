using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block35 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x35; }
        public override string Description { get => "Unknown"; }

        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        1: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        2: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        3: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        4: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        5: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        6: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        7: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        8: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        9: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        A: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        B: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        C: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        D: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        E: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        F: 00 00 00 00  00 00 00 00   00 00 00 00  00
        */

        #endregion

        public Block35() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            return Serializer(codeplugContents, address, contents) + address;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
