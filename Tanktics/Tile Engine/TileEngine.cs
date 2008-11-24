#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Tanktics
{
    public class TileEngine
    {
        #region Fields

        Texture2D texture;
        Rectangle[] tiles;
        public readonly int TileWidth;
        public readonly int TileHeight;

        int[,,] map;
        public readonly int MapWidth;
        public readonly int MapHeight;
        int numLayers;

        public Texture2D SelectedTexture;

        #endregion

        #region Properties

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public int WidthInPixels
        {
            get { return MapWidth * TileWidth; }
        }

        public int HeightInPixels
        {
            get { return MapHeight * TileHeight; }
        }

        #endregion

        #region Initialization

        public TileEngine(string filename, int tileW, int tileH, int tileRows, int tileCols)
        {
            try
            {
                StreamReader sr = File.OpenText(filename);
                string line;
                char[] delims = { ' ', '\t', ',' };
                string[] values;

                line = sr.ReadLine() + " " + sr.ReadLine() + " " + sr.ReadLine();
                values = line.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                MapWidth = Convert.ToInt32(values[1]);
                MapHeight = Convert.ToInt32(values[3]);
                numLayers = Convert.ToInt32(values[5]);
                map = new int[numLayers, MapHeight, MapWidth];

                for (int i = 0; i < numLayers; i++)
                {
                    for (int y = 0; y < MapHeight; y++)
                    {
                        do
                        {
                            line = sr.ReadLine();
                            values = line.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                        }
                        while (values.Length == 0);

                        for (int x = 0; x < MapWidth; x++)
                        {
                            map[i, y, x] = Convert.ToInt32(values[x]);
                        }
                    }
                }

                sr.Close();
            }
            catch (Exception)
            {
                MapWidth = MapHeight = numLayers = 0;
                map = new int[numLayers, MapHeight, MapWidth];
            }

            TileWidth = tileW;
            TileHeight = tileH;
            tiles = new Rectangle[tileRows * tileCols];

            for (int i = 0, y = 0; y < tileRows; y++)
            {
                for (int x = 0; x < tileCols; x++, i++)
                {
                    tiles[i] = new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight);
                }
            }
        }

        #endregion

        #region Public Methods

        public bool IsWalkable(int x, int y)
        {
            if (x < 0 || x >= MapWidth || y < 0 || y >= MapHeight)
                return false;

            return map[numLayers - 1, y, x] == 0;
        }

        public void Draw(SpriteBatch batch, Camera2D camera)
        {
            int scaledTileWidth = (int)(camera.Scale * TileWidth + 0.5f);
            int scaledTileHeight = (int)(camera.Scale * TileHeight + 0.5f);

            Point minVisible = new Point(
                (int)(camera.Position.X / scaledTileWidth),
                (int)(camera.Position.Y / scaledTileHeight));
            Point maxVisible = new Point(
                ((int)(camera.Position.X + camera.Viewport.Width) / scaledTileWidth) + 1,
                ((int)(camera.Position.Y + camera.Viewport.Height) / scaledTileHeight) + 1);

            minVisible.X = Math.Max(minVisible.X, 0);
            minVisible.Y = Math.Max(minVisible.Y, 0);
            maxVisible.X = Math.Min(maxVisible.X, MapWidth);
            maxVisible.Y = Math.Min(maxVisible.Y, MapHeight);

            for (int i = 0; i < numLayers - 1; i++)
            {
                for (int y = minVisible.Y; y < maxVisible.Y; y++)
                {
                    for (int x = minVisible.X; x < maxVisible.X; x++)
                    {
                        if (map[i, y, x] >= 0)
                        {
                            batch.Draw(
                                texture,
                                new Rectangle(
                                    (x - minVisible.X) * scaledTileWidth + camera.Viewport.X - (int)camera.Position.X % scaledTileWidth,
                                    (y - minVisible.Y) * scaledTileHeight + camera.Viewport.Y - (int)camera.Position.Y % scaledTileHeight,
                                    scaledTileWidth,
                                    scaledTileHeight),
                                tiles[map[i, y, x]],
                                Color.White);
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch batch, Camera2D camera, UnitController units, Point selected)
        {
            int scaledTileWidth = (int)(camera.Scale * TileWidth + 0.5f);
            int scaledTileHeight = (int)(camera.Scale * TileHeight + 0.5f);

            Point minVisible = new Point(
                (int)(camera.Position.X / scaledTileWidth),
                (int)(camera.Position.Y / scaledTileHeight));
            Point maxVisible = new Point(
                ((int)(camera.Position.X + camera.Viewport.Width) / scaledTileWidth) + 1,
                ((int)(camera.Position.Y + camera.Viewport.Height) / scaledTileHeight) + 1);

            minVisible.X = Math.Max(minVisible.X, 0);
            minVisible.Y = Math.Max(minVisible.Y, 0);
            maxVisible.X = Math.Min(maxVisible.X, MapWidth);
            maxVisible.Y = Math.Min(maxVisible.Y, MapHeight);

            for (int i = 0; i < numLayers - 1; i++)
            {
                for (int y = minVisible.Y; y < maxVisible.Y; y++)
                {
                    for (int x = minVisible.X; x < maxVisible.X; x++)
                    {
                        if (map[i, y, x] >= 0)
                        {
                            batch.Draw(
                                texture,
                                new Rectangle(
                                    (x - minVisible.X) * scaledTileWidth + camera.Viewport.X - (int)camera.Position.X % scaledTileWidth,
                                    (y - minVisible.Y) * scaledTileHeight + camera.Viewport.Y - (int)camera.Position.Y % scaledTileHeight,
                                    scaledTileWidth,
                                    scaledTileHeight),
                                tiles[map[i, y, x]],
                                Color.White);
                        }
                    }
                }
            }

            //draw units
            if (units != null)
            {
                for (int y = minVisible.Y; y < maxVisible.Y; y++)
                {
                    for (int x = minVisible.X; x < maxVisible.X; x++)
                    {
                        units.draw(batch, x, y, new Rectangle(
                            (x - minVisible.X) * scaledTileWidth + camera.Viewport.X - (int)camera.Position.X % scaledTileWidth,
                            (y - minVisible.Y) * scaledTileHeight + camera.Viewport.Y - (int)camera.Position.Y % scaledTileHeight,
                            scaledTileWidth,
                            scaledTileHeight));
                    }
                }
            }

            //draw selected border
            if (SelectedTexture != null &&
                minVisible.X <= selected.X && selected.X < maxVisible.X &&
                minVisible.Y <= selected.Y && selected.Y < maxVisible.Y)
            {
                batch.Draw(
                    SelectedTexture,
                    new Rectangle(
                        (selected.X - minVisible.X) * scaledTileWidth + camera.Viewport.X - (int)camera.Position.X % scaledTileWidth,
                        (selected.Y - minVisible.Y) * scaledTileHeight + camera.Viewport.Y - (int)camera.Position.Y % scaledTileHeight,
                        scaledTileWidth,
                        scaledTileHeight),
                    Color.White);
            }
        }

        #endregion
    }
}
