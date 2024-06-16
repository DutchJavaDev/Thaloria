namespace Thaloria.World.Interface
{
  public interface IScene
  {
    string Name { get; }
    void Init(SceneManager sceneManager);
    Task Load();
    void Update();
    void Render();
    Task Dispose();
  }
}
