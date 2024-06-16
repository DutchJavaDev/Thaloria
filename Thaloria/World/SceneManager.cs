using Thaloria.World.Interface;
using Thaloria.World.Scenes;

namespace Thaloria.World
{

  public sealed class SceneManager
  {
    private int Index = 0;
    private readonly int MaxScenes = 4;
    private readonly IScene[] Scenes;
    private IScene _currentScene;

    public SceneManager()
    {
      Scenes = new IScene[MaxScenes];
      var def = new DefaultScene();

      _currentScene = def;
      AddScene(def);
    }

    public void AddScene(IScene scene)
    {
      if (Index > Scenes.Length) 
      {
        throw new IndexOutOfRangeException($"Adding to much scenes, Increase {nameof(MaxScenes)}");
      }

      Scenes[Index] = scene;
      Scenes[Index].Init(this);

      Index++;
    }

    public IScene GetScene()
    {
      return _currentScene;
    }

    public async void SwitchToScene(string name)
    {
      await _currentScene.Dispose().ConfigureAwait(false);

      // Default scene
      _currentScene = Scenes[0];

      var sceneToSwitch = Scenes.Where(i => i.Name == name).First();

      await sceneToSwitch.Load().ConfigureAwait(false);

      _currentScene = sceneToSwitch;
    }

    public void DisposeAll()
    {
      foreach (var scene in Scenes)
      {
        scene.Dispose();
      }
    }
  }
}
