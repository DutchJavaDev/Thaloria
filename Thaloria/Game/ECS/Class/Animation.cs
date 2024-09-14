using Thaloria.Game.Helpers;

namespace Thaloria.Game.ECS.Class
{
  public sealed class Animation(AnimationTypes animationName, int frameCount, int rowStart, bool flipTexture = false)
  {
    public readonly AnimationTypes AnimationName = animationName;
    public readonly int FrameCount = frameCount;
    public readonly int RowStart = rowStart;
    public readonly bool FlipTexture = flipTexture;
  }
}
