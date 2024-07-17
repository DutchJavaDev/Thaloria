namespace Thaloria.Game.Interface
{
  public interface IScene
  {
    SceneManagerEnum SceneReference { get; }
    void Init(SceneManager sceneManager);
    Task Load();
    void Update();
    void Render();
    Task Dispose();
  }
}
