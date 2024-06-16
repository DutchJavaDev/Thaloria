using Thaloria.World.Scenes;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Thaloria.Loaders;

namespace Thaloria.World
{
  public sealed class ThaloriaGame
  {
    private readonly SceneManager _sceneManager;
    public ThaloriaGame()
    {
      _sceneManager = new SceneManager();
      _sceneManager.AddScene(new DefaultScene());
      _sceneManager.AddScene(new MenuScene());
      _sceneManager.AddScene(new WorldScene());
    }

    public void Init()
    {
      //_sceneManager.SwitchToScene(nameof(MenuScene));
    }

    // Main game loop
    // Updating and rendering
    public void Run()
    {
      var scene = _sceneManager.GetScene();

      // Update
      scene.Update();

      // Render 
      ClearBackground(Color.Black);
      scene.Render();
    }

    public void Dispose()
    {
      _sceneManager.DisposeAll();
    }
  }
}
