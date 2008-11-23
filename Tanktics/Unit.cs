using System;
using System.Collections.Generic;
using System.Text;

namespace Tanktics
{
    class Unit
    {
        public Boolean hasMoved;
        public int vision;
        public Boolean canWater;
        public int movement;
        public int team;
        public int cost;
        public int currentX;
        public int currentY;
        public int previousX;
        public int previousY;
        public AnimatingSprite sprite;
        public String type;
        public int unitNumber;


        public void sethasMoved(Boolean toSet)
        {
            hasMoved = toSet;
        }

        //Sets prevX and prevY to currX and CurrY
        //Used at end of turn
        public void updatePreXandY()
        {
            previousX = currentX;
            previousY = currentY;
            hasMoved = false;
        }

    }
}
