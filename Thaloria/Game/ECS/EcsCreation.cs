using DefaultEcs;
using System.Numerics;
using Thaloria.Game.ECS.Components;
using Thaloria.Game.Helpers;
using Thaloria.Game.Map.Tiled;
using Thaloria.Game.Physics;
using Thaloria.Loaders;

namespace Thaloria.Game.ECS
{
  public static class EcsCreation
  {
    private static readonly World _world;

    public static World Instance => _world;

    static EcsCreation()
    {
      _world = new World();
      _world.SetMaxCapacity<CameraComponent>(1);
      _world.SetMaxCapacity<PlayerComponent>(1);
    }

    public static void SetWorldComponent<T>(T component) where T : struct
    {
      _world.Set(component);
    }

    public static void SpawnNpcs(IEnumerable<TiledCollisionObject> npcs)
    {
      foreach (var spawn in npcs)
      {
        spawn.TryGetIntProperty("npc", out var npcType);
        spawn.TryGetIntProperty("amount", out var amount);
        CreateNpc((ThaloriaNpc)npcType,amount,spawn.Xf,spawn.Yf);
      }
    }

    private static void CreateNpc(ThaloriaNpc npcType, int amount, float x, float y)
    {
      switch (npcType)
      {
        case ThaloriaNpc.Skeleton:
          var tilesetName = "skeleton_swordless.png";
          // TODO fix this so it gets called only once....
          ResourceManager.LoadResourceTexture2DTileset(tilesetName,tilesetName);

          for (int i = 0; i < amount; i++)
          {
            var skeleton = _world.CreateEntity();
            skeleton.Set(new RenderComponent()
            {
              HasTexture = true,
              TextureName = tilesetName,
              TextureHeight = 48,
              TextureWidth = 48
            });

            PhysicsWorld.Instance.CreateDynamicBody(x, y, 12, 16, skeleton.GetHashCode());

            var animations = new Class.Animation[]
        {
        new (AnimationTypes.Idle,6,0),
        new (AnimationTypes.Walking_Right,6,4),
        new (AnimationTypes.Walking_Left,6,4,true), // Flip option to go left
        new (AnimationTypes.Walking_Up,6,5),
        new (AnimationTypes.Idle_Up,6,3),
        new (AnimationTypes.Walking_Down,6,3),
        new (AnimationTypes.Jumping_Right,6,4,true) // Flip option to go left
        };

            var animationComponent = new AnimationComponent(tilesetName, 48, 48, 0.075f, animations);

            skeleton.Set(animationComponent);
          }
          break;

        case ThaloriaNpc.SkeletonSword:
          break;

        case ThaloriaNpc.Slime:
          break;
      }
    }

    public static void CreatePlayer(float x, float y)
    {
      var player = _world.CreateEntity();

      player.Set(new PlayerComponent());
      player.Set(new RenderComponent()
      {
        HasTexture = true,
        TextureName = ResourceNames.PlayerTileset,
        TextureWidth = 48,
        TextureHeight = 48,
      });

      // Create a more efficient way if centering the body on the sprite
      PhysicsWorld.Instance.CreateDynamicBody(x, y, 12, 16, player.GetHashCode());

      // Base animations
      var animations = new Class.Animation[]
      {
        new (AnimationTypes.Idle,6,0),
        new (AnimationTypes.Walking_Right,6,4),
        new (AnimationTypes.Walking_Left,6,4,true), // Flip option to go left
        new (AnimationTypes.Walking_Up,6,5),
        new (AnimationTypes.Idle_Up,6,3),
        new (AnimationTypes.Walking_Down,6,3),
        new (AnimationTypes.Jumping_Right,6,4,true) // Flip option to go left
      };

      var animationComponent = new AnimationComponent(ResourceNames.PlayerTileset, 48, 48, 0.075f, animations);

      player.Set(animationComponent);
    }
  }
}
