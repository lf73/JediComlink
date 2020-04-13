using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public class Block07 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x07; }
        public override string Description { get => "Softpot Radio"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 59 12 11 18  15 00 70 00   48 4D 4D 4D  03 86 06 8C
        1: CB 00 6E 51
        */

        private const int UNKNOWN1 = 0x00; //to 13
        #endregion


        #region Propeties
        public byte[] Unknown1
        {
            get => Contents.Slice(UNKNOWN1, 20).ToArray();
            //set => XYZ = value; //TODO
        }
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 59 12 11 18  15 00 70 00   48 4D 4D 4D  03 86 06 8C
        1: CB 00 6E 51
        */

        #endregion

        public Block07() { }

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
            sb.AppendLine($"Unknown1 Bytes: {FormatHex(Unknown1)}");

            return sb.ToString();
        }
    }
}