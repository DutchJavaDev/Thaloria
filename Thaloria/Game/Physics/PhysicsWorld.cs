using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Common;
using DefaultEcs.System;
using Thaloria.Game.ECS.Class;

namespace Thaloria.Game.Physics
{
  public sealed class PhysicsWorld : ISystem<float>
  {
    public static PhysicsWorld Instance;
    public readonly World World;

    static PhysicsWorld() 
    {
      Instance = new PhysicsWorld();
    }

    PhysicsWorld()
    {
      World = new World(new Vector2(0, 0));

      World.ContactManager.OnBroadphaseCollision += (e, f) =>
      {

      };

      World.ContactManager.PostSolve += (e, g) => 
      {
        
      };
    }

    public bool IsEnabled { get; set; }

    public void CreateChainBody(float x, float y, Vector2[] _verticies, TagObject tag = default, OnCollisionEventHandler handler = null)
    {
      var verticies = new Vertices(_verticies);

      var body = World.CreateChainShape(verticies,new Vector2(x,y));
      body.Tag = tag;
      body.FixedRotation = true;
      if (handler != null) 
      {
        body.OnCollision += handler;
      }
    }

    //public void CreatePolygonShape(float x, float y, Vector2[] _verticies)
    //{
    //  var verticies = new Vertices(_verticies);

    //  var body = World.CreatePolygon(verticies, 1f, new Vector2(x, y));

    //  body.FixedRotation = true;
    //}

    public void CreateDynamicBody(float x, float y, float width, float height, TagObject tag, OnCollisionEventHandler handler = null)
    {
      var body = World.CreateRectangle(width, height, 1f, new Vector2(x, y), 0, BodyType.Dynamic);
      body.FixedRotation = true;
      body.Tag = tag;
       
      if (handler != null) 
      {
        body.OnCollision += handler;
      }
    }

    //public void CreateStaticBody(float x, float y, float width, float height, int tag = -1, OnCollisionEventHandler handler = null)
    //{
    //  var body = World.CreateRectangle(width, height, 1f, new Vector2(x, y),0);
    //  body.Tag = tag;

    //  if (handler != null)
    //  {
    //    body.OnCollision += handler;
    //  }
    //}

    //public Body? GetBodyByTag(int tag)
    //{
    //  return World.BodyList.FirstOrDefault(i => i.Tag != null && i.Tag.Equals(tag));
    //}

    public Body? GetBodyByEntityTag(int tag)
    {
      return World.BodyList.Where(i => i.Tag != null).FirstOrDefault(i => ((TagObject)i.Tag).EntityTag == tag);
    }

    public Body? GetBodyByTileGuid(Guid guid)
    {
      return World.BodyList.Where(i => i.Tag != null).FirstOrDefault(i => ((TagObject)i.Tag).TileTag == guid);
    }

    public BodyCollection GetBodies() => World.BodyList;

    public void Update(float state)
    {
      // Not all objects need to awake
      // Check using the camera en set awake to false or true
      var stepTime = 1f / state;
      World.Step(stepTime);
    }

    public void Dispose()
    {
      World.Clear();
    }
  }
}
