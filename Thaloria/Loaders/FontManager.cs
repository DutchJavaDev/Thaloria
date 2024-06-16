using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Thaloria.Loaders
{
  public static class FontManager
  {
    private readonly static List<Font> Fonts = new List<Font>();
    
    public static void LoadFonts() 
    {
      Fonts.Add(LoadFont("Resources/fonts/IMMORTAL.ttf"));
    }

    public static Font GetFont(int index)
    {
      return Fonts[index];
    }

    public static void DisposeFonts()
    {
      foreach (Font font in Fonts)
      {
        UnloadFont(font);
      }
    }
  }
}
