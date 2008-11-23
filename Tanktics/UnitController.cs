using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tanktics
{
    class UnitController
    {
        //Maximum allowed units per team
        static int MAXIMUMUNITS = 20;

        //Total Units created thus far
        //Used to give a distinct and special number to each unit
        int totalUnitsMade = 0;
        
        Unit[,] currentBoard;
        Unit[,] originalBoard;
        int xSize;
        int ySize;
        
        //Simple Boolean 2D array
        //0 No Water
        //1 Water HumVee onry!
        Boolean[,] Water;

        Unit[] team1 = new Unit[MAXIMUMUNITS];
        int team1Length = 0;
        Unit[] team2 = new Unit[MAXIMUMUNITS];
        int team2Length= 0;
        Unit[] team3 = new Unit[MAXIMUMUNITS];
        int team3Length = 0;
        Unit[] team4 = new Unit[MAXIMUMUNITS];
        int team4Length = 0;

        Boolean[] inGame = new Boolean[4];

        Unit[] actionsTakenThisTurn = new Unit[MAXIMUMUNITS];
        int totalactions;
        public Unit currentUnit;
        int currentUnitNum = 0;

        int currentPlayer;

        Unit[] unitsKilledThisTurn = new Unit[MAXIMUMUNITS];
        int numUnitsKilled = 0;

        //Value to multiply the array index by to get the screen position of sprites
        int spriteAdjusterX;
        int spriteAdjusterY;


        //Creates a new Unit Controller and sets the number of players
        //DOES NOT:
        //Add or create any units
        //Set the values of the Water Array
        //ATM these must be done with seperate calls
        public UnitController(int width, int height, int numOfPlayers)
        {
            currentBoard = new Unit[height,width];
            originalBoard = new Unit[height, width];
            Water = new Boolean[height, width];

            ySize = height;
            xSize = width;

            spriteAdjusterX = 25;
            spriteAdjusterY = 25;
                                    
            int i = 0;
            int j = 0;

            while (i < ySize)
            {
                j = 0;
                while (j < xSize)
                {
                                
                    currentBoard[i, j] = new NullUnit();
                    originalBoard[i, j] = new NullUnit();
                    Water[i, j] = false;
                    j++;
                }
                i++;
            }

            i = 0;
            while (i < numOfPlayers)
            {
                inGame[i] = true;
                i++;
            }
            currentPlayer = 1;

            i = 0;
            while (i < MAXIMUMUNITS){
                unitsKilledThisTurn[i] = new NullUnit();
                i++;
            }
        }

        //Used for making water spaces on the map
        //Could pass water array into constructor but I do not know how to pass and then manipulate
        // 2D arrays in XNA.
        public void setWater(int x, int y)
        {
            Water[y, x] = true;
        }

        //Removes the player from the player rotation and removes their pieces from currentBoard
        // Note: You just lost the game
        public void loseTheGame(int Loser)
        {
            inGame[Loser - 1] = false;

            int y = 0;
            int x = 0;
            while (y < ySize)
            {
                x = 0;
                while (x < xSize)
                {
                    if (currentBoard[y, x].team == Loser)
                    {
                        currentBoard[y, x] = new NullUnit();
                    }
                    x++;
                }
                y++;
            }
            int i = 0;
            if (Loser == 1)
            {
                while (i < team1Length)
                {
                    team1[i] = new NullUnit();
                    i++;
                }
                team1Length = 0;
            }
            else if (Loser == 2)
            {
                while (i < team2Length)
                {
                    team2[i] = new NullUnit();
                    i++;
                }
                team2Length = 0;
            }
            else if (Loser == 3)
            {
                while (i < team3Length)
                {
                    team3[i] = new NullUnit();
                    i++;
                }
                team3Length = 0;
            }
            else 
            {
                while (i < team4Length)
                {
                    team4[i] = new NullUnit();
                    i++;
                }
                team4Length = 0;
            }
        }

        //Returns true if active player has more units
        //False otherwise
        public Boolean hasMoreUnits()
        {
            Boolean foundOne = false;
            int i = 0;

            if (currentPlayer == 1)
            {
                while (i < team1Length)
                {
                    if (!team1[i].hasMoved)
                    {
                        foundOne = true;
                    }
                    i++;
                }
            }

            else if (currentPlayer == 2)
            {
                while (i < team2Length)
                {
                    if (!team2[i].hasMoved)
                    {
                        foundOne = true;
                    }
                    i++;
                }
            }
            else if (currentPlayer == 3)
            {
                while (i < team3Length)
                {
                    if (!team3[i].hasMoved)
                    {
                        foundOne = true;
                    }
                    i++;
                }
            }
            else 
            {
                while (i < team4Length)
                {
                    if (!team4[i].hasMoved)
                    {
                        foundOne = true;
                    }
                    i++;
                }
            }
            return foundOne;
        }

        //Selects next unit owned by active player
        public void nextUnit()
        {   
            //if the current player has no more unused units
            //do not bother doing this method
            //I would also suggest making the nextUnit button unselectable if
            //hasMoreUnits is true
            if (hasMoreUnits())
            {
                currentUnitNum++;
                if (currentPlayer == 1)
                {
                    if (currentUnitNum == team1Length)
                    {
                        currentUnitNum = 0;
                    }
                    currentUnit = team1[currentUnitNum];
                }
                if (currentPlayer == 2)
                {
                    if (currentUnitNum == team2Length)
                    {
                        currentUnitNum = 0;
                    }
                    currentUnit = team2[currentUnitNum];
                }
                if (currentPlayer == 3)
                {
                    if (currentUnitNum == team3Length)
                    {
                        currentUnitNum = 0;
                    }
                    currentUnit = team3[currentUnitNum];
                }
                if (currentPlayer == 4)
                {
                    if (currentUnitNum == team4Length)
                    {
                        currentUnitNum = 0;
                    }
                    currentUnit = team4[currentUnitNum];
                }


                //If the next in line has already moved call again to get next
                if (currentUnit.hasMoved)
                {
                    nextUnit();
                }
            }

        }

        //Adds a Unit to the Board
        public Boolean addUnit(String setType, int setTeam, int startingX, int startingY, Texture2D SpriteSheet)
        {
            Unit newUnit;
            if (setType.Equals("artillery"))
            {
                newUnit = new Artillery(setTeam, startingX, startingY, SpriteSheet, totalUnitsMade);
            }
            else if (setType.Equals("humvee"))
            {
                newUnit = new HumVee(setTeam, startingX, startingY, SpriteSheet, totalUnitsMade);
            }
            else
            {
                newUnit = new Tank(setTeam, startingX, startingY, SpriteSheet, totalUnitsMade);
            }
            totalUnitsMade++;

            if (newUnit.team == 1)
            {
                if (team1Length < MAXIMUMUNITS)
                {
                    currentBoard[newUnit.currentY, newUnit.currentX] = newUnit;
                    team1[team1Length] = newUnit;
                    team1Length++;
                    return true;
                }
            }
            else if (newUnit.team == 2)
            {
                if (team2Length < MAXIMUMUNITS)
                {
                    currentBoard[newUnit.currentY, newUnit.currentX] = newUnit;
                    team2[team2Length] = newUnit;
                    team2Length++;
                    return true;
                }
            }
            else if (newUnit.team == 3)
            {
                if (team3Length < MAXIMUMUNITS)
                {
                    currentBoard[newUnit.currentY, newUnit.currentX] = newUnit;
                    team3[team3Length] = newUnit;
                    team3Length++;
                    return true;
                }
            }
            else if (newUnit.team == 4)
            {
                if (team4Length < MAXIMUMUNITS)
                {
                    currentBoard[newUnit.currentY, newUnit.currentX] = newUnit;
                    team4[team4Length] = newUnit;
                    team4Length++;
                    return true;
                }
            }
            else
            {
                currentBoard[newUnit.currentY, newUnit.currentX] = newUnit;
                return true;
            }
            return false;
        }

        public void finalize()
        {
            //Step 1:
            //Play animation for each movement in the actions Taken array
            //To be added later... maybe
            

            //Step 2:
            //Reset actionsTaken array
            int i = 0;
            while (i < MAXIMUMUNITS)
            {
                actionsTakenThisTurn[i] = new NullUnit();
                i++;
            }
            totalactions = 0;
            
            //Step 3:
            //Update the Original Board for next turn
            int y = 0;
            int x = 0;
            while (y < ySize)
            {
                x = 0;
                while (x < xSize)
                {
                    originalBoard[y,x] = currentBoard[y, x];
                    if (originalBoard[y,x].team != 0)
                    {
                        originalBoard[y,x].updatePreXandY();
                    }
                    x++;
                }
                y++;
            }
            //Step 4: 
            //Update the Starting X and Y for each unit of that team
            i = 0;
            if (currentPlayer == 1)
            {
                while (i < team1Length)
                {
                    team1[i].updatePreXandY();
                    i++;
                }
            }
            else if (currentPlayer == 2)
            {
                while (i < team2Length)
                {
                    team2[i].updatePreXandY();
                    i++;
                }
            }
            else if (currentPlayer == 3)
            {
                while (i < team3Length)
                {
                    team3[i].updatePreXandY();
                    i++;
                }
            }
            else
            {
                while (i < team4Length)
                {
                    team4[i].updatePreXandY();
                    i++;
                }
            }
            //Step 5:
            //Remove all killed units from unit lists
            i = 0;
            while (!(unitsKilledThisTurn[i].type.Equals("null")))
            {
                removeUnit(unitsKilledThisTurn[i].unitNumber, unitsKilledThisTurn[i].team);
                i++;
            }


            //Step 6:
            //Move to next player and select his first unit
            //NOTE:
            //An advance wars style black screen may want to be added before this step
            //to prevent players from seeing each others screens

            currentPlayer++;
            if (currentPlayer == 5)
                currentPlayer = 1;
            while (inGame[currentPlayer-1]== false)
            {
                currentPlayer++;
                if (currentPlayer == 5)
                    currentPlayer = 1;
            }
            if (currentPlayer == 1)
                currentUnit = team1[0];
            if (currentPlayer == 2)
                currentUnit = team2[0];
            if (currentPlayer == 3)
                currentUnit = team3[0];
            else
                currentUnit = team4[0];
        }


        //Movement specifics for HumVees
        //ASUMPTION: the game will never feed it a position off the board
        //           for example it will never make a call goal(-1, 4)
        //Returns:  0 if Trying to move greater than maximum movement range of unit
        //          1 if legal move
        //          2 if Unit has already made a move this turn;
        //          3 if Other Unit(s) is blocking all paths to the space
        //          9 End of move method reached. Unknown Error.
        public int moveHumVee(int goalX, int goalY)
        {
            //Check if unit has already moved this turn
            if (currentUnit.hasMoved)
                return 2;
            
            //Check if within absolute value range
            int absoluteDistance = Math.Abs(goalX - currentUnit.currentX) + Math.Abs(goalY - currentUnit.currentY);
            if (absoluteDistance > currentUnit.movement)
            {
                return 0;
            }
            //Location comparison Booleans
            // True if goal is ____ (of) unit.
            Boolean right;
            Boolean left;
            Boolean above;
            Boolean below;

            if (goalX > currentUnit.currentX)
            {
                right = true;
                left = false;
            }
            else if (goalX < currentUnit.currentX)
            {
                right = false;
                left = true;
            }
            else
            {
                right = false;
                left = false;
            }
            if (goalY > currentUnit.currentY)
            {
                below = true;
                above = false;
            }
            else if (goalY < currentUnit.currentY)
            {
                below = false;
                above = true;
            }
            else
            {
                below = false;
                above = false;
            }

            //If unit is not trying to go anywhere (ie. unit is being told to be removed
            //from the next unit line) it is valid move
            if ((!right && !left && !below && !above))
            {
                return 1;
            }
            //If player is trying to go one space left
            if (left && !above && !below && (absoluteDistance == 1))
            {
                //if the unit to the left is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team)
                {
                    return 3;
                }
                //if there is no unit to the left  
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX - 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //If player is trying to go one space right
            if (right && !above && !below && (absoluteDistance == 1))
            {
                //if the unit to the right is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team)
                {
                    return 3;
                }
                //if there is no unit to the right  
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX + 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go one space above
            if (above && !right && !left && (absoluteDistance == 1))
            {
                //if the unit above is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY-1, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is no unit above
                else if (currentBoard[currentUnit.currentY-1, currentUnit.currentX].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY -1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY-1, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY -1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go one space below
            if (below && !right && !left && (absoluteDistance == 1))
            {
                //if the unit below is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is no unit below
                else if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 1, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }

            //if player is trying to go two spaces left
            if (left && !above && !below && (absoluteDistance == 2))
            {
                //if there is any unit 1 space left you can not go
                if (currentBoard[currentUnit.currentY, currentUnit.currentX-1].team!=0)
                {
                    return 3;
                }
                //if the unit 2 to the left is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX - 2].team)
                {
                    return 3;
                }
                //if there is no unit 2 to the left  
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX - 2].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX - 2];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go two spaces right
            if (right && !above && !below && (absoluteDistance == 2))
            {
                //if there is any unit 1 space right you can not go
                if (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0)
                {
                    return 3;
                }
                //if the unit 2 to the right is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX + 2].team)
                {
                    return 3;
                }
                //if there is no unit 2 to the right 
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX + 2].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX + 2];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go two spaces above
            if (above && !right && !left && (absoluteDistance == 2))
            {
                //if there is any unit 1 space above you can not go
                if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                {
                    return 3;
                }
                //if the unit 2 above is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY - 2, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is no unit 2 above  
                else if (currentBoard[currentUnit.currentY - 2, currentUnit.currentX].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 2, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go two spaces below
            if (below && !right && !left && (absoluteDistance == 2))
            {
                //if there is any unit 1 space below you can not go
                if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                {
                    return 3;
                }
                //if the unit 2 below is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY + 2, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is no unit 2 below  
                else if (currentBoard[currentUnit.currentY + 2, currentUnit.currentX].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 2, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }

            //if player is trying to go above and left
            if (above && left)
            {
                //if the unit above and left is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1].team)
                {
                    return 3;
                }
                //if there is any unit directy above AND left then the unit can not go
                else if ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                {
                    return 3;
                }
                //if there is no unit above and left
                else if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go above and right
            if (above && right)
            {
                //if the unit above and right is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1].team)
                {
                    return 3;
                }
                //if there is any unit directy above AND right then the unit can not go
                else if ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                {
                    return 3;
                }
                //if there is no unit above and right
                else if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go below and left
            if (below && left)
            {
                //if the unit below and left is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team)
                {
                    return 3;
                }
                //if there is any unit directy below AND left then the unit can not go
                else if ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                {
                    return 3;
                }
                //if there is no unit below and left
                else if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
             //if player is trying to go below and right
            if (below && right)
            {
                //if the unit below and right is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team)
                {
                    return 3;
                }
                //if there is any unit directy below AND right then the unit can not go
                else if ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                {
                    return 3;
                }
                //if there is no unit below and right
                else if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }

            return 9;
        }

        //Movement specifics for Artillery
        //ASUMPTION: the game will never feed it a position off the board
        //           for example it will never make a call goal(-1, 4)
        //Returns:  0 if Trying to move greater than maximum movement range of unit
        //          1 if legal move
        //          2 if Unit has already made a move this turn;
        //          3 if Other Unit(s) is blocking all paths to the space
        //          4 if trying to enter or pass through water
        //          5 if Special Artillary Error
        //               Attempt to shoot or move to enemy diagonally
        //          9 End of move method reached. Unknown Error.
        public int moveArtillery(int goalX, int goalY)
        {
            //Check if unit has already moved this turn
            if (currentUnit.hasMoved)
                return 2;

            //Check if within absolute value range
            int absoluteDistance = Math.Abs(goalX - currentUnit.currentX) + Math.Abs(goalY - currentUnit.currentY);
            if (absoluteDistance > currentUnit.movement)
            {
                return 0;
            }
            //Location comparison Booleans
            // True if goal is ____ (of) unit.
            Boolean right;
            Boolean left;
            Boolean above;
            Boolean below;

            if (goalX > currentUnit.currentX)
            {
                right = true;
                left = false;
            }
            else if (goalX < currentUnit.currentX)
            {
                right = false;
                left = true;
            }
            else
            {
                right = false;
                left = false;
            }
            if (goalY > currentUnit.currentY)
            {
                below = true;
                above = false;
            }
            else if (goalY < currentUnit.currentY)
            {
                below = false;
                above = true;
            }
            else
            {
                below = false;
                above = false;
            }

            //If unit is not trying to go anywhere (ie. unit is being told to be removed
            //from the next unit line) it is valid move
            if ((!right && !left && !below && !above))
            {
                return 1;
            }
            //If player is trying to go one space left
            if (left && !above && !below && (absoluteDistance == 1))
            {
                //if the unit to the left is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team)
                {
                    return 3;
                }
                //if there is a unit (but not your unit)
                if (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0)
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX - 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX - 1] = new NullUnit();
                    return 1;
                }
                //if there is water to the left
                else if (Water[currentUnit.currentY, currentUnit.currentX - 1])
                {
                    return 4;
                }
                //else there is no water, no friendly, and no enemy so move 
                else 
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //If player is trying to go one space right
            if (right && !above && !below && (absoluteDistance == 1))
            {
                //if the unit to the right is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team)
                {
                    return 3;
                }
                //if there is a unit (but not your unit)
                if (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0)
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX + 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX + 1] = new NullUnit();
                    return 1;
                }
                //if there is water to the right
                else if (Water[currentUnit.currentY, currentUnit.currentX + 1])
                {
                    return 4;
                }
                //else there is no water, no friendly, and no enemy so move 
                else
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go one space above
            if (above && !right && !left && (absoluteDistance == 1))
            {
                //if the unit above is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is a unit (but not your unit)
                if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 1, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY - 1, currentUnit.currentX] = new NullUnit();
                    return 1;
                }
                //if there is water above
                else if (Water[currentUnit.currentY - 1, currentUnit.currentX])
                {
                    return 4;
                }
                //else there is no water, no friendly, and no enemy so move 
                else
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go one space below
            if (below && !right && !left && (absoluteDistance == 1))
            {
                //if the unit below is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is a unit (but not your unit)
                if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 1, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY + 1, currentUnit.currentX] = new NullUnit();
                    return 1;
                }
                //if there is water below
                else if (Water[currentUnit.currentY + 1, currentUnit.currentX])
                {
                    return 4;
                }
                //else there is no water, no friendly, and no enemy so move 
                else
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }

            //if player is trying to go two spaces left
            if (left && !above && !below && (absoluteDistance == 2))
            {
                //if the unit 2 to the left is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX - 2].team)
                {
                    return 3;
                }
                //if there is unit (but not your unit)
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX - 2].team != 0)
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX - 2];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX - 2] = new NullUnit();
                    return 1;
                }
                //if there is no unit 2 to the left  
                else 
                {
                    if (Water[currentUnit.currentY, currentUnit.currentX - 1]
                        || Water[currentUnit.currentY, currentUnit.currentX - 2]) 
                    {
                        return 4;
                    }
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go two spaces right
            if (right && !above && !below && (absoluteDistance == 2))
            {
                //if the unit 2 to the right is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX + 2].team)
                {
                    return 3;
                }
                //if there is unit (but not your unit)
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX + 2].team != 0)
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX + 2];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX + 2] = new NullUnit();
                    return 1;
                }
                //if there is no unit 2 to the right  
                else
                {
                    if (Water[currentUnit.currentY, currentUnit.currentX + 1]
                        || Water[currentUnit.currentY, currentUnit.currentX + 2])
                    {
                        return 4;
                    }
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go two spaces above
            if (above && !right && !left && (absoluteDistance == 2))
            {
                //if the unit 2 above is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 2, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is unit (but not your unit)
                else if (currentBoard[currentUnit.currentY - 2, currentUnit.currentX].team != 0)
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 2, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY - 2, currentUnit.currentX] = new NullUnit();
                    return 1;
                }
                //if there is no unit 2 above 
                else
                {
                    if (Water[currentUnit.currentY - 1, currentUnit.currentX]
                        || Water[currentUnit.currentY - 2, currentUnit.currentX])
                    {
                        return 4;
                    }
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go two spaces below
            if (below && !right && !left && (absoluteDistance == 2))
            {
                //if the unit 2 below is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 2, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is unit (but not your unit)
                else if (currentBoard[currentUnit.currentY + 2, currentUnit.currentX].team != 0)
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 2, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY + 2, currentUnit.currentX] = new NullUnit();
                    return 1;
                }
                //if there is no unit 2 below
                else
                {
                    if (Water[currentUnit.currentY + 1, currentUnit.currentX]
                        || Water[currentUnit.currentY + 2, currentUnit.currentX])
                    {
                        return 4;
                    }
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }

            //if player is trying to go above and left
            if (above && left)
            {
                //if the unit above and left is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1].team)
                {
                    return 3;
                }
                //if there is any unit directy above AND left then the unit can not go
                else if ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                {
                    return 3;
                }
                //if there is water directly above AND left then the unit can not go
                else if ((Water[currentUnit.currentY - 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX - 1]))
                {
                    return 4;
                }
                //if there is no unit above and left
                else if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1].team == 0)
                {
                    if (Water[currentUnit.currentY - 1, currentUnit.currentX - 1])
                    {
                        return 4;
                    }
                    //if no water
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //if there is a unit (not urs)
                else 
                {
                    return 5;
                }
            }
            //if player is trying to go above and right
            if (above && right)
            {
                //if the unit above and right is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1].team)
                {
                    return 3;
                }
                //if there is any unit directy above AND right then the unit can not go
                else if ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                {
                    return 3;
                }
                //if there is water directly above AND right then the unit can not go
                else if ((Water[currentUnit.currentY - 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX + 1]))
                {
                    return 4;
                }
                //if there is no unit above and left
                else if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1].team == 0)
                {
                    if (Water[currentUnit.currentY - 1, currentUnit.currentX + 1])
                    {
                        return 4;
                    }
                    //if no water
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //if there is a unit (not urs)
                else
                {
                    return 5;
                }
            }
            //if player is trying to go below and left
            if (below && left)
            {
                //if the unit below and left is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team)
                {
                    return 3;
                }
                //if there is any unit directy below AND left then the unit can not go
                else if ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                {
                    return 3;
                }
                //if there is water directly below AND left then the unit can not go
                else if ((Water[currentUnit.currentY + 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX - 1]))
                {
                    return 4;
                }
                //if there is no unit below and left
                else if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team == 0)
                {
                    if (Water[currentUnit.currentY + 1, currentUnit.currentX - 1])
                    {
                        return 4;
                    }
                    //if no water
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //if there is a unit (not urs)
                else
                {
                    return 5;
                }
            }
            //if player is trying to go below and right
            if (below && right)
            {
                //if the unit below and right is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team)
                {
                    return 3;
                }
                //if there is any unit directy below AND right then the unit can not go
                else if ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                {
                    return 3;
                }
                //if there is water directly below AND right then the unit can not go
                else if ((Water[currentUnit.currentY + 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX + 1]))
                {
                    return 4;
                }
                //if there is no unit below and right
                else if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team == 0)
                {
                    if (Water[currentUnit.currentY + 1, currentUnit.currentX + 1])
                    {
                        return 4;
                    }
                    //if no water
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //if there is a unit (not urs)
                else
                {
                    return 5;
                }
            }

            return 9;
        }

        //Movement specifics for Tanks
        //ASUMPTION: the game will never feed it a position off the board
        //           for example it will never make a call goal(-1, 4)
        //Returns:  0 if Trying to move greater than maximum movement range of unit
        //          1 if legal move
        //          2 if Unit has already made a move this turn;
        //          3 if Other Unit(s) is blocking all paths to the space
        //          4 if trying to enter or pass through water
        //          9 End of move method reached. Unknown Error.
        public int moveTank(int goalX, int goalY)
        {
            //Check if unit has already moved this turn
            if (currentUnit.hasMoved)
                return 2;

            //Check if within absolute value range
            int dX = Math.Abs(goalX - currentUnit.currentX);
            int dY = Math.Abs(goalY - currentUnit.currentY);
            int absoluteDistance = dX +dY;
            if (absoluteDistance > currentUnit.movement)
            {
                return 0;
            }
            //Location comparison Booleans
            // True if goal is ____ (of) unit.
            Boolean right;
            Boolean left;
            Boolean above;
            Boolean below;

            if (goalX > currentUnit.currentX)
            {
                right = true;
                left = false;
            }
            else if (goalX < currentUnit.currentX)
            {
                right = false;
                left = true;
            }
            else
            {
                right = false;
                left = false;
            }
            if (goalY > currentUnit.currentY)
            {
                below = true;
                above = false;
            }
            else if (goalY < currentUnit.currentY)
            {
                below = false;
                above = true;
            }
            else
            {
                below = false;
                above = false;
            }

            //If unit is not trying to go anywhere (ie. unit is being told to be removed
            //from the next unit line) it is valid move
            if ((!right && !left && !below && !above))
            {
                return 1;
            }
            //If player is trying to go one space left
            if (left && !above && !below && (absoluteDistance == 1))
            {
                //if the unit to the left is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team)
                {
                    return 3;
                }
                //if there is water to left
                else if (Water[currentUnit.currentY, currentUnit.currentX - 1])
                {
                    return 4;
                }
                //if there is no unit to the left  
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX - 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //If player is trying to go one space right
            if (right && !above && !below && (absoluteDistance == 1))
            {
                //if the unit to the right is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team)
                {
                    return 3;
                }
                //if there is water to right
                else if (Water[currentUnit.currentY, currentUnit.currentX + 1])
                {
                    return 4;
                }
                //if there is no unit to the right  
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX + 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go one space above
            if (above && !right && !left && (absoluteDistance == 1))
            {
                //if the unit above is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is water above
                else if (Water[currentUnit.currentY - 1, currentUnit.currentX])
                {
                    return 4;
                }
                //if there is no unit above
                else if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 1, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go one space below
            if (below && !right && !left && (absoluteDistance == 1))
            {
                //if the unit below is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is water above
                else if (Water[currentUnit.currentY + 1, currentUnit.currentX])
                {
                    return 4;
                }
                //if there is no unit below
                else if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 1, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }

            //if player is trying to go two spaces left
            if (left && !above && !below && (absoluteDistance == 2))
            {
                //if there is any unit 1 space left you can not go
                if (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0)
                {
                    return 3;
                }
                //if there is water
                else if ((Water[currentUnit.currentY, currentUnit.currentX - 1])
                    || (Water[currentUnit.currentY, currentUnit.currentX - 2]))
                {
                    return 4;
                }
                //if the unit 2 to the left is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX - 2].team)
                {
                    return 3;
                }
                //if there is no unit 2 to the left  
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX - 2].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX - 2];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go two spaces right
            if (right && !above && !below && (absoluteDistance == 2))
            {
                //if there is any unit 1 space right you can not go
                if (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0)
                {
                    return 3;
                }
                //if there is water
                else if ((Water[currentUnit.currentY, currentUnit.currentX + 1])
                    || (Water[currentUnit.currentY, currentUnit.currentX + 2]))
                {
                    return 4;
                }
                //if the unit 2 to the right is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX + 2].team)
                {
                    return 3;
                }
                //if there is no unit 2 to the right 
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX + 2].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX + 2];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go two spaces above
            if (above && !right && !left && (absoluteDistance == 2))
            {
                //if there is any unit 1 space above you can not go
                if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                {
                    return 3;
                }
                //if there is water
                else if ((Water[currentUnit.currentY, currentUnit.currentY - 1])
                    || (Water[currentUnit.currentY, currentUnit.currentY - 2]))
                {
                    return 4;
                }
                //if the unit 2 above is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY - 2, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is no unit 2 above  
                else if (currentBoard[currentUnit.currentY - 2, currentUnit.currentX].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 2, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go two spaces below
            if (below && !right && !left && (absoluteDistance == 2))
            {
                //if there is any unit 1 space below you can not go
                if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                {
                    return 3;
                }
                //if there is water
                else if ((Water[currentUnit.currentY, currentUnit.currentY + 1])
                    || (Water[currentUnit.currentY, currentUnit.currentY + 2]))
                {
                    return 4;
                }
                //if the unit 2 below is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY + 2, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is no unit 2 below  
                else if (currentBoard[currentUnit.currentY + 2, currentUnit.currentX].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 2, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            
            //if player is trying to go three spaces left
            if (left && !above && !below && (absoluteDistance == 3))
            {
                //if there is any unit 1 or 2 space left you can not go
                if ((currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0)
                    || (currentBoard[currentUnit.currentY, currentUnit.currentX - 2].team != 0))
                {
                    return 3;
                }
                //if there is water
                else if ((Water[currentUnit.currentY, currentUnit.currentX - 1])
                    || (Water[currentUnit.currentY, currentUnit.currentX - 2])
                    || (Water[currentUnit.currentY, currentUnit.currentX - 3]))
                {
                    return 4;
                }
                //if the unit 3 to the left is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX - 3].team)
                {
                    return 3;
                }
                //if there is no unit 3 to the left  
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX - 3].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 3;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX - 3];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX - 3;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go three spaces right
            if (right && !above && !below && (absoluteDistance == 3))
            {
                //if there is any unit 1 or 2 space right you can not go
                if ((currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0)
                    || (currentBoard[currentUnit.currentY, currentUnit.currentX + 2].team != 0))
                {
                    return 3;
                }
                //if there is water
                else if ((Water[currentUnit.currentY, currentUnit.currentX + 1])
                    || (Water[currentUnit.currentY, currentUnit.currentX + 2])
                    || (Water[currentUnit.currentY, currentUnit.currentX + 3]))
                {
                    return 4;
                }
                //if the unit 3 to the right is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY, currentUnit.currentX + 3].team)
                {
                    return 3;
                }
                //if there is no unit 3 to the right  
                else if (currentBoard[currentUnit.currentY, currentUnit.currentX  + 3].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 3;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY, currentUnit.currentX + 3];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentX = currentUnit.currentX + 3;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go three spaces above
            if (above && !left && !right && (absoluteDistance == 3))
            {
                //if there is any unit 1 or 2 space above you can not go
                if ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    || (currentBoard[currentUnit.currentY - 2, currentUnit.currentX].team != 0))
                {
                    return 3;
                }
                //if there is water
                if ((Water[currentUnit.currentY - 1, currentUnit.currentX])
                    || (Water[currentUnit.currentY - 2, currentUnit.currentX])
                    || (Water[currentUnit.currentY - 3, currentUnit.currentX]))
                {
                    return 4;
                }
                //if the unit 3 above is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY - 3, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is no unit 3 above  
                else if (currentBoard[currentUnit.currentY - 3, currentUnit.currentX].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 3;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 3, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 3;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go three spaces below
            if (above && !left && !right && (absoluteDistance == 3))
            {
                //if there is any unit 1 or 2 space below you can not go
                if ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    || (currentBoard[currentUnit.currentY + 2, currentUnit.currentX].team != 0))
                {
                    return 3;
                }
                //if there is water
                if ((Water[currentUnit.currentY + 1, currentUnit.currentX])
                    || (Water[currentUnit.currentY + 2, currentUnit.currentX])
                    || (Water[currentUnit.currentY + 3, currentUnit.currentX]))
                {
                    return 4;
                }
                //if the unit 3 below is on the same team as current unit
                else if (currentUnit.team == currentBoard[currentUnit.currentY + 3, currentUnit.currentX].team)
                {
                    return 3;
                }
                //if there is no unit 3 below  
                else if (currentBoard[currentUnit.currentY + 3, currentUnit.currentX].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 3;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 3, currentUnit.currentX];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 3;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            
            //if player is trying to go above and left
            if (above && left && (absoluteDistance == 2))
            {
                //if the unit above and left is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1].team)
                {
                    return 3;
                }
                //if there is any unit directy above AND left then the unit can not go
                else if ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                {
                    return 3;
                }
                //if there is water
                else if (((Water[currentUnit.currentY - 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX - 1]))
                    || (Water[currentUnit.currentY - 1, currentUnit.currentX - 1]))
                {
                    return 4;
                }
                //if there is no unit above and left
                else if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go above and right
            if (above && right && (absoluteDistance == 2))
            {
                //if the unit above and right is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1].team)
                {
                    return 3;
                }
                //if there is any unit directy above AND right then the unit can not go
                else if ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                {
                    return 3;
                }
                //if there is water
                else if (((Water[currentUnit.currentY - 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX + 1]))
                    || (Water[currentUnit.currentY - 1, currentUnit.currentX + 1]))
                {
                    return 4;
                }
                //if there is no unit above and right
                else if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go below and left
            if (below && left && (absoluteDistance == 2))
            {
                //if the unit below and left is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team)
                {
                    return 3;
                }
                //if there is any unit directy below AND left then the unit can not go
                else if ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                {
                    return 3;
                }
                //if there is water
                else if (((Water[currentUnit.currentY + 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX - 1]))
                    || (Water[currentUnit.currentY + 1, currentUnit.currentX - 1]))
                {
                    return 4;
                }
                //if there is no unit below and left
                else if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if player is trying to go below and right
            if (below && right && (absoluteDistance == 2))
            {
                //if the unit below and right is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team)
                {
                    return 3;
                }
                //if there is any unit directy below AND right then the unit can not go
                else if ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                {
                    return 3;
                }
                //if there is water
                else if (((Water[currentUnit.currentY + 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX + 1]))
                    || (Water[currentUnit.currentY + 1, currentUnit.currentX + 1]))
                {
                    return 4;
                }
                //if there is no unit below and right
                else if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            ///////////////////////////////////////////////////////////////////////////////////
            //Things get ugly here
            //For each i must check if any 2 points create an unpassible wall either in units or water
            ////////////////////////////////////////////////////////////////////////////////////

            //if above 2 left 1
            if (above && left && (dY > dX)){
                //if the unit at goal is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 2, currentUnit.currentX - 1].team)
                {
                    return 3;
                }
                //if there is are units blocking all possible paths
                else if (((currentBoard[currentUnit.currentY - 2, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1].team != 0)
                    && (currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0))
                    )
                {
                    return 3;
                }
                //if there is water blocking the path OR at goal
                else if ((((Water[currentUnit.currentY - 2, currentUnit.currentX])
                    && (Water[currentUnit.currentY - 1, currentUnit.currentX - 1]))
                    || ((Water[currentUnit.currentY - 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX - 1]))
                    || ((Water[currentUnit.currentY - 1, currentUnit.currentX - 1])
                    && (Water[currentUnit.currentY - 1, currentUnit.currentX])))
                    || (Water[currentUnit.currentY -2, currentUnit.currentX - 1])
                    )
                {
                    return 4;
                }
                //if there is no unit at goal
                else if (currentBoard[currentUnit.currentY - 2, currentUnit.currentX - 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 2;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 2, currentUnit.currentX - 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 2;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if above 2 right 1
            if (above && right && (dY > dX)){
                //if the unit at goal is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 2, currentUnit.currentX + 1].team)
                {
                    return 3;
                }
                 //if there is are units blocking all possible paths
                else if (((currentBoard[currentUnit.currentY - 2, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1].team != 0)
                    && (currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0))
                    )
                {
                    return 3;
                }
                //if there is water blocking the path OR at goal
                else if ((((Water[currentUnit.currentY - 2, currentUnit.currentX])
                    && (Water[currentUnit.currentY - 1, currentUnit.currentX - 1]))
                    || ((Water[currentUnit.currentY - 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX - 1]))
                    || ((Water[currentUnit.currentY - 1, currentUnit.currentX - 1])
                    && (Water[currentUnit.currentY - 1, currentUnit.currentX])))
                    || (Water[currentUnit.currentY -2, currentUnit.currentX + 1])
                    )
                {
                    return 4;
                }
                //if there is no unit at goal
                else if (currentBoard[currentUnit.currentY - 2, currentUnit.currentX + 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 2;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 2, currentUnit.currentX + 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 2;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if below 2 left 1
            if (below && left && (dY > dX)){
                //if the unit at goal is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 2, currentUnit.currentX - 1].team)
                {
                    return 3;
                }
                //if there is are units blocking all possible paths
                else if (((currentBoard[currentUnit.currentY + 2, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team != 0)
                    && (currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0))
                    )
                {
                    return 3;
                }
                //if there is water blocking the path OR at goal
                else if ((((Water[currentUnit.currentY + 2, currentUnit.currentX])
                    && (Water[currentUnit.currentY + 1, currentUnit.currentX - 1]))
                    || ((Water[currentUnit.currentY + 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX - 1]))
                    || ((Water[currentUnit.currentY + 1, currentUnit.currentX - 1])
                    && (Water[currentUnit.currentY + 1, currentUnit.currentX])))
                    || (Water[currentUnit.currentY + 2, currentUnit.currentX - 1])
                    )
                {
                    return 4;
                }
                //if there is no unit at goal
                else if (currentBoard[currentUnit.currentY + 2, currentUnit.currentX - 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 2;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 2, currentUnit.currentX - 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 2;
                    currentUnit.currentX = currentUnit.currentX - 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if below 2 right 1
            if (below && right && (dY > dX)){
                //if the unit at goal is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 2, currentUnit.currentX + 1].team)
                {
                    return 3;
                }
                 //if there is are units blocking all possible paths
                else if (((currentBoard[currentUnit.currentY + 2, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team != 0)
                    && (currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0))
                    )
                {
                    return 3;
                }
                //if there is water blocking the path OR at goal
                else if ((((Water[currentUnit.currentY + 2, currentUnit.currentX])
                    && (Water[currentUnit.currentY + 1, currentUnit.currentX - 1]))
                    || ((Water[currentUnit.currentY + 1, currentUnit.currentX])
                    && (Water[currentUnit.currentY, currentUnit.currentX - 1]))
                    || ((Water[currentUnit.currentY + 1, currentUnit.currentX - 1])
                    && (Water[currentUnit.currentY + 1, currentUnit.currentX])))
                    || (Water[currentUnit.currentY + 2, currentUnit.currentX + 1])
                    )
                {
                    return 4;
                }
                //if there is no unit at goal
                else if (currentBoard[currentUnit.currentY + 2, currentUnit.currentX + 1].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 2;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 2, currentUnit.currentX + 1];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 2;
                    currentUnit.currentX = currentUnit.currentX + 1;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //////////////////////////////////////////////
            //if above 1 left 2
            if (above && left && (dX > dY)){
                //if the unit at goal is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 2].team)
                {
                    return 3;
                }
                //if there is are units blocking all possible paths
                else if (((currentBoard[currentUnit.currentY -1, currentUnit.currentX - 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 2].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                    )
                {
                    return 3;
                }
                //if there is water blocking the path OR at goal
                else if ((((currentBoard[currentUnit.currentY -1, currentUnit.currentX - 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 2].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0)))
                    || (Water[currentUnit.currentY - 1, currentUnit.currentX - 2])
                    )
                {
                    return 4;
                }
                //if there is no unit at goal
                else if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 2].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 1, currentUnit.currentX - 2];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if above 1 right 2
            if (above && right && (dX > dY)){
                //if the unit at goal is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 2].team)
                {
                    return 3;
                }
                //if there is are units blocking all possible paths
                else if (((currentBoard[currentUnit.currentY -1, currentUnit.currentX + 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 2].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                    )
                {
                    return 3;
                }
                //if there is water blocking the path OR at goal
                else if ((((currentBoard[currentUnit.currentY -1, currentUnit.currentX + 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 2].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                    || ((currentBoard[currentUnit.currentY - 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0)))
                    || (Water[currentUnit.currentY - 1, currentUnit.currentX + 2])
                    )
                {
                    return 4;
                }
                //if there is no unit at goal
                else if (currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 2].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY - 1, currentUnit.currentX + 2];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY - 1;
                    currentUnit.currentX = currentUnit.currentX + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if below 1 left 2
            if (below && left && (dX > dY)){
                //if the unit at goal is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 2].team)
                {
                    return 3;
                }
                //if there is are units blocking all possible paths
                else if (((currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 2].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                    )
                {
                    return 3;
                }
                //if there is water blocking the path OR at goal
                else if ((((currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 2].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX - 1].team != 0)))
                    || (Water[currentUnit.currentY + 1, currentUnit.currentX - 2])
                    )
                {
                    return 4;
                }
                //if there is no unit at goal
                else if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 2].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 1, currentUnit.currentX - 2];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX - 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            //if below 1 right 2
            if (below && right && (dX > dY)){
                //if the unit at goal is on the same team as current unit
                if (currentUnit.team == currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 2].team)
                {
                    return 3;
                }
                //if there is are units blocking all possible paths
                else if (((currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 2].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                    )
                {
                    return 3;
                }
                //if there is water blocking the path OR at goal
                else if ((((currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 2].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 1].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0))
                    || ((currentBoard[currentUnit.currentY + 1, currentUnit.currentX].team != 0)
                    && (currentBoard[currentUnit.currentY, currentUnit.currentX + 1].team != 0)))
                    || (Water[currentUnit.currentY + 1, currentUnit.currentX + 2])
                    )
                {
                    return 4;
                }
                //if there is no unit at goal
                else if (currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 2].team == 0)
                {
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
                //else there is a unit and it is an enemy
                else
                {
                    unitsKilledThisTurn[numUnitsKilled] = currentBoard[currentUnit.currentY + 1, currentUnit.currentX + 2];
                    numUnitsKilled++;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = new NullUnit();
                    currentUnit.currentY = currentUnit.currentY + 1;
                    currentUnit.currentX = currentUnit.currentX + 2;
                    currentBoard[currentUnit.currentY, currentUnit.currentX] = currentUnit;
                    return 1;
                }
            }
            return 9;
        }

        //Call for all unite Movement
        //Does alot of upkeep and calls the specific move calls
        //NOTE:      ONLY CALL THIS FOR MOVEMENT
        //Returns:  0 if Trying to move greater than maximum movement range of unit
        //          1 if legal move
        //          2 if Unit has already made a move this turn;
        //          3 if Other Unit(s) is blocking all paths to the space
        //          4 if trying to enter or pass through water
        //          5 if Special Artillary Error
        //               Attempt to shoot or move to enemy diagonally
        //          8 if goalX or goalY was outside array
        //          9 End of move method reached. Unknown Error.
        public int moveUnit(int goalX, int goalY){
            int results;

            if ((goalX < 0) || (goalY < 0) || (goalX >= xSize) || (goalY >= ySize))
            {
                return 8;
            }

            if (currentUnit.type.Equals("tank")){
                results = moveTank(goalX, goalY);
            }
            else if (currentUnit.type.Equals("humvee")){
                results = moveHumVee(goalX, goalY);
            }
            else{
                results = moveArtillery(goalX, goalY);
            }

            if (results == 1)
            {
                // Step 1: Add to moves made list
                actionsTakenThisTurn[totalactions] = currentUnit;
                totalactions++;
                // Step 2: Set unit to has moved
                currentUnit.sethasMoved(true);
                // Step 3: Call next unit
                nextUnit();
            }

            return results;
        }

        //Undoes the last move done by player
        //UNFINISHED
        public void undo()
        {

        }

        public void update(GameTime gametime)
        {
            int x = 0;
            int y = 0;
            while (y < ySize)
            {
                x = 0;
                while (x < xSize)
                {
                    if (currentBoard[y, x].team != 0)
                    {
                        currentBoard[y, x].sprite.Update(gametime);
                    }
                    x++;
                }
                y++;
            }
        }

        public void draw(SpriteBatch batch)
        {
            int x = 0;
            int y = 0;
            while (y < ySize)
            {
                x = 0;
                while (x < xSize)
                {
                    if (currentBoard[y, x].team != 0)
                    {
                        currentBoard[y, x].sprite.Position.X = currentBoard[y, x].currentX * spriteAdjusterX;
                        currentBoard[y, x].sprite.Position.Y = currentBoard[y, x].currentY * spriteAdjusterY;
                        if (currentBoard[y, x].currentX <= xSize / 2)
                            currentBoard[y, x].sprite.CurrentAnimation = "Right";
                        else
                            currentBoard[y, x].sprite.CurrentAnimation = "Left";
                        currentBoard[y, x].sprite.Draw(batch);
                    }
                    x++;
                }
                y++;
            }
        }

        //Removes the given unit from the players unit list
        public void removeUnit(int unitNum, int teamNum)
        {
            int i = 0;
            if (teamNum == 1)
            {
                while (i < team1Length)
                {
                    if (team1[i].unitNumber == unitNum)
                    {
                        team1[i] = team1[team1Length];
                        team1[team1Length] = new NullUnit();
                        team1Length--;
                    }
                    i++;
                }
            }
            else if (teamNum == 2){
                while (i < team2Length){    
                    if (team2[i].unitNumber == unitNum)
                    {
                        team2[i] = team2[team2Length];
                        team2[team1Length] = new NullUnit();
                        team2Length--;
                    }
                    i++;
                }
            }
            else if (teamNum == 3){
                while (i < team2Length)
                {
                    if (team3[i].unitNumber == unitNum)
                    {
                        team3[i] = team3[team3Length];
                        team3[team3Length] = new NullUnit();
                        team3Length--;
                    }
                    i++;
                }
            }
            else{        
                if (team4[i].unitNumber == unitNum)
                    {
                        team4[i] = team4[team4Length];
                        team4[team4Length] = new NullUnit();
                        team4Length--;
                    }
                    i++;
                }
            }
        }
    }