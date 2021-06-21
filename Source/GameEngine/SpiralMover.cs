using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using GameEngine;

namespace GameEngine
{
    public class SpiralMover
    {
        
        public Sprite Sprite { get; set; }
        public Vector2  Position { get; set; }
        public float Radius { get; set; }
        public float Phase { get; set; }
        public float Speed { get; set; }




        public SpiralMover(Texture2D texture, Vector2 position, float radius = 150)
        {
            Sprite = new Sprite(texture);
            Position = position;
            Radius = radius;
            Speed = 1;
        }

        public void Update()
        {
            Position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Phase += Time.ElapsedGameTime * Speed;

            Sprite.Position = Position + new Vector2((float)(Radius * Math.Cos(Phase)), (float)(Radius * Math.Sin(Phase)));
        }

        public void Draw(SpriteBatch spritebatch)
        {
            Sprite.Draw(spritebatch);
        }

    }
}
