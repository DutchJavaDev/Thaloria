using DefaultEcs.System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Thaloria.Game.ECS.Components;
using DefaultEcs;
using Thaloria.Game.Physics;
using Thaloria.Game.Helpers;
using nkast.Aether.Physics2D.Common;

namespace Thaloria.Game.ECS.Systems
{
  [With(typeof(PlayerComponent))]
  public sealed class InputSystem() : AEntitySetSystem<float>(EcsCreation.Instance)
  {
    private readonly float _speed = 100f;

    protected override void Update(float state, in Entity entity)
    {
      var body = PhysicsWorld.Instance.GetBodyByTag(entity.GetHashCode());
      ref var animationController = ref entity.Get<AnimationComponent>();
      var velocity = _speed * state;

      // Reset to stop moving
      body.LinearVelocity = Vector2.Zero;
      //animationController.SetAnimation(AnimationTypes.Idle);
      
      // DEV
      //if (IsKeyDown(KeyboardKey.L))
      //{
      //  body.FixedRotation = !body.FixedRotation;

      //  if (body.FixedRotation) 
      //  {
      //    body.Rotation = 0;
      //  }
      //}

      if (IsKeyDown(KeyboardKey.A))
      {
        body.LinearVelocity += new Vector2(velocity * -1, 0);
        animationController.SetAnimation(AnimationTypes.Walking_Left);
      }

      if (IsKeyDown(KeyboardKey.D))
      {
        body.LinearVelocity += new Vector2(velocity, 0);
        animationController.SetAnimation(AnimationTypes.Walking_Right);
      }

      if (IsKeyDown(KeyboardKey.W))
      {
        body.LinearVelocity += new Vector2(0, velocity * -1);
        animationController.SetAnimation(AnimationTypes.Walking_Up);
      }

      if (IsKeyDown(KeyboardKey.S))
      {
        body.LinearVelocity += new Vector2(0, velocity);
        animationController.SetAnimation(AnimationTypes.Walking_Down);
      }
    }

  }
}
