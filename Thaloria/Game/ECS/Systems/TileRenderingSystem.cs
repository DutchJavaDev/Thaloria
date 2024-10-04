using DefaultEcs;
using DefaultEcs.System;
using Raylib_cs;
using Thaloria.Game.ECS.Components;
using Thaloria.Game.Map;
using Thaloria.Loaders;
using static Raylib_cs.Raylib;

namespace Thaloria.Game.ECS.Systems
{
  public sealed class TileRenderingSystem(World world, TileData[] tiles, MapLoader Map) : ISystem<float>
  {
    public bool IsEnabled { get; set; }
    private readonly TileData[] Tiles = tiles;
    private readonly Dictionary<Guid, int> AnimationFrameDictionary = tiles.Where(i => i.HasAnimation)
      .Select(i => new { Id = i.guid, Frame = 0 })
      .ToDictionary(i => i.Id, i => i.Frame);

    private float ElapsedTime = 0f;
    private readonly float UpdateTime = 0.145f;
    private bool UpdateFrames = false;

    public void Dispose()
    {}

    public void Update(float state)
    {
      ElapsedTime += state;
      UpdateFrames = false;

      if (ElapsedTime > UpdateTime)
      {
        ElapsedTime = 0f;
        UpdateFrames = true;
      }

      ref CameraComponent cameraComponent = ref world.Get<CameraComponent>();

      BeginMode2D(cameraComponent.Camera2D);
      for (int i = 0; i < Tiles.Length; i++)
      {
        ref TileData tile =  ref Tiles[i];

        var x = tile.RenderPosition.X;
        var y = tile.RenderPosition.Y;

        if (CheckCollisionRecs(cameraComponent.CameraView, new Rectangle(x, y, Map.TileWidth, Map.TileHeight)))
        {
          if (tile.HasAnimation)
          {
            var frameIndex = AnimationFrameDictionary[tile.guid];

            if (UpdateFrames) 
            {
              frameIndex++;

              if (frameIndex > tile.RenderFrames.Length-1)
              {
                frameIndex = 0;
              }

              AnimationFrameDictionary[tile.guid] = frameIndex;
            }
            DrawTextureRec(ResourceManager.GetTexture2DTileset(tile.TextureName), tile.RenderFrames[frameIndex], tile.RenderPosition, Color.White);
          }
          else
          {
            DrawTextureRec(ResourceManager.GetTexture2DTileset(tile.TextureName), tile.TexturePosition, tile.RenderPosition, Color.White);
          }
        }
      }
      EndMode2D();
    }
  }
}
