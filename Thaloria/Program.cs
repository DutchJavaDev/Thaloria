using static Raylib_cs.Raylib;
using Thaloria.Game;
using System.Reflection;

namespace Thaloria
{
  internal static class Program
  {
    private static readonly ThaloriaGame Thaloria = new();
    public static Assembly CurrentAssembly => Assembly.GetExecutingAssembly();
    private static async Task Main(string[] args)
    {
      const int windowWidth = 1280;
      const int windowHeight = 860;

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
