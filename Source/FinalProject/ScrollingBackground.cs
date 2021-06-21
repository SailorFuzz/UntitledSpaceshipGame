using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GameEngine;

namespace FinalProject
{
    public class ScrollingBackground
    {
        public Texture2D Texture { get; set; }
        private Rectangle Rectangle { get; set; }
        private Rectangle Rectangle2 { get; set; }
        public int Speed { get; set; }

        

        public ScrollingBackground(Texture2D texture)
        {
            float rescale = ScreenManager.Width / texture.Width;

            Texture = texture;
            Rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle2 = new Rectangle(0, -texture.Height, texture.Width, texture.Height);


        }

        public void Update()
        {
            Rectangle = new Rectangle(0, Rectangle.Y + Speed, Texture.Width, Texture.Height);
            Rectangle2 = new Rectangle(0, Rectangle2.Y + Speed, Texture.Width, Texture.Height);

            if (Rectangle.Y > ScreenManager.Height)
                Rectangle = new Rectangle(0, Rectangle2.Y - Texture.Height, Texture.Width, Texture.Width);
            if (Rectangle2.Y > ScreenManager.Height)
                Rectangle2 = new Rectangle(0, Rectangle.Y - Texture.Height, Texture.Width, Texture.Width);


        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rectangle, Color.White);
            spriteBatch.Draw(Texture, Rectangle2, Color.White);
        }
    }


}
