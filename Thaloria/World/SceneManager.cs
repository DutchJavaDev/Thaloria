using Thaloria.World.Interface;
using Thaloria.World.Scenes;

namespace Thaloria.World
{
  public enum SceneManagerEnum : byte
  {
    DefaultScene,
    MenuScene,
    GameScene
  }

  public sealed class SceneManager
  {
    private readonly IDictionary<SceneManagerEnum, IScene> Scenes;
    private IScene _currentScene;

    public SceneManager()
    {
      Scenes = new Dictionary<SceneManagerEnum, IScene>();
      SetDefaultScene();
    }

    private void SetDefaultScene()
    {
      var defaultScene = new DefaultScene();
      _currentScene = defaultScene;
      AddScene(defaultScene);
    }

    public void AddScene(IScene scene)
    {
      var reference = scene.SceneReference;
      if (Scenes.TryAdd(reference, scene))
      {
        scene.Init(this);
      }
    }

    public IScene GetScene()
    {
      return _currentScene;
    }

    public async void SwitchToScene(SceneManagerEnum scene)
    {
      // Dispose current scene
      await _currentScene.Dispose().ConfigureAwait(false);

      // Set loading scene
      _currentScene = Scenes[SceneManagerEnum.DefaultScene];

      // Get the scene we are going to switch to and start the loading
      var sceneToSwitchTo = Scenes[scene];

      await sceneToSwitchTo.Load().ConfigureAwait(false);

      // Done loading, now switch to that scene
      _currentScene = sceneToSwitchTo;
    }

    public void DisposeAll()
    {
      foreach (var scene in Scenes.Values)
      {
        scene.Dispose();
      }
    }
  }
}
