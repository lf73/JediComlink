using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JediComlink
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

        public string GetText()
        {
            return InternalCodeplug.ToString() + Environment.NewLine + Environment.NewLine
                + Environment.NewLine + ExternalCodeplug.ToString();
        }

        public byte[] Serialize()
        {
            byte[] bytes = new byte[InternalCodeplug.ExternalCodeplugVector + ExternalCodeplug.ExternalCodeplugSize];
            InternalCodeplug.Serialize(bytes, 0);
            ExternalCodeplug.Serialize(bytes, InternalCodeplug.ExternalCodeplugVector);
            return bytes;
        }
    }
}
