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
        Texture2D hudTexture;
        Rectangle position;

        public Hud(int x, int y, int width, int height)
        {
            position = new Rectangle(x, y, width, height);
        }

        public void LoadContent(ContentManager content)
        {
            if (content == null)
                return;

            //load stuff
            hudTexture = content.Load<Texture2D>("HUD/hud2 copy");

        }

        //public void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        //{
        //    base.Update(gameTime, otherScreenHasFocus, true);
        //}

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(hudTexture, position, Color.White);
        }
    }

    //class ButtonHud : Hud
    //{
    //    public ButtonHud(int x, int y, int width, int height) : base(x,y,width,height)
    //    {
    //    }

    //    public override void LoadContent()
    //    {
    //        base.LoadContent();
    //    }

    //    public override void Draw(GameTime gameTime)
    //    {
    //        base.Draw(gameTime);
    //    }
    //}
}
