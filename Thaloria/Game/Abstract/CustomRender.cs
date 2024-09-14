using Thaloria.Game.Interface;

namespace Thaloria.Game.Abstract
{
  public abstract class CustomRender : ICustomRender
  {
    public bool IsEnabled { get; set; }
    public abstract void PreRender(float state);
    public abstract void Render(float state);
    public abstract void PostRender(float state);
    public void Update(float state)
    {
      PreRender(state);
      Render(state);
      PostRender(state);
    }
    public abstract void Dispose();
  }
}
