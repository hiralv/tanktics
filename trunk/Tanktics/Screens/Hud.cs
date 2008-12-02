using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tanktics
{
    class Hud
    {
        public Texture2D hudTexture;
        public Rectangle position;
        public Texture2D blank;

        public Hud(int x, int y, int width, int height)
        {
            position = new Rectangle(x, y, width, height);
        }

        public virtual void LoadContent(ContentManager content)
        {
            if (content == null)
                return;

            //load stuff

            hudTexture = content.Load<Texture2D>("HUD/hud2 copy");

        }

        public virtual void Draw(SpriteBatch batch)
        {

            //Viewport port = ScreenManager.GraphicsDevice.Viewport;

            //port.X = 0;
            //port.Y = 450;
            //port.Width = 800;
            //port.Height = 650;

            //ScreenManager.SpriteBatch.Begin();
            //ScreenManager.SpriteBatch.Draw(hudTexture, position, Color.White);
            //ScreenManager.SpriteBatch.End();

            batch.Draw(hudTexture, position, Color.White);

        }
    }

    class ButtonHud : Hud
    {
        public Texture2D next;
        public Texture2D previous;
        public Texture2D done;
        public Texture2D finalize;

        public ButtonHud(int x, int y, int width, int height)
            : base(x, y, width, height)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            if (content == null)
                return;

            //base.LoadContent(content);
            //load stuff

            hudTexture = content.Load<Texture2D>("HUD/hud3 copy");
            blank = content.Load<Texture2D>("HUD/mud texture copy");
            next = content.Load<Texture2D>("HUD/next copy");
            previous = content.Load<Texture2D>("HUD/previous copy");
            finalize = content.Load<Texture2D>("HUD/finalize copy");
            done = content.Load<Texture2D>("HUD/done copy");
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(blank, position, Color.White);
            batch.Draw(hudTexture, position, Color.White);
            
            batch.Draw(done, new Vector2(340, 500), null, Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
            batch.Draw(previous, new Vector2(380, 500), null, Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
            batch.Draw(next, new Vector2(420, 500), null, Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
            batch.Draw(finalize, new Vector2(340, 550), null, Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
            //batch.Draw(previous, new Vector2(390, 550), null, Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
            //batch.Draw(next, new Vector2(440, 550), null, Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
        }
    }

    class DataHud : Hud
    {
        public DataHud(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            if (content == null)
                return;

            //load stuff
            hudTexture = content.Load<Texture2D>("HUD/hud3 copy");
            blank = content.Load<Texture2D>("HUD/mud texture copy");
            

        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(blank, position, Color.White);
            batch.Draw(hudTexture, position, Color.White);
        }
    }

    class ModelHud : Hud
    {
        AnimatingSprite currentUnit;
        Rectangle spriteDest;

        public ModelHud(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            currentUnit = new AnimatingSprite();
            currentUnit.Animations.Add("rotate", new Animation(2000, 260, 50, 5, 10, 0, 0));
            currentUnit.CurrentAnimation = "rotate";

            Rectangle spriteRect = currentUnit.Animations["rotate"].CurrentFrame;
            //find the scale needed to make frame fit in hud rectangle
            float scale = Math.Min(
                0.8f * (position.Width - 20) / spriteRect.Width,
                0.8f * (position.Height - 20) / spriteRect.Height);

            spriteDest = new Rectangle(
                0, 0,
                (int)(scale * spriteRect.Width),
                (int)(scale * spriteRect.Height));
            //center sprite in hud rectangle
            spriteDest.X = position.X + (position.Width - 20) / 2 - spriteDest.Width / 2;
            spriteDest.Y = position.Y + 20 + (position.Height - 20) / 2 - spriteDest.Height / 2;
        }

        public override void LoadContent(ContentManager content)
        {
            if (content == null)
                return;

            //load stuff
            hudTexture = content.Load<Texture2D>("HUD/hud5 copy");
            blank = content.Load<Texture2D>("HUD/mud texture copy");

            currentUnit.Texture = content.Load<Texture2D>("Unit Animations/Tank/Tank Rotating/Tank Rotating White");
        }

        public void Update(GameTime gameTime)
        {
            currentUnit.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(blank, position, Color.White);
            batch.Draw(hudTexture, position, Color.White);
            currentUnit.Draw(batch, spriteDest);
        }
    }

    class GraphHud : Hud
    {
        Graph graph1, graph2, graph3, graph4;

        //Texture2D graph;
        public GraphHud(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            if (content == null)
                return;

            //load stuff
            hudTexture = content.Load<Texture2D>("HUD/hud3 copy");
            blank = content.Load<Texture2D>("HUD/mud texture copy");
            graph1 = new Graph(18, Color.Red, position.X + 30, position.Y + 120, content);
            graph2 = new Graph(63, Color.Green, position.X + 50, position.Y + 120, content);
            graph3 = new Graph(36, Color.Blue, position.X + 70, position.Y + 120, content);
            graph4 = new Graph(54, Color.Yellow, position.X + 90, position.Y + 120, content);
            //graph = content.Load<Texture2D>("HUD/graph");
        }

        public void Draw(SpriteBatch batch, ScreenManager screenManager)
        {
            SpriteFont font = screenManager.Font;

            batch.Draw(blank, position, Color.White);
            batch.Draw(hudTexture, position, Color.White);
            batch.DrawString(font, "Graph", new Vector2(position.X + 30, position.Y + 10), Color.Blue);
            graph1.Draw(batch);
            graph2.Draw(batch);
            graph3.Draw(batch);
            graph4.Draw(batch);
            //batch.Draw(graph, new Vector2(500, 500), null, Color.White, 0.0f, Vector2.Zero, 0.2f, SpriteEffects.None, 0.0f);
        }
    }

    class Graph
    {
        int value;
        Color color;
        int posx, posy;
        Texture2D texture;

        public Graph(int value, Color color, int posx, int posy, ContentManager content)
        {
            this.value = value;
            this.color = color;
            this.posx = posx;
            this.posy = posy;
            texture = content.Load<Texture2D>("blank");
        }

        public void Draw(SpriteBatch batch)
        {
            for (int i = 0; i < value; i++)
            {
                for(int j=0; j< 10; j++)
                    batch.Draw(texture, new Vector2(posx + j, posy - i), color);
                
            }
        }
    }
}
