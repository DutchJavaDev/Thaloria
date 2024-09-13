using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Thaloria.Loaders
{
  public static class ResourceManager
  {
    private static readonly Dictionary<string, Font> Fonts = [];
    private static readonly Dictionary<string, Texture2D> Textures = [];
    private static readonly string FontResourceFolder = "Resources/Fonts/";
    private static readonly string TilesetResourceFolder = "Resources/Tilesets/";

    public static void LoadResourceFont(string name, string fileName)
    {
      if (Fonts.ContainsKey(name))
        return;

      var path = $"{FontResourceFolder}{fileName}";

      var font = LoadFont(path);

      Fonts.TryAdd(name, font);
    }

    public static void LoadResourceTexture2DTileset(string name, string fileName)
    {
      if (Textures.ContainsKey(name))
        return;

      var path = $"{TilesetResourceFolder}{fileName}";
      
      var texture = LoadTexture(path);
      
      Textures.TryAdd(name, texture);
    }

    public static Font GetFont(string name)
    {
      return Fonts[name];
    }

    public static Texture2D GetTexture2DTileset(string name)
    {
      return Textures[name];
    }

    public static void Dispose()
    {
      foreach (Font font in Fonts.Values)
      {
        UnloadFont(font);
      }
      Fonts.Clear();

      foreach (Texture2D texture in Textures.Values)
      {
        UnloadTexture(texture);
      }
      Textures.Clear();
    }
  }
}
