using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Codeplug
    {
        public Block01 InternalCodeplug { get; protected set; }

        public Block30 ExternalCodeplug { get; protected set; }

        public Codeplug(string path)
        {           
            var contents = File.ReadAllBytes(path);
            InternalCodeplug = new Block01(contents);
        }

        public string GetText()
        {
            return InternalCodeplug.ToString();
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[InternalCodeplug.InternalCodeplugSize];
            int i = 0;
            var blockBytes = InternalCodeplug.Serialize();
            Array.Copy(blockBytes, 0, bytes, i, blockBytes.Length);
            //i += blockBytes.Length;
            //foreach (var block in Children)
            //{
            //    blockBytes = block.GetBytes();
            //    Array.Copy(blockBytes, 0, bytes, i, blockBytes.Length);
            //    i += blockBytes.Length;
            //}

            return bytes;
        }


    }
}
