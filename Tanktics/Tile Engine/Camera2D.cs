//Coded by robby?
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Tanktics
{
    //Robby Florence
    public class Camera2D
    {
        #region Fields

        //information about tile engine
        readonly int tileWidth;
        readonly int tileHeight;
        readonly int mapWidth;
        readonly int mapHeight;

        //camera position (relative to tile engine)
        public Vector2 Position = Vector2.Zero;
        //camera viewport (relative to window)
        public Rectangle Viewport = new Rectangle();
        //movement speed
        public float Speed = 100f;

        //zooming variables
        float scale = 1f;
        float scaleRate = 0.25f;
        public float MinScale = 0.1f;
        public float MaxScale = 2f;

        //player using this camera
        public int PlayerNum;

        #endregion

        #region Properties

        public float Scale
        {
            get { return scale; }
            set
            {
                if (value > 0)
                    scale = value;
            }
        }

        #endregion

        #region Initialization

        public Camera2D(int player, int tileW, int tileH, int mapW, int mapH)
        {
            PlayerNum = player;
            tileWidth = tileW;
            tileHeight = tileH;
            mapWidth = mapW;
            mapHeight = mapH;
        }

        #endregion

        #region Private Methods

        //prevent camera from moving off edge of the tile map
        private void ClampToViewport()
        {
            Vector2 maxPosition = new Vector2(
                (int)(scale * tileWidth + 0.5f) * mapWidth - Viewport.Width,
                (int)(scale * tileHeight + 0.5f) * mapHeight - Viewport.Height);

            if (Position.X < 0)
                Position.X = 0;
            else if (Position.X > maxPosition.X)
                Position.X = maxPosition.X;
            if (Position.Y < 0)
                Position.Y = 0;
            else if (Position.Y > maxPosition.Y)
                Position.Y = maxPosition.Y;
        }

        #endregion

        #region Public Methods

        //move the camera's x and/or y position
        public void Move(Vector2 direction, float elapsed)
        {
            Position.X += direction.X * elapsed * Speed;
            Position.Y += direction.Y * elapsed * Speed;

            ClampToViewport();
        }

        //increase scale
        public void ZoomIn(float elapsed)
        {
            scale += elapsed * scaleRate;

            if (scale > MaxScale)
                scale = MaxScale;
        }
        
        //decrease scale
        public void ZoomOut(float elapsed)
        {
            scale -= elapsed * scaleRate;

            if (scale < MinScale)
                scale = MinScale;

            ClampToViewport();
        }

        //center the camera around tile at (x, y) if it is outside viewport
        public void JumpTo(int x, int y)
        {
            int scaledTileWidth = (int)(scale * tileWidth + 0.5f);
            int scaledTileHeight = (int)(scale * tileHeight + 0.5f);

            //calculate range of visible tiles
            Point minVisible = new Point(
                (int)(Position.X / scaledTileWidth) + 1,
                (int)(Position.Y / scaledTileHeight) + 1);
            Point maxVisible = new Point(
                (int)(Position.X + Viewport.Width) / scaledTileWidth - 1,
                (int)(Position.Y + Viewport.Height) / scaledTileHeight - 1);

            //center around (x, y) if its outside viewport
            if (x < minVisible.X || x > maxVisible.X)
                Position.X = x * scaledTileWidth - Viewport.Width / 2;
            if (y < minVisible.Y || y > maxVisible.Y)
                Position.Y = y * scaledTileHeight - Viewport.Height / 2;

            ClampToViewport();
        }

        #endregion
    }
}
