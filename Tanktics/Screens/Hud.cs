#region File Description
//-----------------------------------------------------------------------------
// HUD.cs
//
// The HUD displays player information in the GameplayScreen - Robby Florence
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Tanktics
{
    //Robby Florence
    public class HUD
    {
        #region Fields

        public enum HUDUnits { APC, Tank, Artillery };

        Texture2D background;
        Rectangle backgroundRect;
        Texture2D[] borders = new Texture2D[4];
        Rectangle borderRect;
        //rectangles of left (rotating unit) and center (player info) hud sections
        Rectangle leftRect, centerRect;

        SpriteFont font;

        Texture2D unitIcons;
        //frames in unit icons spritesheet
        //first dimension is (white, green, grey, brown)
        //second dimension is (apc, tank, artillery)
        Rectangle[,] unitIconFrames = new Rectangle[4, 3];

        //rotating unit variables
        Texture2D[,] rotationTextures = new Texture2D[4, 3];
        AnimatingSprite[] rotationSprites = new AnimatingSprite[3];
        int currentUnitType;

        string hudInfo;
        int currentTeam = 0;
        //number of living units for current team
        int numArtillery;
        int numTanks;
        int numAPCs;
        //max number of units
        int maxArtillery;
        int maxTanks;
        int maxAPCs;
        //texture used to draw graphs
        Texture2D blank;
        //graph height for each team
        //determined by percentage of units still on the board
        float graph1Height;
        float graph2Height;
        float graph3Height;
        float graph4Height;

        #endregion

        #region Initialization

        public HUD(int x, int y, int width, int height)
        {
            //rectangle for background texture
            backgroundRect = new Rectangle(x, y, (int)(630f / 800f * width), height);
            //rectangle for border texture
            borderRect = new Rectangle(x, y, width, height);
            //rectangle for rotating unit
            leftRect = new Rectangle(
                x,
                y + (int)(20f / 170f * height),
                (int)(150f / 800f * width),
                (int)(150f / 170f * height));
            //rectangle for player information
            centerRect = new Rectangle(
                x + (int)(170f / 800f * width),
                leftRect.Y,
                (int)(460f / 800f * width),
                leftRect.Height);

            //rectangles for frames in unit icons spritesheet
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    unitIconFrames[row, col] = new Rectangle(col * 32, row * 32, 32, 32);
                }
            }
        }
        
        /// <summary>
        /// A place to load all content
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            if (content == null)
                return;

            background = content.Load<Texture2D>("HUD/hud background");
            borders[0] = content.Load<Texture2D>("HUD/hud border white");
            borders[1] = content.Load<Texture2D>("HUD/hud border green");
            borders[2] = content.Load<Texture2D>("HUD/hud border grey");
            borders[3] = content.Load<Texture2D>("HUD/hud border brown");
            unitIcons = content.Load<Texture2D>("HUD/unit icons");
            font = content.Load<SpriteFont>("gamefont");
            blank = content.Load<Texture2D>("blank");

            //load rotating unit textures
            rotationTextures[0, (int)HUDUnits.APC] = content.Load<Texture2D>("Unit Animations/APC/APC Rotating/APC Rotating White");
            rotationTextures[1, (int)HUDUnits.APC] = content.Load<Texture2D>("Unit Animations/APC/APC Rotating/APC Rotating Green");
            rotationTextures[2, (int)HUDUnits.APC] = content.Load<Texture2D>("Unit Animations/APC/APC Rotating/APC Rotating Grey");
            rotationTextures[3, (int)HUDUnits.APC] = content.Load<Texture2D>("Unit Animations/APC/APC Rotating/APC Rotating Brown");
            rotationTextures[0, (int)HUDUnits.Tank] = content.Load<Texture2D>("Unit Animations/Tank/Tank Rotating/Tank Rotating White");
            rotationTextures[1, (int)HUDUnits.Tank] = content.Load<Texture2D>("Unit Animations/Tank/Tank Rotating/Tank Rotating Green");
            rotationTextures[2, (int)HUDUnits.Tank] = content.Load<Texture2D>("Unit Animations/Tank/Tank Rotating/Tank Rotating Grey");
            rotationTextures[3, (int)HUDUnits.Tank] = content.Load<Texture2D>("Unit Animations/Tank/Tank Rotating/Tank Rotating Brown");
            rotationTextures[0, (int)HUDUnits.Artillery] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Rotating/Artillery Rotating White");
            rotationTextures[1, (int)HUDUnits.Artillery] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Rotating/Artillery Rotating Green");
            rotationTextures[2, (int)HUDUnits.Artillery] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Rotating/Artillery Rotating Grey");
            rotationTextures[3, (int)HUDUnits.Artillery] = content.Load<Texture2D>("Unit Animations/Artillery/Artillery Rotating/Artillery Rotating Brown");

            //create apc animation
            rotationSprites[(int)HUDUnits.APC] = new AnimatingSprite();
            rotationSprites[(int)HUDUnits.APC].Texture = rotationTextures[0, (int)HUDUnits.APC];
            rotationSprites[(int)HUDUnits.APC].Animations.Add("rotate", new Animation(1410, 380, 50, 5, 10, 0, 0));
            rotationSprites[(int)HUDUnits.APC].CurrentAnimation = "rotate";
            //create tank animation
            rotationSprites[(int)HUDUnits.Tank] = new AnimatingSprite();
            rotationSprites[(int)HUDUnits.Tank].Texture = rotationTextures[0, (int)HUDUnits.Tank];
            rotationSprites[(int)HUDUnits.Tank].Animations.Add("rotate", new Animation(2000, 260, 50, 5, 10, 0, 0));
            rotationSprites[(int)HUDUnits.Tank].CurrentAnimation = "rotate";
            //create artillery animation
            rotationSprites[(int)HUDUnits.Artillery] = new AnimatingSprite();
            rotationSprites[(int)HUDUnits.Artillery].Texture = rotationTextures[0, (int)HUDUnits.Artillery];
            rotationSprites[(int)HUDUnits.Artillery].Animations.Add("rotate", new Animation(1540, 210, 50, 5, 10, 0, 0));
            rotationSprites[(int)HUDUnits.Artillery].CurrentAnimation = "rotate";
        }

        #endregion

        #region Update and Draw

        public void Update(GameTime gameTime, TurnController turn, UnitController units)
        {
            rotationSprites[currentUnitType].Update(gameTime);

            currentTeam = units.currentPlayer;

            //for setup phase, get rotating animation of unit being added
            if (turn.phase == 0)
            {
                if (turn.totalAPC < turn.MAXAPC)
                    currentUnitType = (int)HUDUnits.APC;
                else if (turn.totalTank < turn.MAXTANK)
                    currentUnitType = (int)HUDUnits.Tank;
                else
                    currentUnitType = (int)HUDUnits.Artillery;
            }
            //get rotating animation of current unit
            else if (units.currentUnit != null)
            {
                if (units.currentUnit.type.Equals("apc"))
                    currentUnitType = (int)HUDUnits.APC;
                else if (units.currentUnit.type.Equals("tank"))
                    currentUnitType = (int)HUDUnits.Tank;
                else
                    currentUnitType = (int)HUDUnits.Artillery;
            }

            rotationSprites[currentUnitType].Texture = rotationTextures[currentTeam - 1, currentUnitType];

            //update phase and points
            if (turn.phase == 0)
                hudInfo = "Phase: Setup\n";
            else if (turn.phase == 2)
                hudInfo = "Phase: Movement\n";
            else if (turn.phase == 3)
                hudInfo = "Phase: Combat\n";
            else if (turn.phase == 4)
                hudInfo = "Phase: Purchase\n";
            else
                hudInfo = "Phase:\n";

            hudInfo += "Points: " + turn.points + "\nAPCs:\nTanks:\nArtillery:";

            //update number of each unit
            numAPCs = units.getNumUnits(currentTeam, "apc");
            numTanks = units.getNumUnits(currentTeam, "tank");
            numArtillery = units.getNumUnits(currentTeam, "artillery");
            maxAPCs = turn.MAXAPC;
            maxTanks = turn.MAXTANK;
            maxArtillery = turn.MAXARTIL;

            int maxTotalUnits = maxAPCs + maxTanks + maxArtillery;

            //update graphs
            graph1Height = (float)(units.getNumUnits(1, "apc") + units.getNumUnits(1, "tank") +
                units.getNumUnits(1, "artillery")) / maxTotalUnits;
            graph2Height = (float)(units.getNumUnits(2, "apc") + units.getNumUnits(2, "tank") +
                units.getNumUnits(2, "artillery")) / maxTotalUnits;
            graph3Height = (float)(units.getNumUnits(3, "apc") + units.getNumUnits(3, "tank") +
                units.getNumUnits(3, "artillery")) / maxTotalUnits;
            graph4Height = (float)(units.getNumUnits(4, "apc") + units.getNumUnits(4, "tank") +
                units.getNumUnits(4, "artillery")) / maxTotalUnits;

        }

        public void Draw(SpriteBatch batch)
        {
            //draw hud background
            batch.Draw(background, backgroundRect, Color.White);

            //exit if current team hasnt been set yet
            if (currentTeam < 1)
                return;

            //draw team-colored border
            batch.Draw(borders[currentTeam - 1], borderRect, Color.White);
            
            //draw rotating unit in left rectangle
            Rectangle currentSpriteRect = rotationSprites[currentUnitType].Animations["rotate"].CurrentFrame;

            //calculate scale to make the sprite fit in destination
            float scale = Math.Min(
                0.8f * (float)leftRect.Width / currentSpriteRect.Width,
                0.8f * (float)leftRect.Height / currentSpriteRect.Height);

            //use scale to get new destination rectangle
            //centered in original destination
            Rectangle spriteDest = new Rectangle(
                0, 0,
                (int)(scale * currentSpriteRect.Width),
                (int)(scale * currentSpriteRect.Height));
            spriteDest.X = leftRect.X + leftRect.Width / 2 - spriteDest.Width / 2;
            spriteDest.Y = leftRect.Y + leftRect.Height / 2 - spriteDest.Height / 2;

            rotationSprites[currentUnitType].Draw(batch, spriteDest, Color.White);

            //draw hud info text
            Vector2 textSize = font.MeasureString(hudInfo);
            Vector2 textPosition = new Vector2(
                    centerRect.X + (int)(0.05f * centerRect.Width),
                    centerRect.Y + (int)(0.05f * centerRect.Height));
            float textScale = 0.9f * centerRect.Height / textSize.Y;

            batch.DrawString(
                font,
                hudInfo,
                textPosition,
                Color.Black,
                0f,
                Vector2.Zero,
                textScale,
                SpriteEffects.None,
                0f);

            //draw unit icons
            Color fade;
            Color dark = new Color(96, 96, 96);

            //draw apc icons
            Rectangle iconPosition = new Rectangle(
                (int)(centerRect.X + 0.18f * centerRect.Width),
                (int)(textPosition.Y + 0.4f * textScale * textSize.Y),
                (int)(0.2f * textScale * textSize.Y),
                (int)(0.2f * textScale * textSize.Y));

            for (int i = 1; i <= maxAPCs; i++)
            {
                if (i <= numAPCs)
                    fade = Color.White;
                else
                    fade = dark;

                batch.Draw(
                    unitIcons,
                    iconPosition,
                    unitIconFrames[currentTeam - 1, (int)HUDUnits.APC],
                    fade);
                iconPosition.X += iconPosition.Width;
            }

            //draw tank icons
            iconPosition.X = (int)(centerRect.X + 0.21f * centerRect.Width);
            iconPosition.Y += (int)(0.2f * textScale * textSize.Y);

            for (int i = 1; i <= maxTanks; i++)
            {
                if (i <= numTanks)
                    fade = Color.White;
                else
                    fade = dark;

                batch.Draw(
                    unitIcons,
                    iconPosition,
                    unitIconFrames[currentTeam - 1, (int)HUDUnits.Tank],
                    fade);
                iconPosition.X += iconPosition.Width;
            }

            //draw artillery icons
            iconPosition.X = (int)(centerRect.X + 0.33f * centerRect.Width);
            iconPosition.Y += (int)(0.2f * textScale * textSize.Y);

            for (int i = 1; i <= maxArtillery; i++)
            {
                if (i <= numArtillery)
                    fade = Color.White;
                else
                    fade = dark;

                batch.Draw(
                    unitIcons,
                    iconPosition,
                    unitIconFrames[currentTeam - 1, (int)HUDUnits.Artillery],
                    fade);
                iconPosition.X += iconPosition.Width;
            }

            //draw graph title
            textSize = font.MeasureString("Army");
            textPosition.X = centerRect.X + (int)(0.77 * centerRect.Width);
            batch.DrawString(
                font,
                "Army",
                textPosition,
                Color.Black,
                0f,
                Vector2.Zero,
                textScale,
                SpriteEffects.None,
                0f);

            //draw graph 1
            float totalGraphHeight = 0.9f * centerRect.Height - textScale * textSize.Y;
            Rectangle graphPosition = new Rectangle(
                (int)(centerRect.X + 0.75f * centerRect.Width),
                (int)(textPosition.Y + textScale * textSize.Y + (1f - graph1Height) * totalGraphHeight),
                (int)(0.05f * centerRect.Width),
                (int)(graph1Height * totalGraphHeight));
            batch.Draw(blank, graphPosition, new Color(240, 240, 240));

            //draw graph 2
            graphPosition.X += (int)(0.05f * centerRect.Width);
            graphPosition.Y = (int)(textPosition.Y + textScale * textSize.Y + (1f - graph2Height) * totalGraphHeight);
            graphPosition.Height = (int)(graph2Height * totalGraphHeight);
            batch.Draw(blank, graphPosition, new Color(120, 170, 110));

            //draw graph 3
            graphPosition.X += (int)(0.05f * centerRect.Width);
            graphPosition.Y = (int)(textPosition.Y + textScale * textSize.Y + (1f - graph3Height) * totalGraphHeight);
            graphPosition.Height = (int)(graph3Height * totalGraphHeight);
            batch.Draw(blank, graphPosition, new Color(180, 180, 180));

            //draw graph 4
            graphPosition.X += (int)(0.05f * centerRect.Width);
            graphPosition.Y = (int)(textPosition.Y + textScale * textSize.Y + (1f - graph4Height) * totalGraphHeight);
            graphPosition.Height = (int)(graph4Height * totalGraphHeight);
            batch.Draw(blank, graphPosition, new Color(170, 140, 80));
        }

        #endregion
    }
}
