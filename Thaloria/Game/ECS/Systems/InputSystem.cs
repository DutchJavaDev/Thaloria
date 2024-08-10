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

      playerPosition.XVelocity = 0;
      playerPosition.YVelocity = 0;

      var velocity = _speed * state;
      var xPosition = playerPosition.X;
      var yPosition = playerPosition.Y;

      if (IsKeyDown(KeyboardKey.A))
      {
        xPosition -= velocity;
        playerPosition.XVelocity = velocity * -1;
      }

      if (IsKeyDown(KeyboardKey.D))
      {
        xPosition += velocity;
        playerPosition.XVelocity = velocity;
      }

      if (IsKeyDown(KeyboardKey.W))
      {
        yPosition -= velocity;
        playerPosition.YVelocity = velocity * -1;
      }

      if (IsKeyDown(KeyboardKey.S))
      {
        yPosition += velocity;
        playerPosition.YVelocity = velocity;
      }

      playerPosition.Position = new(xPosition,yPosition);
    }

  }
}
