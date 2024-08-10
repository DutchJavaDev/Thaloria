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
      var collidingObjects = CollisionObjects.Where(i => CheckCollisionRecs(dynamicRec,i)).ToList();

      foreach (var collidingObject in collidingObjects)
      {
        // Moving right
        if (positionComponent.XVelocity > 0)
        {
          // check from center
          if (dynamicRec.X + dynamicRec.Width / 2 < collidingObject.X)
          {
            var newX = collidingObject.X - dynamicRec.Width;
            positionComponent.Position = new(newX, positionComponent.Y);
            continue;
          }
        }

        // Moving left
        if (positionComponent.XVelocity < 0)
        { 
          // check from center
          if (collidingObject.X + collidingObject.Width / 2 < dynamicRec.X)
          {
            var newX = collidingObject.X + collidingObject.Width;
            positionComponent.Position = new(newX, positionComponent.Y);
            continue;
          }
        }

        // Moving up
        if (positionComponent.YVelocity < 0)
        {
          // check from center
          if(dynamicRec.Y > collidingObject.Y + collidingObject.Height / 2)
          {
            var newY = collidingObject.Y + collidingObject.Height;
            positionComponent.Position = new(positionComponent.X, newY);
            continue;
          }
        }

        // Moving down
        if (positionComponent.YVelocity > 0) 
        {
          // check from center
          if(dynamicRec.Y + dynamicRec.Height / 2 < collidingObject.Y)
          {
            var newY = collidingObject.Y - dynamicRec.Height;
            positionComponent.Position = new(positionComponent.X, newY);
            continue;
          }
        }
      }
    }
  }
}
