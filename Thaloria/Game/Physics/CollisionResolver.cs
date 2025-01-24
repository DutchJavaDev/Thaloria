using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using Thaloria.Game.ECS.Class;
using Thaloria.Game.Helpers;
using Thaloria.Game.Npc;

namespace Thaloria.Game.Physics
{
  public static class CollisionResolver
  {
    private static Dictionary<List<int>, OnCollisionEventHandler> ObjectCollisionHandlers = [];
    private static Dictionary<int, OnCollisionEventHandler> NpcCollisionHandlers = [];

    static CollisionResolver()
    {
      // Objects
      // doors
      List<int> doorIds = [591, 595];
      ObjectCollisionHandlers.Add(doorIds, DoorCollisionResolver);

      // Npc's
      // player
      NpcCollisionHandlers.Add(0, PlayerCollisionHandler);

      //skeleton
      //NpcCollisionHandlers.Add((int)ThaloriaNpc.Skeleton, SkeletonCollisionHandler);

      PhysicsWorld.Instance.World.ContactManager.PostSolve += PostSolveCollision;
    }

    private static void PostSolveCollision(Contact contact, ContactVelocityConstraint impulse)
    {
      // Prevent force applied to bodies
      if (contact.FixtureA != null && contact.FixtureB != null) 
      {
        contact.FixtureA.Body.LinearVelocity = Vector2.Zero;
        contact.FixtureB.Body.LinearVelocity = Vector2.Zero;

        contact.FixtureA.Body.LinearDamping = 0.0f;
        contact.FixtureB.Body.LinearDamping = 0.0f;

        contact.FixtureA.Body.AngularDamping = 0.0f;
        contact.FixtureB.Body.AngularDamping = 0.0f;
      }
    }

    public static OnCollisionEventHandler GetObjectOnCollisionEventHandler(int id)
    {
      return ObjectCollisionHandlers.Where(i => i.Key.Contains(id)).Select(i => i.Value).FirstOrDefault();
    }

    public static OnCollisionEventHandler GetNpcOnCollisionEventHandler(int npcId)
    {
      return NpcCollisionHandlers.Where(i => i.Key == npcId).Select(i => i.Value).FirstOrDefault();
    }

    private static bool PlayerCollisionHandler(Fixture sender, Fixture other, Contact contact)
    {
      if (other.Body.Tag != null && other.Body.Tag is TagObject otherTag)
      {
        if (otherTag.Name == "Skeleton")
        {

        }
      }

      return true;
    }

    private static bool SkeletonCollisionHandler(Fixture sender, Fixture other, Contact contact)
    {
      if (other.Body.Tag != null && other.Body.Tag is TagObject otherTag)
      {
        if (otherTag.Name == "player")
        {

        }
      }

      return true;
    }

    private static bool DoorCollisionResolver(Fixture sender, Fixture other, Contact contact)
    {
      if (other.Body.Tag != null && other.Body.Tag is TagObject otherTag)
      {
        if (otherTag.Name == "player")
        {
          var selfTag = (TagObject)sender.Body.Tag;

          if (selfTag.TileTag == Guid.Empty) return true;

          ThaloriaStatic.FixedAnimations[selfTag.TileTag] = true;

          // tele port to a binded door?
        }
      }

      return true;
    }

  }
}
