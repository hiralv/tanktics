using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tanktics
{
    class Hud : GameScreen
    {
        Texture2D hudTexture;
        ContentManager content;
        Rectangle position;

        public Hud(int x, int y, int width, int height)
        {
            position = new Rectangle(x, y, width, height);
            IsPopup = true;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            //load stuff
            hudTexture = content.Load<Texture2D>("HUD/hud2 copy");
            
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, true);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(hudTexture, position, Color.White);
            ScreenManager.SpriteBatch.End();
        }
    }

    class ButtonHud : Hud
    {
        public ButtonHud(int x, int y, int width, int height) : base(x,y,width,height)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
