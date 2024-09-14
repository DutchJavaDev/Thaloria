using DefaultEcs.System;

namespace Thaloria.Game.Interface
{
  public interface ICustomRender : ISystem<float>
  {
    void PreRender(float state);
    void Render(float state);
    void PostRender(float state);
  }
}
