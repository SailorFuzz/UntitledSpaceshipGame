﻿using System;

namespace FinalProject
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Final())
                game.Run();
        }
    }
}
