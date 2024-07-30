using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using Thaloria.Game.ECS.Components;

namespace Thaloria.Game.Helpers
{
  public enum CollisionDirection
  {
    Null,
    Left,
    Right,
    Up,
    Down,
  }

  public readonly struct CollisionResult(CollisionDirection collisionDirection, float moveX, float moveY)
  {
    public readonly CollisionDirection Direction = collisionDirection;
    public readonly float MoveX = moveX;
    public readonly float MoveY = moveY;
  }

  public static class CollisionHelper
  {
    /// <summary>
    /// TODO FIX this :), works for now I guess
    /// </summary>
    /// <param name="dynamicBody"></param>
    /// <param name="staticBody"></param>
    /// <returns></returns>

    public static CollisionResult ResolveCollision(Rectangle dynamicBody, Rectangle staticBody)
    {
      float moveX;
      CollisionDirection collisionDirection = CollisionDirection.Null;
      if (dynamicBody.X < staticBody.X)
      {
        moveX = staticBody.X - (dynamicBody.X + dynamicBody.Width);
      }
      else
      {
        moveX = (staticBody.X + staticBody.Width) - dynamicBody.X;
      }

      float moveY;
      if (dynamicBody.Y < staticBody.Y)
      {
        moveY = staticBody.Y - (dynamicBody.Y + dynamicBody.Height);
      }
      else
      {
        moveY = (staticBody.Y + staticBody.Height) - dynamicBody.Y;
      }

      // Get the direction
      if (Math.Abs(moveX) < Math.Abs(moveY))
      {
        collisionDirection = moveX > 0 ? CollisionDirection.Left : CollisionDirection.Right;
      }
      else
      {
        collisionDirection = moveY > 0 ? CollisionDirection.Up : CollisionDirection.Down;
      }

      return new(collisionDirection, moveX, moveY);
    }
  }
}
