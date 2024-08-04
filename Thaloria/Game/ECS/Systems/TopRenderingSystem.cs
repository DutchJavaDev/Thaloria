using DefaultEcs;
using DefaultEcs.System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Thaloria.Game.ECS.Components;
using Thaloria.Game.Map;
using System.Numerics;

namespace Thaloria.Game.ECS.Systems
{
  [With(typeof(PlayerComponent))]
  public sealed class TopRenderingSystem : AEntitySetSystem<float>
  {
    private readonly List<YbufferComponent> ybufferComponents = [];
    private readonly int PlayerYBufferId = 999;

    public TopRenderingSystem(World world, TileData[] topTileData) : base(world)
    {
      ybufferComponents = topTileData.Select(i => new YbufferComponent 
      {
        Id = i.TileId,
        IsPlayer = false,
        HasTexture = true,
        Position = i.RenderPosition,
        TexturePosition = i.TexturePosition,
        YIndex = (int)(i.RenderPosition.Y + i.TexturePosition.Height)
      }).ToList();
    }

    protected override void PreUpdate(float state)
    {
      ref readonly Entity player = ref Set.GetEntities()[0];
      ref PositionComponent playerPosition = ref player.Get<PositionComponent>();
      ref BodyComponent playerBody = ref player.Get<BodyComponent>();
      ref RenderComponent playerRender = ref player.Get<RenderComponent>();

      var playerYBufferIndex = ybufferComponents.FindIndex(i => i.Id == PlayerYBufferId);

      // Not found need to be added
      if (playerYBufferIndex == -1)
      {
        ybufferComponents.Add(new YbufferComponent
        {
          Id = PlayerYBufferId,
          IsPlayer = true,
          HasTexture = false, // Turn true when texture added
          Color = playerRender.RenderColor,
          Position = playerPosition.Position,
          TexturePosition = new Rectangle(playerPosition.X,playerPosition.Y,playerBody.Width,playerBody.Height),
          YIndex = (int)(playerPosition.Y+playerBody.Height),
        });
      }
      else // Update position etc...
      {
        var playerYBufferComp = ybufferComponents[playerYBufferIndex];

        playerYBufferComp.Position = playerPosition.Position;
        playerYBufferComp.YIndex = (int)(playerPosition.Position.Y + playerBody.Height);

        ybufferComponents[playerYBufferIndex] = playerYBufferComp;
      }

      // Sort based of y position
      ybufferComponents.Sort((a, b) => a.YIndex.CompareTo(b.YIndex));
    }

    protected override void Update(float state, in Entity entity)
    {
      // TODO load other textures
      ref Texture2D TileTexture = ref World.Get<Texture2D>();
      ref CameraComponent cameraComponent = ref World.Get<CameraComponent>();

      BeginMode2D(cameraComponent.Camera2D);
      foreach (var item in ybufferComponents)
      {
        var x = item.Position.X;
        var y = item.Position.Y;
        var width = item.TexturePosition.Width;
        var height = item.TexturePosition.Height;

        if (!CheckCollisionRecs(cameraComponent.CameraView, new Rectangle(x, y, width, height)))
        {
          continue;
        }

        if (item.HasTexture)
        {
          DrawTextureRec(TileTexture, item.TexturePosition, item.Position, Color.White);
        } // TODO when player has texture
        else
        {
          var position = new Vector2(x,y);
          var size = new Vector2(width,height);
          DrawRectangleV(position, size, item.Color);
        }
      }
      EndMode2D();
    }
  }

  struct YbufferComponent(int Id, int yIndex, bool isPlayer, bool hasTexture, Color color, Vector2 position, Rectangle texturePosition)
  {
    public int Id = Id;
    public int YIndex = yIndex;
    public bool IsPlayer = isPlayer;
    public bool HasTexture = hasTexture;
    public Color Color = color;
    public Vector2 Position = position;
    public Rectangle TexturePosition = texturePosition;
  }
}
