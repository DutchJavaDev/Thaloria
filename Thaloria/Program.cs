using Raylib_cs;
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
            var windowWidth = 640;
            var windowHeight = 480;

            SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint);
            InitWindow(windowWidth, windowHeight, "Thaloria");
            SetWindowMinSize(640, 480);

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
