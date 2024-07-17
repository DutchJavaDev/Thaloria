using DefaultEcs.System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Thaloria.Game.ECS.Components;
using DefaultEcs;

namespace Thaloria.Game.ECS.Systems
{
  [With(typeof(PlayerComponent))]
  public sealed class InputSystem(World world) : AEntitySetSystem<float>(world)
  {
    private readonly float _speed = 75f;

    protected override void Update(float state, in Entity entity)
    {
      ref PlayerComponent player = ref entity.Get<PlayerComponent>();

      var velocity = _speed * state;

      if (IsKeyDown(KeyboardKey.A))
      {
        player.Body.X -= velocity;
      }

      if (IsKeyDown(KeyboardKey.D))
      {
        player.Body.X += velocity;
      }

      if (IsKeyDown(KeyboardKey.W))
      {
        player.Body.Y -= velocity;
      }

      if (IsKeyDown(KeyboardKey.S))
      {
        player.Body.Y += velocity;
      }
    }

  }
}
