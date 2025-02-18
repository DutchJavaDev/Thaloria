﻿using static Raylib_cs.Raylib;
using Raylib_cs;
using System.Numerics;
using Thaloria.Game.Abstract;
using Thaloria.Game.ECS.Components;
using Thaloria.Game.Map;
using Thaloria.Game.Physics;
using Thaloria.Loaders;
using DefaultEcs;
using Thaloria.Game.Helpers;
using Thaloria.Game.ECS.Class;

namespace Thaloria.Game.ECS.Systems
{
  public sealed class RenderPipelineSystem : CustomRender
  {
    private readonly List<YIndexItem>  yIndexItems;
    private readonly World _world;
    private EntitySet _entitySet;

    private float ElapsedTime = 0f;
    private readonly float UpdateTime = 0.145f;
    private bool UpdateFrames = false;
    private Font _imortalFont;

    public RenderPipelineSystem(World world, TileData[] tileData) 
    {
      _world = world;
      _imortalFont = ResourceManager.GetFont(ResourceNames.ImmortalFont);
      var fixedAnimations = tileData.Where(i => i.FixedAnimation).ToDictionary(i => i.TileGuid, i => false);

      foreach (var animation in fixedAnimations) 
      {
        ThaloriaStatic.FixedAnimations.Add(animation.Key, animation.Value);
      }

      yIndexItems = tileData.Select(i => new YIndexItem 
      {
        LayerId = i.LayerId,
        // If i ever need the tile id for some reason then I have to fix this...4
        Id = i.TileId * i.TileId + 1000, // :)
        isPlayer = false,
        hasTexture = true,
        Position = i.RenderPosition,
        TexturePosition = i.HasAnimation ? i.RenderFrames[0] : i.TexturePosition,
        YIndex = (int)(i.RenderPosition.Y + (i.HasAnimation ? i.RenderFrames[0].Height : i.TexturePosition.Height)),
        TextureName = i.TextureName,
        isStatic = true, // ??? 
        HasAnimation = i.HasAnimation,
        AnimationFrames = i.HasAnimation ? i.RenderFrames : default,
        Guid = i.TileGuid,
        FixedAnimation = i.FixedAnimation
      }).ToList();

      // TODO needs testing
      _entitySet = _world.GetEntities()
                           .WithEither<RenderComponent>()
                           .Or<AnimationComponent>().AsSet();


      // This can cause some performance issues or bugs
      _world.SubscribeComponentAdded<RenderComponent>(UpdateEntitySet);
      _world.SubscribeComponentAdded<AnimationComponent>(UpdateEntitySet);
      _world.SubscribeComponentRemoved<RenderComponent>(UpdateEntitySet);
      _world.SubscribeComponentRemoved<AnimationComponent>(UpdateEntitySet);
    }

    // Update when either a RenderComponent has been added or removed
    private void UpdateEntitySet<T>(in Entity entity, in T component)
    {
      // TODO needs testing
      _entitySet = _world.GetEntities()
                     .WithEither<RenderComponent>()
                     .Or<AnimationComponent>().AsSet();
    }

    // Organize everything based on y index
    // Alle entities with renderComponent
    // Render based of y index
    public override void PreRender(float state)
    {
      foreach (var entity in _entitySet.GetEntities()) 
      {
        var id = entity.GetHashCode();

        var hasAnimationComponent = entity.Has<AnimationComponent>();
        var hasPlayerComponent = entity.Has<PlayerComponent>();

        ref RenderComponent renderComponent = ref entity.Get<RenderComponent>();

        var physicsBody = PhysicsWorld.Instance.GetBodyByEntityTag(id);
        var position = physicsBody.Position;

        // center on sprite

        // This is a buf that is fixed with this crazy if statement lol..
        if (renderComponent.TextureWidth == 48)
        {
          position.X -= renderComponent.TextureWidth / 2;
          position.Y -= renderComponent.TextureHeight / 1.5f;
        }
        else
        {
          position.X -= renderComponent.TextureWidth / 2;
          position.Y -= renderComponent.TextureHeight / 2;
        }

        var frameX = 0f;
        var frameY = 0f;
        var flipTexture = false;

        if (hasAnimationComponent)
        {
          ref AnimationComponent animationComponent = ref entity.Get<AnimationComponent>();
          var frame = animationComponent.GetFramePosition();
          frameX = frame.X;
          frameY = frame.Y; 
          flipTexture = animationComponent.IsFlipped();
        }

        var yIndexId = yIndexItems.FindIndex(i => i.Id == id);

        if (yIndexId == -1) 
        {
          // not added, need to be added
          yIndexItems.Add(new YIndexItem
          {
            Id = id,
            hasTexture = renderComponent.HasTexture,
            isPlayer = hasPlayerComponent,
            Color = renderComponent.RenderColor,
            Position = new Vector2(position.X, position.Y),
            TexturePosition = new Rectangle(frameX, frameY, renderComponent.TextureWidth, renderComponent.TextureHeight),
            TextureName = renderComponent.HasTexture ? renderComponent.TextureName : string.Empty,
            YIndex = (int)(position.Y + renderComponent.TextureHeight),
            FlipTexure = flipTexture,
          });
        }
        else
        {
          // Update
          var YindexItem = yIndexItems[yIndexId];
          YindexItem.Position = new Vector2(position.X, position.Y);
          YindexItem.YIndex = (int)(position.Y + renderComponent.TextureHeight);
          YindexItem.TexturePosition.X = frameX;
          YindexItem.TexturePosition.Y = frameY;
          YindexItem.FlipTexure = flipTexture;
          yIndexItems[yIndexId] = YindexItem;
        }
      }

      ElapsedTime += state;
      UpdateFrames = false;

      if (ElapsedTime > UpdateTime)
      {
        ElapsedTime = 0f;
        UpdateFrames = true;
      }

      // refactor this, make it more readable
      if (UpdateFrames) 
      {
        // Update animated tiles
        var animatedTiles = yIndexItems.Where(i => i.HasAnimation).ToList();

        foreach (var animatedTile in animatedTiles)
        {

          if (animatedTile.FixedAnimation)
          {
            if (!ThaloriaStatic.FixedAnimations[animatedTile.Guid])
            {
              continue;
            }
          }

          var index = yIndexItems.FindIndex(i => i.Guid == animatedTile.Guid);

          var yindexItem = yIndexItems[index];

          // Update
          var frame = yindexItem.AnimationFrame;

          frame++;

          if (animatedTile.FixedAnimation) 
          {
            if (frame > yindexItem.AnimationFrames.Length - 1)
            {
              frame = 0;
            }
            ThaloriaStatic.FixedAnimations[animatedTile.Guid] = false;
          }
          else
          {
            if (frame > yindexItem.AnimationFrames.Length - 1)
            {
              frame = 0;
            }
          }

          yindexItem.AnimationFrame = frame;
          yindexItem.TexturePosition = yindexItem.AnimationFrames[frame];

          // set back
          yIndexItems[index] = yindexItem;
        }
      }

      // Sort on yindex
      yIndexItems.Sort((a,b) => 
      {
        // always render items on layer 5 first or atleast before the player
        int layerComparison = (a.LayerId == 5 ? 0 : 1).CompareTo(b.LayerId == 5 ? 0 : 1);
        return layerComparison != 0 ? layerComparison : a.YIndex.CompareTo(b.YIndex); ;
      });
    }

    public override void Render(float state)
    {
      ref CameraComponent cameraComponent = ref _world.Get<CameraComponent>();
      var cameraView = cameraComponent.CameraView;
      
      BeginMode2D(cameraComponent.Camera2D);
      foreach (var item in yIndexItems.Where(i => CheckCollisionRecs(cameraView,i.Bounds))) 
      {
        var x = item.Position.X;
        var y = item.Position.Y;
        var width = item.TexturePosition.Width;
        var height = item.TexturePosition.Height;
        var texturePosition = item.TexturePosition;

        if (item.FlipTexure && item.hasTexture)
        {
          texturePosition.Width *= -1;
        }

        if ((item.isStatic && item.hasTexture) || (item.hasTexture && !item.isStatic))
        {
          // Draw a the texture of the static component
          // Draw dynamic and animated texture
          DrawTextureRec(ResourceManager.GetTexture2DTileset(item.TextureName), texturePosition, item.Position, Color.White);
        }
        
        if (!item.hasTexture)
        {
          // Draw rectangle incase I forgot to add a texture
          DrawRectangle((int)x,(int)y,(int)width,(int)height,item.Color);
        }
      }
      EndMode2D();
    }
    public override void PostRender(float state)
    {
      return;
    }
    public override void Dispose()
    {
      return;
    }
  }

  struct YIndexItem(int layer,
                    int Id, 
                    int yIndex, 
                    bool isPlayer, 
                    bool hasTexture, 
                    Color color, 
                    Vector2 position, 
                    Rectangle texturePosition, 
                    string textureName, 
                    bool isStatic = false,
                    bool flipTextue = false,
                    bool animation = false,
                    Rectangle[] animationFrames = default,
                    Guid uniqueid = default,
                    bool fixedAnimation = default)
  {
    public int LayerId = layer;
    public int Id = Id;
    public int YIndex = yIndex;
    public bool isPlayer = isPlayer;
    public bool hasTexture = hasTexture;
    public Color Color = color;
    public Vector2 Position = position;
    public Rectangle TexturePosition = texturePosition;
    public string TextureName = textureName;
    public bool isStatic = isStatic;
    public bool FlipTexure = flipTextue;
    public bool HasAnimation = animation;
    public Rectangle[] AnimationFrames = animationFrames;
    public int AnimationFrame = 0;
    public Guid Guid = uniqueid;
    public bool FixedAnimation = fixedAnimation;
    public readonly Rectangle Bounds => new(Position.X,Position.Y,TexturePosition.Width,TexturePosition.Height);
  }
}
