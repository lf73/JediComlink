using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JediComlink
{
    public class Codeplug
    {
        public byte[] Contents { get; protected set; }
        public Block01 Root { get; protected set; }

        public Codeplug(string path)
        {
            Contents = File.ReadAllBytes(path);
            Root = new Block01(this);
        }

        public string GetText()
        {
            return Root.ToString();
        }


    }
}
