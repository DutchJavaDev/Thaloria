using DefaultEcs;
using DefaultEcs.System;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Thaloria.Game.ECS.Components;
using Thaloria.Game.Map;

namespace Thaloria.Game.ECS.Systems
{
  public sealed class CollisionBodyRenderingSystem(World world, MapLoader mapLoader) : ISystem<float>
  {
    public bool IsEnabled { get; set; }

    private readonly List<Rectangle> Bodies = mapLoader.CollisionBodies;
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
        foreach (var body in Bodies)
        {
          if (CheckCollisionRecs(cameraComponent.CameraView, body))
          {
            DrawRectangleLinesEx(body, Thickness, Color);
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
