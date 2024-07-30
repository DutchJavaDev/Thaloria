using Raylib_cs;
using Thaloria.Loaders;
using static Raylib_cs.Raylib;
using Thaloria.Game;

namespace Thaloria
{
    static class Program
    {
        private readonly static ThaloriaGame Thaloria = new();
        static async Task Main(string[] args)
        {
            var windowWidth = 1280;
            var windowHeight = 860;

            SetConfigFlags(ConfigFlags.VSyncHint);
            InitWindow(windowWidth, windowHeight, "Thaloria");
            SetWindowMinSize(windowWidth, windowHeight);

            SetTargetFPS(45);

            FontManager.LoadFonts();
            Thaloria.Init();
            
            while (!WindowShouldClose())
            {
                Thaloria.Run();
            }

            await Thaloria.Dispose();

            FontManager.DisposeFonts();
            CloseWindow();
        }
    }
}
