using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block31 : Block
    {
        public override byte Id { get => 0x31; }
        public override string Description { get => "Radio Wide"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 01 14 03 28  1E 10 41 60   00 02 04 08  00 05 00 00
        1: 06 00 00 05  00 00 05 00   28 09 00 00  0A 0A 00 01
        2: 01 01 00 00  00 00 00 28   00 00 00 00  00 00 00 00
        3: 00 00 00 00  00 00
        */

        private const int CONTENTS_LENGTH = 0x36;
        private const int UNKNOWN1 = 0x00; //thru 0x35
        #endregion

        #region Propeties
        [DisplayName("Unknown Byte Values 1")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Unknown1 { get; set; }
        #endregion


        public Block31() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            Unknown1 = contents.Slice(UNKNOWN1, CONTENTS_LENGTH).ToArray();
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            Unknown1.AsSpan().CopyTo(contents.Slice(UNKNOWN1));
            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
