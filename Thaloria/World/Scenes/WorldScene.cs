using Raylib_cs;
using static Raylib_cs.Raylib;
using Thaloria.World.Interface;

namespace Thaloria.World.Scenes
{
  public sealed class WorldScene : IScene
  {
    public string Name => nameof(WorldScene);
    private SceneManager? _sceneManager;

    public Task Dispose()
    {
      return Task.CompletedTask;
    }

    public void Init(SceneManager sceneManager)
    {
      _sceneManager = sceneManager;
    }

    public Task Load()
    {
      return Task.CompletedTask;
    }

    public void Render()
    {
      BeginDrawing();
      DrawText("In game", 250, 250, 18, Color.Red);
      EndDrawing();
    }

    public void Update()
    {
      if (IsKeyPressed(KeyboardKey.Enter))
      {
        _sceneManager?.SwitchToScene(nameof(MenuScene));
      }
    }
  }
}
