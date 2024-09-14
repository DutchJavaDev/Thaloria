using DefaultEcs;
using Thaloria.Game.ECS.Class;
using Thaloria.Game.ECS.Components;
using Thaloria.Game.Helpers;
using Thaloria.Game.Physics;

namespace Thaloria.Game.ECS
{
  public static class EcsCreation
  {
    private static World _world;

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

    public static void CreatePlayer()
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
      PhysicsWorld.Instance.CreateDynamicBody(150, 150, 12, 16, player.GetHashCode());

      // Base animations
      var animations = new Animation[]
      {
        new (AnimationTypes.Idle,6,0),
        new (AnimationTypes.Walking_Right,6,4), // Flip option to go left
        new (AnimationTypes.Walking_Left,6,4,true),
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
