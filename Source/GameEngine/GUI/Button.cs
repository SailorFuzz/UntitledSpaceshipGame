using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public class Button : GUIElement
    {
        public float TextScale { get; set; }
        public override void Update()
        {
            if (InputManager.IsMouseReleased(0) &&
                    Bounds.Contains(InputManager.GetMousePosition()))
                OnAction();
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);
            spriteBatch.DrawString(font, Text, new Vector2(Bounds.X + Bounds.Width/2 - TextScale*font.MeasureString(Text).X/2, Bounds.Y + Bounds.Height/2 - TextScale * font.MeasureString(Text).Y/2), Color.Black, 0, Vector2.Zero, TextScale, new SpriteEffects(), 0);
        }
    }

}
