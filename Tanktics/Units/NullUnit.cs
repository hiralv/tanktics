//Coded by Acey and robby
using System;
using System.Collections.Generic;
using System.Text;

namespace Tanktics
{
    class NullUnit: Unit
    {
        public NullUnit()
        {
            type = "null";
            team = 0;

            //used to check tiles surrounding null unit for visibility
            //must be max of apc, artillery, and tank vision values
            vision = 5;
        }
    }
}
