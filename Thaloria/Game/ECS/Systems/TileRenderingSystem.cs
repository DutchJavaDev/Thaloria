using DefaultEcs;
using DefaultEcs.System;
using Raylib_cs;
using Thaloria.Game.ECS.Components;
using Thaloria.Game.Helpers;
using Thaloria.Game.Map;
using Thaloria.Loaders;
using static Raylib_cs.Raylib;

namespace Thaloria.Game.ECS.Systems
{
  public sealed class TileRenderingSystem(World world, TileData[] tiles, MapLoader Map) : ISystem<float>
  {
    public bool IsEnabled { get; set; }
    private readonly TileData[] Tiles = tiles;

    public void Dispose()
    {}

    public void Update(float state)
    {
      ref CameraComponent cameraComponent = ref world.Get<CameraComponent>();

      BeginMode2D(cameraComponent.Camera2D);
      for (int i = 0; i < Tiles.Length; i++)
      {
        ref TileData tile =  ref Tiles[i];

        var x = tile.RenderPosition.X;
        var y = tile.RenderPosition.Y;

        if (CheckCollisionRecs(cameraComponent.CameraView, new Rectangle(x, y, Map.TileWidth, Map.TileHeight)))
        {
          DrawTextureRec(ResourceManager.GetTexture2DTileset(tile.TextureName), tile.TexturePosition, tile.RenderPosition, Color.White);
        }
      }
      EndMode2D();
    }
  }
}
