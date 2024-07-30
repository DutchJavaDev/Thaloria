using Thaloria.Game.Interface;
using Thaloria.Game.Scenes;

namespace Thaloria.Game
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
    private IScene? _currentScene;

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
      if (_currentScene == null)
        throw new ArgumentNullException("No current scene");

      return _currentScene;
    }

    public async void SwitchToScene(SceneManagerEnum scene)
    {
      // Dispose current scene
      await GetScene().DisposeAsync()
        .ConfigureAwait(false);

      // Set loading scene
      _currentScene = Scenes[SceneManagerEnum.DefaultScene];

      // Get the scene we are going to switch to
      var sceneToSwitchTo = Scenes[scene];

      // Start loading the scene to switch to
      await sceneToSwitchTo.LoadAsync()
        .ConfigureAwait(false);

      // Done loading, now switch to that scene
      _currentScene = sceneToSwitchTo;
    }

    public async Task DisposeAllAsync()
    {
      foreach (var scene in Scenes.Values)
      {
        await scene.DisposeAsync();
      }
    }
  }
}
