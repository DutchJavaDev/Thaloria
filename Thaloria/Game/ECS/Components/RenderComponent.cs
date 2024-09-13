using Raylib_cs;

namespace Thaloria.Game.ECS.Components
{
  public struct RenderComponent
  {
    /// <summary>
    /// Determine if this component has a texture or not, if not it will render a shape
    /// </summary>
    public readonly bool HasTexture { get; init; }

    /// <summary>
    ///  Color for the shape
    /// </summary>
    public Color RenderColor { get; set; }

    public string TextureName { get; init; }

    public int TextureWidth { get; init; }

    public int TextureHeight { get; init; }
  }
}
