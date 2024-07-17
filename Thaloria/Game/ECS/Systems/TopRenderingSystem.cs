using DefaultEcs;
using DefaultEcs.System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Thaloria.Game.ECS.Components;
using Thaloria.Game.Map;
using Thaloria.Game.Helpers;
using System.Numerics;

namespace Thaloria.Game.ECS.Systems
{
  [With(typeof(PlayerComponent))]
  public sealed class TopRenderingSystem(World world, MapLoader Map) : AEntitySetSystem<float>(world)
  {
    private readonly TileData[] Tiles = Map.TileData.Where(i => i.LayerId == 2).ToArray();
    private bool _playerDrawn = false;

    protected override void Update(float state, in Entity entity)
    {
      _playerDrawn = false;
      ref PlayerComponent playerComponent = ref entity.Get<PlayerComponent>();
      ref Texture2D TileTexture = ref World.Get<Texture2D>();
      ref CameraComponent cameraComponent = ref World.Get<CameraComponent>();

      var items = Tiles.Select(i => new ZbufferItem
      {
        hasTexture = true,
        Position = i.RenderPosition,
        TexturePosition = i.TexturePosition,
        YDepthindex = (int)((i.RenderPosition.Y + i.TexturePosition.Height))
      }).ToList();

      items.Add(new ZbufferItem
      {
        hasTexture = false,
        Color = playerComponent.Color,
        Position = playerComponent.Body.Position,
        TexturePosition = playerComponent.Body,
        YDepthindex = (int)(playerComponent.Body.Position.Y + (playerComponent.Body.Height))
      });

      items.Sort((a, b) => a.YDepthindex.CompareTo(b.YDepthindex));

      BeginMode2D(cameraComponent.Camera2D);

      foreach (var item in items)
      {
        var x = item.Position.X;
        var y = item.Position.Y;
        var width = item.TexturePosition.Width;
        var height = item.TexturePosition.Height;

        if (!CheckCollisionRecs(cameraComponent.CameraView, new Rectangle(x, y, width, height)))
        {
          continue;
        }

        if (item.hasTexture)
        {
          DrawTextureRec(TileTexture, item.TexturePosition, item.Position, Color.White);
          var bounds = new Rectangle 
          {
            Position = item.Position,
            Width = item.TexturePosition.Width,
            Height = item.TexturePosition.Height
          };

          DrawRectangleLinesEx(bounds,0.5f,Color.Red);
        }
        else
        {
          DrawRectangleRec(item.TexturePosition,item.Color);
        }
      }

      EndMode2D();
    }
  }

  class ZbufferItem 
  {
    public int YDepthindex {  get; set; }
    public bool hasTexture {  get; set; }
    public Color Color { get; set; }
    public Vector2 Position { get; set; }
    public Rectangle TexturePosition { get; set; }
  }
}
