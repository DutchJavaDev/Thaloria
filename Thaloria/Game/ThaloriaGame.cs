using Thaloria.Game.Scenes;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Thaloria.Loaders;
using Thaloria.Game.Helpers;

namespace Thaloria.Game
{
  public sealed class ThaloriaGame
  {
    private readonly SceneManager _sceneManager;
    public ThaloriaGame()
    {
      _sceneManager = new SceneManager();
    }

    public void Init()
    {
      _sceneManager.AddScene(new MenuScene());
      _sceneManager.AddScene(new GameScene());
      _sceneManager.SwitchToScene(SceneManagerEnum.MenuScene);
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

    public async Task Dispose()
    {
      await _sceneManager.DisposeAllAsync();
      ResourceManager.Dispose();
    }
  }
}
