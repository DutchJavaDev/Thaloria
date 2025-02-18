﻿using Raylib_cs;
using System.Reflection;
using System.Text.Json;
using Thaloria.Game.Helpers;
using Thaloria.Loaders;

namespace Thaloria.Game.Map
{
  public sealed class CharacterLoader
  {
    private static readonly Assembly CurrentAssembly = Program.CurrentAssembly;

    private readonly CustomTileLoader CustomTileLoader = new();
    public async Task LoadCharacters()
    {
      var path = CreateResourcePath("Tilesets","characters.json");

      var characterAtlas = await DeserilizeResouceFromStream<TileAtlas>(path);

      CustomTileLoader.LoadAtlasData(characterAtlas);

      ResourceManager.LoadResourceTexture2DTileset(ResourceNames.CharaterTileSet,characterAtlas.Atlas.ImagePath);
    }

    public Rectangle GetCharacterRectangle(string name) => CustomTileLoader.GetRectangle(name);

    private static async Task<T?> DeserilizeResouceFromStream<T>(string path) where T : class
    {
      using var resourceStream = CurrentAssembly.GetManifestResourceStream(path);

      using var resourceStreamReader = new StreamReader(resourceStream);

      return JsonSerializer.Deserialize<T>(await resourceStreamReader?.ReadToEndAsync());
    }
    private static string CreateResourcePath(string mapName , string name)
    {
      return $"Thaloria.Resources.{mapName}.{name}";
    }
  }
}
