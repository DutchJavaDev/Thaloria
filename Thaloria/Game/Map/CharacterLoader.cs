using Raylib_cs;
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
      var path = AssemblyDataLoader.CreateTilesetResourcePath("characters.json");

      var characterAtlas = await AssemblyDataLoader.DeserilizeResouceFromStream<TileAtlas>(path);

      CustomTileLoader.LoadAtlasData(characterAtlas);

      ResourceManager.LoadResourceTexture2DTileset(ResourceNames.CharaterTileSet,characterAtlas.Atlas.ImagePath);
    }

    public Rectangle GetCharacterRectangle(string name) => CustomTileLoader.GetRectangle(name);
  }
}
