using Raylib_cs;
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

      //SetConfigFlags(ConfigFlags.VSyncHint);
      InitWindow(windowWidth, windowHeight, "Thaloria");

      SetTargetFPS(60);
      
      SetWindowMinSize(windowWidth, windowHeight);

      Thaloria.Init();

      while (!WindowShouldClose())
      {
        Thaloria.Run();
      }

      await Thaloria.Dispose();
      CloseWindow();
    }
  }
}
