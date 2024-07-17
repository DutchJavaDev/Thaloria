using Raylib_cs;

namespace Thaloria.Game.ECS.Components
{
  public struct PlayerComponent(Rectangle initialBody)
  {
    public Rectangle Body = initialBody;

    public readonly Color Color = Color.White;
  }
}
