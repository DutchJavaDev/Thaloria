using System.Numerics;

namespace Thaloria.Game.ECS.Components
{
  public struct PositionComponent
  {
    public Vector2 Position { get; set; }
    public readonly float X => Position.X;
    public readonly float Y => Position.Y;
    public float XVelocity { get; set; }
    public float YVelocity { get; set; }
  }
}
