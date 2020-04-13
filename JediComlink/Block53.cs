using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Block53 : Block
    {
        private byte[] _contents;
        public Span<byte> Contents { get => _contents; set => _contents = value.ToArray(); }

        public override int Id { get => 0x53; }
        public override string Description { get => "Scan List"; }

        #region Propeties
        #endregion

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 00 FF FF 00  0F 00 00 00   00 00 00 00  00 00 00 00
        1: 00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00
        2: 00 00 00
        */

        #endregion


        public Block53() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            Contents = Deserializer(codeplugContents, address);
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = Contents.ToArray().AsSpan(); //TODO
            var nextAddress = address + Contents.Length + BlockSizeAdjustment;

            //TODO Not sure why this appears in at least two files. Appears to be junk?
            /* C:\JediDumps\466AWA2867H01UCD6PW1BN-Codeplug.bin (12/10/2019 5:02:40 PM)
               StartOffset(h): 00000855, EndOffset(h): 0000088E, Length(h): 0000003A */

            byte[] rawData = {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x25, 0x33, 0x00, 0x00,
                0x01, 0x01, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x37,
                0x0A, 0x0D, 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x05, 0x00, 0x00, 0x20, 0xD0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0xF3, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            rawData.CopyTo(codeplugContents.AsSpan(nextAddress));
            nextAddress += rawData.Length;

            Serializer(codeplugContents, address, contents);
            return nextAddress;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetTextHeader());

            return sb.ToString();
        }
    }
}
