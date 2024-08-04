namespace Thaloria.Game.Interface
{
  public interface IScene
  {
    SceneManagerEnum SceneReference { get; }
    void Init(SceneManager sceneManager);
    Task LoadAsync();
    void Update();
    void Render();
    Task DisposeAsync();
  }
}
