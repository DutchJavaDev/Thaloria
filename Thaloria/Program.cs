﻿using Raylib_cs;
using Thaloria.Loaders;
using static Raylib_cs.Raylib;
using Thaloria.World;

namespace Thaloria
{
    static class Program
    {
        private readonly static ThaloriaGame Thaloria = new();
        static unsafe void Main(string[] args)
        {
            var windowWidth = 1600;
            var windowHeight = 900;

            SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint);
            InitWindow(windowWidth, windowHeight, "Thaloria");
            SetWindowMinSize(320, 180);

            SetTargetFPS(45);

            FontManager.LoadFonts();
            Thaloria.Init();

            while (!WindowShouldClose())
            {
                Thaloria.Run();
            }
            Thaloria.Dispose();
            FontManager.DisposeFonts();
            CloseWindow();
        }
    }
}
