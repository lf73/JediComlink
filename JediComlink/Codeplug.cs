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
        public Block01 Root { get; protected set; }

        public List<Block> Children { get; protected set; } = new List<Block>();

        public Codeplug(string path)
        {           
            var contents = File.ReadAllBytes(path);
            Root = new Block01(this, contents);
        }

        public string GetText()
        {
            return Root.ToString();
        }


    }
}
