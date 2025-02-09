using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Thaloria.Loaders
{
  public static class ResourceManager
  {
    private static readonly Dictionary<string, Font> Fonts = [];
    private static readonly Dictionary<string, Texture2D> Textures = [];

    public static void LoadResourceFont(string name, string fileName)
    {
      if (Fonts.ContainsKey(name))
        return;
      
      var extension = Path.GetExtension(fileName);
      
      var path = AssemblyDataLoader.CreateFontResourcePath(fileName);
      
      using var stream = AssemblyDataLoader.GetResourceStream(path);
      
      var font = LoadFontFromMemory(extension, stream.ToArray(), 18, null,0);
      
      Fonts.TryAdd(name, font);
    }

    public static void LoadResourceTexture2DTileset(string name, string fileName)
    {
      if (Textures.ContainsKey(name))
        return;
      
      var extension = Path.GetExtension(fileName);
      
      var path = AssemblyDataLoader.CreateTilesetResourcePath(fileName);
      
      using var stream = AssemblyDataLoader.GetResourceStream(path);
        
      var texture = LoadTextureFromImage(LoadImageFromMemory(extension, stream.ToArray()));

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
