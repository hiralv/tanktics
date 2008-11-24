#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Tanktics
{
    public class Camera2D
    {
        #region Fields

        readonly int tileWidth;
        readonly int tileHeight;
        readonly int mapWidth;
        readonly int mapHeight;

        public Vector2 Position = Vector2.Zero;
        public Rectangle Viewport = new Rectangle();
        public float Speed = 100f;

        float scale = 1f;
        float scaleRate = 0.25f;
        public float MinScale = 0.1f;
        public float MaxScale = 2f;

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

        public Camera2D(int tileW, int tileH, int mapW, int mapH)
        {
            tileWidth = tileW;
            tileHeight = tileH;
            mapWidth = mapW;
            mapHeight = mapH;
        }

        #endregion

        #region Private Methods

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

        public void Move(Vector2 direction, float elapsed)
        {
            Position.X += direction.X * elapsed * Speed;
            Position.Y += direction.Y * elapsed * Speed;

            ClampToViewport();
        }

        public void ZoomIn(float elapsed)
        {
            scale += elapsed * scaleRate;

            if (scale > MaxScale)
                scale = MaxScale;
        }

        public void ZoomOut(float elapsed)
        {
            scale -= elapsed * scaleRate;

            if (scale < MinScale)
                scale = MinScale;

            ClampToViewport();
        }

        #endregion
    }
}
