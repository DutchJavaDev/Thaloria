using DefaultEcs;
using DefaultEcs.System;
using Thaloria.Game.ECS.Components;
using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using Thaloria.Game.Helpers;

namespace Thaloria.Game.ECS.Systems
{
  [With(typeof(RenderComponent))]
  [With(typeof(PositionComponent))]
  [With(typeof(BodyComponent))]
  [With(typeof(DynamicBodyComponent))]
  public sealed class CollisionSystem(World world, Rectangle[] CollisionObjects) : AEntitySetSystem<float>(world)
  {
    protected override void Update(float state, in Entity entity)
    {
      // Runs for every entity that has the components
      ref PositionComponent positionComponent = ref entity.Get<PositionComponent>();
      ref readonly BodyComponent bodyComponent = ref entity.Get<BodyComponent>();
      ref readonly RenderComponent renderComponent = ref entity.Get<RenderComponent>();

      var dynamicRec = new Rectangle()
      {
        Position = positionComponent.Position,
        Width = bodyComponent.Width,
        Height = bodyComponent.Height
      };

      // Checking all objects....
      var collidingObjects = CollisionObjects.Where(i => CheckCollisionRecs(dynamicRec,i));

      foreach (var collidingObject in collidingObjects)
      {
        var result = CollisionHelper.ResolveCollision(dynamicRec, collidingObject);

        if (Math.Abs(result.MoveX) < Math.Abs(result.MoveY))
        {
          var oldX = positionComponent.X;
          var newX = positionComponent.X + result.MoveX;
          positionComponent.Position = new Vector2(newX, positionComponent.Y);
        }
        else
        {
          var oldY = positionComponent.Y;
          var newY = positionComponent.Y + result.MoveY;
          positionComponent.Position = new Vector2(positionComponent.X, newY);
        }
      }
    }
  }
}
