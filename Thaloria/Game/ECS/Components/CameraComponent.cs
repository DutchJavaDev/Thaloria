using Raylib_cs;

namespace Thaloria.Game.ECS.Components
{
  public struct CameraComponent(Camera2D initialCamera)
  {
    public Camera2D Camera2D = initialCamera;
    public Rectangle CameraView = new();
  }
}
