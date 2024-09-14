using DefaultEcs;
using DefaultEcs.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thaloria.Game.ECS.Components;

namespace Thaloria.Game.ECS.Systems
{
  [With(typeof(AnimationComponent))]
  public sealed class AnimationSystem(World world) : AEntitySetSystem<float>(world)
  {
    protected override void Update(float state, in Entity entity)
    {
      ref var animationComponent = ref entity.Get<AnimationComponent>();

      animationComponent.ElapsedTime += state;

      if (animationComponent.ElapsedTime > animationComponent.UpdateTime)
      {
        animationComponent.ElapsedTime = 0;

        var animation = animationComponent.CurrentAnimation;

        var currentAnimation = animationComponent.Animations.FirstOrDefault(i => i.AnimationName == animation);

        animationComponent.CurrentFrame++;

        if (animationComponent.CurrentFrame > currentAnimation?.FrameCount) 
        {
          animationComponent.CurrentFrame = 0;
        }
      }
    }
  }
}
