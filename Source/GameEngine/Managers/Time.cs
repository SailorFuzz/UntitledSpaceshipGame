﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine
{
    public static class Time
    {
        public static float ElapsedGameTime { get; private set; }
        public static TimeSpan TotalGameTime { get; private set; }

        public static void Initialize()
        {
            ElapsedGameTime = 0;
            TotalGameTime = new TimeSpan(0);
        }

        public static void Update(GameTime gameTime)
        {
            ElapsedGameTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;
            TotalGameTime = gameTime.TotalGameTime;
        }

    }
}