using DefaultEcs;
using DefaultEcs.System;
using Thaloria.Game.ECS.Components;
using Thaloria.Game.Helpers;
using Thaloria.Game.Map;
using Thaloria.Game.Physics;

namespace Thaloria.Game.ECS.Systems
{
  [With(typeof(PlayerComponent))]
  public class CameraSystem(World world, MapLoader Map) : AEntitySetSystem<float>(world)
  {
    protected override void Update(float state, in Entity entity)
    {
      ref CameraComponent cameraComponent = ref World.Get<CameraComponent>();

      var body = PhysicsWorld.Instance.GetBodyByEntityTag(entity.GetHashCode());

      // Future me add feature that whatever you click becomes the target?

      cameraComponent.Camera2D.Target = new System.Numerics.Vector2(body.Position.X,body.Position.Y);

      // Clamp camera to 0 when going left
      if (cameraComponent.Camera2D.Target.X - cameraComponent.Camera2D.Offset.X / 2 < 0)
      {
        cameraComponent.Camera2D.Target.X = cameraComponent.Camera2D.Offset.X / 2;
      }

      // Clamp camera to max width when going right
      if (cameraComponent.Camera2D.Target.X + cameraComponent.Camera2D.Offset.X / 2 > Map.MapWidth)
      {
        cameraComponent.Camera2D.Target.X = Map.MapWidth - cameraComponent.Camera2D.Offset.X / 2;
      }

      // Clamp camera to 0 when going up
      if (cameraComponent.Camera2D.Target.Y - cameraComponent.Camera2D.Offset.Y / 2 < 0)
      {
        cameraComponent.Camera2D.Target.Y = cameraComponent.Camera2D.Offset.Y / 2;
      }

      // Clamp camera to max height when going down
      if (cameraComponent.Camera2D.Target.Y + cameraComponent.Camera2D.Offset.Y / 2 > Map.MapHeight)
      {
        cameraComponent.Camera2D.Target.Y = Map.MapHeight - cameraComponent.Camera2D.Offset.Y / 2;
      }

      // Update the view for rendering
      cameraComponent.CameraView.X = cameraComponent.Camera2D.Target.X - cameraComponent.Camera2D.Offset.X / 2;
      cameraComponent.CameraView.Y = cameraComponent.Camera2D.Target.Y - cameraComponent.Camera2D.Offset.Y / 2;
      cameraComponent.CameraView.Width = cameraComponent.Camera2D.Offset.X;
      cameraComponent.CameraView.Height = cameraComponent.Camera2D.Offset.Y;
    }
  }
}
