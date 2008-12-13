using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tanktics
{
    public class TurnController
    {
        UnitController unitController;

        //Current Phase
        //0 = Unit Placement Phase
        //1 = Point Phase (Points are awarded to player)
        //2 = Movement Phase
        //3 = Combat Phase
        //4 = Purchase Phase
        //5 = Another Players turn phase
        public int phase = 0;
        public int totalAPC = 0;
        public int totalTank = 0;
        public int totalArtil = 0;

        public int MAXAPC = 3;
        public int MAXTANK = 2;
        public int MAXARTIL = 1;

        int startingSmallX;
        int startingSmallY;
        int startingBigX;
        int startingBigY;

        public int points = 0;
        int team;

        TurnController nextTurnController;



        public TurnController(UnitController UC, int theTeam, int ssX, int ssY, int sbX, int sbY)
        {
            unitController = UC;
            startingSmallX = ssX;
            startingSmallY = ssY;
            startingBigX = sbX;
            startingBigY = sbY;
            nextTurnController = this;
            team = theTeam;
        }

        //Sets the next turn controller to given turn controller
        //Acey Boyce
        public void setNext(TurnController TC)
        {
            nextTurnController = TC;
        }

        //Returns next turn controller
        //Acey Boyce
        public TurnController getNext()
        {
            return nextTurnController;
        }

        //Moves to the next Phase
        //Acey Boyce
        public void nextPhase()
        {
            if (phase == 0)
            {
                phase++;
            }

            phase++;
            if (phase == 3)
            {
                phase++;
                totalArtil = unitController.getNumUnits(team, "artillery");
                totalAPC = unitController.getNumUnits(team, "apc");
                totalTank = unitController.getNumUnits(team, "tank");
            }

                //If you can not buy units skip buy unit phase.
            if (phase == 4)
            {
                if ((points < 2) ||
                    ((totalArtil == 1) && (totalAPC == 3) && (totalTank == 2)))
                {
                    phase++;
                }
            }

            if (phase >= 6)
            {
                phase = 1;
                pointPhase();
                phase++;
            }
        }

        //Adds points based on Factory Control
        //Acey Boyce
        public void pointPhase()
        {
            points = points + calcPoints();
        }

        //Calulates current number of factories under the players control
        //Acey Boyce
        public int calcPoints()
        {
            int numFactories = 0;

            //factory 1
            if (factoryControlled(7, 2, 9, 4))
                numFactories++;
            //factory 2
            if (factoryControlled(2, 7, 4, 9))
                numFactories++;
            //factory 3
            if (factoryControlled(7, 7, 9, 9))
                numFactories++;
            //factory 4
            if (factoryControlled(12, 7, 14, 9))
                numFactories++;
            //factory 5
            if (factoryControlled(7, 12, 9, 14))
                numFactories++;

            #region old way...
            ////Factory 1
            ////If player has a unit in factory get a point
            //if (unitController.unitAt(7, 2).team == team ||
            //    unitController.unitAt(8, 2).team == team ||
            //    unitController.unitAt(9, 2).team == team ||
            //    unitController.unitAt(7, 3).team == team ||
            //    unitController.unitAt(9, 3).team == team ||
            //    unitController.unitAt(7, 4).team == team ||
            //    unitController.unitAt(8, 4).team == team ||
            //    unitController.unitAt(9, 4).team == team)
            //{
            //    numFactories++;
            //    //Unless of course there is another teams unit in there too!
            //    if ((unitController.unitAt(7, 2).team != team && unitController.unitAt(7, 2).team != 0) ||
            //    (unitController.unitAt(8, 2).team != team && unitController.unitAt(8, 2).team != 0) ||
            //    (unitController.unitAt(9, 2).team != team && unitController.unitAt(9, 2).team != 0) ||
            //    (unitController.unitAt(7, 3).team != team && unitController.unitAt(7, 3).team != 0) ||
            //    (unitController.unitAt(9, 3).team != team && unitController.unitAt(9, 3).team != 0) ||
            //    (unitController.unitAt(7, 4).team != team && unitController.unitAt(7, 4).team != 0) ||
            //    (unitController.unitAt(8, 4).team != team && unitController.unitAt(8, 4).team != 0) ||
            //    (unitController.unitAt(9, 4).team != team && unitController.unitAt(9, 4).team != 0))
            //    {
            //        numFactories--;
            //    }
            //}

            ////Factory 2
            ////If player has a unit in factory get a point
            //if (unitController.unitAt(2, 7).team == team ||
            //    unitController.unitAt(3, 7).team == team ||
            //    unitController.unitAt(4, 7).team == team ||
            //    unitController.unitAt(2, 8).team == team ||
            //    unitController.unitAt(4, 8).team == team ||
            //    unitController.unitAt(2, 9).team == team ||
            //    unitController.unitAt(3, 9).team == team ||
            //    unitController.unitAt(4, 9).team == team)
            //{
            //    numFactories++;
            //    //Unless of course there is another teams unit in there too!
            //    if ((unitController.unitAt(2, 7).team != team && unitController.unitAt(2, 7).team != 0) ||
            //    (unitController.unitAt(3, 7).team != team && unitController.unitAt(3, 7).team != 0) ||
            //    (unitController.unitAt(4, 7).team != team && unitController.unitAt(4, 7).team != 0) ||
            //    (unitController.unitAt(2, 8).team != team && unitController.unitAt(2, 8).team != 0) ||
            //    (unitController.unitAt(4, 8).team != team && unitController.unitAt(4, 8).team != 0) ||
            //    (unitController.unitAt(2, 9).team != team && unitController.unitAt(2, 9).team != 0) ||
            //    (unitController.unitAt(3, 9).team != team && unitController.unitAt(3, 9).team != 0) ||
            //    (unitController.unitAt(4, 9).team != team && unitController.unitAt(4, 9).team != 0))
            //    {
            //        numFactories--;
            //    }
            //}

            ////Factory 3
            ////If player has an APC in factory get a point
            //if ((unitController.unitAt(11, 11).team == team && unitController.unitAt(11, 11).type.Equals("apc")) ||
            //    (unitController.unitAt(11, 12).team == team && unitController.unitAt(11, 12).type.Equals("apc")) ||
            //    (unitController.unitAt(11, 13).team == team && unitController.unitAt(11, 13).type.Equals("apc")) ||
            //    (unitController.unitAt(12, 11).team == team && unitController.unitAt(12, 11).type.Equals("apc")) ||
            //    (unitController.unitAt(12, 13).team == team && unitController.unitAt(12, 13).type.Equals("apc")) ||
            //    (unitController.unitAt(13, 11).team == team && unitController.unitAt(13, 11).type.Equals("apc")) ||
            //    (unitController.unitAt(13, 12).team == team && unitController.unitAt(13, 12).type.Equals("apc")) ||
            //    (unitController.unitAt(13, 13).team == team && unitController.unitAt(13, 13).type.Equals("apc")))
            //{
            //    numFactories++;
            //    //Unless of course their is another teams unit in there too!
            //    if ((unitController.unitAt(11, 11).team != team && unitController.unitAt(11, 11).team != 0) ||
            //    (unitController.unitAt(11, 12).team != team && unitController.unitAt(11, 12).team != 0) ||
            //    (unitController.unitAt(11, 13).team != team && unitController.unitAt(11, 13).team != 0) ||
            //    (unitController.unitAt(12, 11).team != team && unitController.unitAt(12, 11).team != 0) ||
            //    (unitController.unitAt(12, 13).team != team && unitController.unitAt(12, 13).team != 0) ||
            //    (unitController.unitAt(13, 11).team != team && unitController.unitAt(13, 11).team != 0) ||
            //    (unitController.unitAt(13, 12).team != team && unitController.unitAt(13, 12).team != 0) ||
            //    (unitController.unitAt(13, 13).team != team && unitController.unitAt(13, 13).team != 0))
            //    {
            //        numFactories--;
            //    }
            //}

            ////Factory 4
            ////If player has an APC in factory get a point
            //if ((unitController.unitAt(19, 11).team == team && unitController.unitAt(19, 11).type.Equals("apc")) ||
            //    (unitController.unitAt(19, 12).team == team && unitController.unitAt(19, 12).type.Equals("apc")) ||
            //    (unitController.unitAt(19, 13).team == team && unitController.unitAt(19, 13).type.Equals("apc")) ||
            //    (unitController.unitAt(20, 11).team == team && unitController.unitAt(20, 11).type.Equals("apc")) ||
            //    (unitController.unitAt(20, 13).team == team && unitController.unitAt(20, 13).type.Equals("apc")) ||
            //    (unitController.unitAt(21, 11).team == team && unitController.unitAt(21, 11).type.Equals("apc")) ||
            //    (unitController.unitAt(21, 12).team == team && unitController.unitAt(21, 12).type.Equals("apc")) ||
            //    (unitController.unitAt(21, 13).team == team && unitController.unitAt(21, 13).type.Equals("apc")))
            //{
            //    numFactories++;
            //    //Unless of course their is another teams unit in there too!
            //    if ((unitController.unitAt(19, 11).team != team && unitController.unitAt(19, 11).team != 0) ||
            //    (unitController.unitAt(19, 12).team != team && unitController.unitAt(19, 12).team != 0) ||
            //    (unitController.unitAt(19, 13).team != team && unitController.unitAt(19, 13).team != 0) ||
            //    (unitController.unitAt(20, 11).team != team && unitController.unitAt(20, 11).team != 0) ||
            //    (unitController.unitAt(20, 13).team != team && unitController.unitAt(20, 13).team != 0) ||
            //    (unitController.unitAt(21, 11).team != team && unitController.unitAt(21, 11).team != 0) ||
            //    (unitController.unitAt(21, 12).team != team && unitController.unitAt(21, 12).team != 0) ||
            //    (unitController.unitAt(21, 13).team != team && unitController.unitAt(21, 13).team != 0))
            //    {
            //        numFactories--;
            //    }
            //}

            ////Factory 5
            ////If player has an APC in factory get a point
            //if ((unitController.unitAt(11, 19).team == team && unitController.unitAt(11, 19).type.Equals("apc")) ||
            //    (unitController.unitAt(11, 20).team == team && unitController.unitAt(11, 20).type.Equals("apc")) ||
            //    (unitController.unitAt(11, 21).team == team && unitController.unitAt(11, 21).type.Equals("apc")) ||
            //    (unitController.unitAt(12, 19).team == team && unitController.unitAt(12, 19).type.Equals("apc")) ||
            //    (unitController.unitAt(12, 21).team == team && unitController.unitAt(12, 21).type.Equals("apc")) ||
            //    (unitController.unitAt(13, 19).team == team && unitController.unitAt(13, 19).type.Equals("apc")) ||
            //    (unitController.unitAt(13, 20).team == team && unitController.unitAt(13, 20).type.Equals("apc")) ||
            //    (unitController.unitAt(13, 21).team == team && unitController.unitAt(13, 21).type.Equals("apc")))
            //{
            //    numFactories++;
            //    //Unless of course their is another teams unit in there too!
            //    if ((unitController.unitAt(11, 19).team != team && unitController.unitAt(11, 19).team != 0) ||
            //    (unitController.unitAt(11, 20).team != team && unitController.unitAt(11, 20).team != 0) ||
            //    (unitController.unitAt(11, 21).team != team && unitController.unitAt(11, 21).team != 0) ||
            //    (unitController.unitAt(12, 19).team != team && unitController.unitAt(12, 19).team != 0) ||
            //    (unitController.unitAt(12, 21).team != team && unitController.unitAt(12, 21).team != 0) ||
            //    (unitController.unitAt(13, 19).team != team && unitController.unitAt(13, 19).team != 0) ||
            //    (unitController.unitAt(13, 20).team != team && unitController.unitAt(13, 20).team != 0) ||
            //    (unitController.unitAt(13, 21).team != team && unitController.unitAt(13, 21).team != 0))
            //    {
            //        numFactories--;
            //    }
            //}
#endregion

            return numFactories;
        }

        //determine if a factory zone at the given location is controlled by this team
        //Robby Florence
        public Boolean factoryControlled(int startX, int startY, int endX, int endY)
        {
            Boolean controlled = false;

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    //unit on another team found, so this team cant control factory
                    if (unitController.unitAt(x, y).team > 0 && unitController.unitAt(x, y).team != team)
                        return false;

                    //unit on this team found, possible to conrol this factory
                    if (unitController.unitAt(x, y).team == team)
                        controlled = true;
                }
            }

            return controlled;
        }

        //Additional checks for creating a unit
        //Acey Boyce
        public Boolean createUnit(String type, int X, int Y)
        {
            //If not empty can't create unit 
            if (!unitController.isEmpty(X, Y))
            {
                return false;
            }
            
            //Must be in the players base
            if ((X >= startingSmallX) && (X <= startingBigX) && (Y >= startingSmallY) && (Y <= startingBigY))
            {
                //Must not excede maximum unit numbers
                if (type.Equals("artillery"))
                {
                    if (totalArtil < MAXARTIL)
                    {   

                        return true;
                    }
                }

                if (type.Equals("tank"))
                {
                    if (totalTank < MAXTANK)
                    {
                        return true;
                    }
                }

                if (type.Equals("apc"))
                {
                    if (totalAPC < MAXAPC)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


    }

}
