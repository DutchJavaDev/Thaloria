using System.Reflection;
using Raylib_cs;
using Thaloria.Game;
using Thaloria.Game.Helpers;

namespace Thaloria.Loaders
{
  public sealed class CharacterLoader
  {
    private readonly CustomTileLoader CustomTileLoader = new();
    public async Task LoadCharacters()
    {
      var path = AssemblyDataLoader.CreateTilesetResourcePath("characters.json");

      var characterAtlas = await AssemblyDataLoader.DeserilizeResouceFromStreamAsync<TileAtlas>(path);

      CustomTileLoader.LoadAtlasData(characterAtlas?? new());

      ResourceManager.LoadResourceTexture2DTileset(ResourceNames.CharaterTileSet,characterAtlas?.Atlas?.ImagePath ?? "");
    }

    public Rectangle GetCharacterRectangle(string name) => CustomTileLoader.GetRectangle(name);
  }
}
