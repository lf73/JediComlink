using System;
using System.IO;

namespace JediCodeplug
{
    public class Codeplug
    {
        public Block01 InternalCodeplug { get; protected set; } = new Block01();

        public Block30 ExternalCodeplug { get; protected set; } = new Block30();

        public Codeplug(string path)
        {
            var contents = File.ReadAllBytes(path);
            InternalCodeplug.Deserialize(contents, 0);
            ExternalCodeplug.Deserialize(contents, InternalCodeplug.ExternalCodeplugVector);
        }

        public string GetTextDump()
        {
            return InternalCodeplug?.GetTextDump() ?? "" + ExternalCodeplug?.GetTextDump() ?? "";
        }

        public byte[] Serialize()
        {
            byte[] bytes = new byte[InternalCodeplug.ExternalCodeplugVector + ExternalCodeplug.ExternalCodeplugSize];
            InternalCodeplug.Serialize(bytes, 0);
            ExternalCodeplug.Serialize(bytes, InternalCodeplug.ExternalCodeplugVector);

            byte[] rawData = {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0xB5, 0x00, 0x00,
                0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x62, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            rawData.CopyTo(bytes.AsSpan(bytes.Length - rawData.Length));



            return bytes;
        }
    }
}
