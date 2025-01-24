using DefaultEcs;
using DefaultEcs.System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Thaloria.Game.ECS.Components;
using Thaloria.Game.Physics;
using nkast.Aether.Physics2D.Collision.Shapes;
using System.Numerics;

namespace Thaloria.Game.ECS.Systems
{
  public sealed class CollisionBodyRenderingSystem(World world) : ISystem<float>
  {
    public bool IsEnabled { get; set; }

    private readonly float Thickness = 0.5f;
    private Color Color = Color.Yellow;

    public void Update(float state)
    {
      if (IsKeyPressed(KeyboardKey.Q))
      {
        IsEnabled = !IsEnabled;
      }

      if (IsEnabled)
      {
        ref CameraComponent cameraComponent = ref world.Get<CameraComponent>();

        BeginMode2D(cameraComponent.Camera2D);
        foreach (var body in PhysicsWorld.Instance.GetBodies())
        {
          var fixtures = body.FixtureList;
          
          foreach (var fixture in fixtures) 
          {
            // TODO refactor
            switch (fixture.Shape.ShapeType)
            {
              case ShapeType.Circle: break;
              case ShapeType.Polygon:
                {
                  var verticies = ((PolygonShape)fixture.Shape).Vertices;

                  for (int i = 0; i < verticies.Count; i++)
                  {
                    var startPoint = body.GetWorldPoint(verticies[i]);
                    var endPoint = body.GetWorldPoint(verticies[(i + 1) % verticies.Count]);

                    Vector2 start = new(startPoint.X, startPoint.Y);
                    Vector2 end = new(endPoint.X, endPoint.Y); // Connect last vertex to the first

                    // Draw line segment between consecutive vertices with specified thickness
                    DrawLineEx(start, end, Thickness, Color);
                  }
                }
                break;
              case ShapeType.Chain:
                {
                  var verticies = ((ChainShape)fixture.Shape).Vertices;

                  for (int i = 0; i < verticies.Count; i++)
                  {
                    var startPoint = body.GetWorldPoint(verticies[i]);
                    var endPoint = body.GetWorldPoint(verticies[(i + 1) % verticies.Count]);

                    Vector2 start = new(startPoint.X, startPoint.Y);
                    Vector2 end = new(endPoint.X, endPoint.Y); // Connect last vertex to the first

                    // Draw line segment between consecutive vertices with specified thickness
                    DrawLineEx(start, end, Thickness, Color);
                  }
                }
                break;
            }
          }
        }
        EndMode2D();
      }
    }

    public void Dispose()
    {
      return;
    }
  }
}
