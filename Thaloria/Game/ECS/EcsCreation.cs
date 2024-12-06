using DefaultEcs;
using Raylib_cs;
using Thaloria.Game.ECS.Components;
using Thaloria.Game.Helpers;
using Thaloria.Game.Map;
using Thaloria.Game.Map.Tiled;
using Thaloria.Game.Npc;
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

    public static void SetWorldComponent<T>(T component)
    {
      _world.Set(component);
    }

    public static void SpawnNpcs(IEnumerable<TiledCollisionObject> npcs)
    {
      var characterLoader = _world.Get<CharacterLoader>();
      foreach (var spawn in npcs)
      {
        spawn.TryGetIntProperty("npc", out var npcType);
        spawn.TryGetIntProperty("amount", out var amount);
        CreateNpc((ThaloriaNpc)npcType,amount,spawn.Xf,spawn.Yf, characterLoader);
      }
    }

    private static void CreateNpc(ThaloriaNpc npcType, int amount, float x, float y, CharacterLoader characterLoader)
    {
      var npc = NpcInfo.GetNpcInfo(npcType);

      // TODO fix this so it gets called only once....
      ResourceManager.LoadResourceTexture2DTileset(npc.TextureName, npc.TextureName);

      for (var i = 0; i < amount; i++)
      {
        var entity = _world.CreateEntity();

        entity.Set(new RenderComponent
        {
          HasTexture = true,
          TextureName = ResourceNames.CharaterTileSet,
          TextureWidth = npc.FrameWidth,
          TextureHeight = npc.FrameHeight,
        });

        var animation = new AnimationComponent(characterLoader.GetCharacterRectangle(npc.TextureName), npc.FrameWidth, npc.FrameHeight, 0.075f, npc.Animations);

        entity.Set(animation);

        var hitboxWidth = npc.HitBoxWidth;
        var hitboxHeight = npc.HitBoxHeight;

        PhysicsWorld.Instance.CreateDynamicBody(x, y, hitboxWidth, hitboxHeight, entity.GetHashCode());
      }
    }

    public static void CreatePlayer(float x, float y)
    {
      var player = _world.CreateEntity();
      var characterLoader = _world.Get<CharacterLoader>();

      player.Set(new PlayerComponent());
      player.Set(new RenderComponent()
      {
        HasTexture = true,
        TextureName = ResourceNames.CharaterTileSet,
        TextureWidth = 48, // this will always be constant 
        TextureHeight = 48, // this will always be constant 
        //RenderColor = Color.Yellow
      });

      // Create a more efficient way if centering the body on the sprite
      var hitBoxWidth = 13;
      var hitboxHeight = 21;

      PhysicsWorld.Instance.CreateDynamicBody(x,y,hitBoxWidth,hitboxHeight, player.GetHashCode());

      //// Base animations
      var animations = new Class.Animation[]
      {
        new (AnimationTypes.Idle,5,0),
        new (AnimationTypes.Walking_Right,5,4),
        new (AnimationTypes.Walking_Left,5,4,true), // Flip option to go left
        new (AnimationTypes.Walking_Up,5,5),
        new (AnimationTypes.Idle_Up,5,3),
        new (AnimationTypes.Walking_Down,5,3),
        new (AnimationTypes.Jumping_Right,5,4,true) // Flip option to go left
      };

      var animationComponent = new AnimationComponent(characterLoader.GetCharacterRectangle("player"),48, 48, 0.075f, animations);

      player.Set(animationComponent);
    }
  }
}
