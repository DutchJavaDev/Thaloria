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
      ref PositionComponent playerPosition = ref entity.Get<PositionComponent>();

      var velocity = _speed * state;
      var xPosition = playerPosition.X;
      var yPosition = playerPosition.Y;

      if (IsKeyDown(KeyboardKey.A))
      {
        xPosition -= velocity;
      }

      if (IsKeyDown(KeyboardKey.D))
      {
        xPosition += velocity;
      }

      if (IsKeyDown(KeyboardKey.W))
      {
        yPosition -= velocity;
      }

      if (IsKeyDown(KeyboardKey.S))
      {
        yPosition += velocity;
      }

      playerPosition.Position = new(xPosition,yPosition);
    }

  }
}
