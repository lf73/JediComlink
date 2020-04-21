using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JediCommunication
{
    static class SB9600Messages
    {
        public static readonly SB9600Message EnterSbep = new SB9600Message(0x00, 0x12, 0x01, 0x06);
        public static readonly SB9600Message ProgrammingMode = new SB9600Message(0x01, 0x02, 0x00, 0x40);
        public static readonly SB9600Message Reset = new SB9600Message(0x00, 0x00, 0x01, 0x08);

    }
}
