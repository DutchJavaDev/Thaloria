using Thaloria.World.Interface;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Thaloria.World.Scenes
{
  public sealed class MenuScene : IScene
  {
    public SceneManagerEnum SceneReference => SceneManagerEnum.MenuScene;
    private SceneManager? _sceneManager;

    private int x = 150;
    private int y = 150;
    private int force = 75;

    public void Init(SceneManager sceneManager)
    {
      _sceneManager = sceneManager;
    }
    public async Task Load()
    {
      await Task.Delay(1000);
    }

    public Task Dispose()
    {
      return Task.CompletedTask;
    }

    public void Update()
    {
      if (IsKeyPressed(KeyboardKey.Enter))
      {
        _sceneManager?.SwitchToScene(SceneManagerEnum.GameScene);
      }
    }

    public void Render()
    {
      BeginDrawing();
      DrawText("Menu loaded", x, y, 18, Color.Green);
      EndDrawing();
    }
  }
}
