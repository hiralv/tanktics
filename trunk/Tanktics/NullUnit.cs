using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tanktics
{
    class NullUnit: Unit
    {
        public NullUnit()
        {
            type = "null";
            team = 0;
        }
    }
}
