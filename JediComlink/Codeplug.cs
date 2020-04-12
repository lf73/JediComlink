using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JediComlink
{
    public class Codeplug
    {
        public Block01 InternalCodeplug { get; protected set; }

        public Block30 ExternalCodeplug { get; protected set; }

        public Codeplug(string path)
        {
            var contents = File.ReadAllBytes(path);

            InternalCodeplug = new Block01();
            InternalCodeplug.Deserialize(contents, 0);


        }

        public string GetText()
        {
            return InternalCodeplug.ToString();
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[InternalCodeplug.InternalCodeplugSize];
            InternalCodeplug.Serialize(bytes, 0);
            return bytes;
        }
    }
}
