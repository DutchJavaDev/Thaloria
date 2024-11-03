using System.Numerics;
using Thaloria.Game.ECS.Class;
using Thaloria.Game.Helpers;

namespace Thaloria.Game.ECS.Components
{
  public struct AnimationComponent(
    string spriteSheet, 
    float frameWidth, 
    float frameHeight, 
    float updateTime,
    Animation[] animations)
  {
    public readonly string SpriteSheetName = spriteSheet;
    public readonly float FrameWidth = frameWidth;
    public readonly float FrameHeight = frameHeight;
    public readonly float UpdateTime = updateTime;
    public readonly Animation[] Animations = animations;

    public AnimationTypes CurrentAnimation = AnimationTypes.Idle;
    public float ElapsedTime = 0;
    public int CurrentFrame = 0;

    public void SetAnimation(AnimationTypes animation) 
    {
      if(CurrentAnimation == animation) return;
      if(!Animations.Any(i => i.AnimationName == animation))
      {
        throw new ArgumentNullException($"This component does not contain the animation {animation}");
      }

      CurrentAnimation = animation;
    }

    public readonly bool IsFlipped()
    {
      var currentAnimation = GetCurrentAnimation();
      return currentAnimation.FlipTexture;
    }

    public readonly Vector2 GetFramePosition() 
    {
      var currentAnimation = GetCurrentAnimation();

      var x = CurrentFrame * FrameWidth;
      var y = currentAnimation.RowStart * FrameHeight;

      return new(x,y);
    }

    private readonly Animation GetCurrentAnimation()
    {
      var cAnimEnum = CurrentAnimation;
      var currentAnimation = Animations.First(i => i.AnimationName == cAnimEnum);
      return currentAnimation;
    }
  }
}
