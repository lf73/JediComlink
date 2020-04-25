using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JediCodeplug
{
    public enum ProgrammingSources : byte
    {
        Lab,

        Factory,

        Debug,

        CPS,

        [Description("DOS RSS")]
        DOSRSS,

        Labtool,

        [Description("Beta CPS")]
        BetaCPS,

        [Description("Radio Cloning")]
        RadioCloning,

        ASTS
    }

}
