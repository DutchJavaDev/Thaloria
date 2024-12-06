using static Raylib_cs.Raylib;
using Thaloria.Game;
using System.Reflection;

namespace Thaloria
{
  static class Program
  {
    private readonly static ThaloriaGame Thaloria = new();
    public static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
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
