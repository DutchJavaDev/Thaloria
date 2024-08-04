namespace Thaloria.Game.ECS.Components
{
  public readonly struct BodyComponent(float Width, float Height)
  {
    public readonly float Width = Width; 
    public readonly float Height = Height;
  }
}
