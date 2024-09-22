using Thaloria.Game.Interface;
using Thaloria.Game.Map;
using DefaultEcs;
using DefaultEcs.System;
using static Raylib_cs.Raylib;
using Raylib_cs;
using Thaloria.Game.ECS.Systems;
using Thaloria.Game.ECS.Components;
using Thaloria.Loaders;
using System.Numerics;
using Thaloria.Game.Helpers;
using Thaloria.Game.Physics;
using Thaloria.Game.ECS.Class;
using Thaloria.Game.ECS;

namespace Thaloria.Game.Scenes
{
  public sealed class GameScene : IScene
  {
    public SceneManagerEnum SceneReference => SceneManagerEnum.GameScene;
    private SceneManager? _sceneManager;
    private readonly MapLoader Map = new("Home");
    private readonly int gameScreenWidth = 640;
    private readonly int gameScreenHeight = 480;

    // ECS
    private ISystem<float>? sequentialUpdateSystems;
    private ISystem<float>? sequentialRenderSystems;

    // Rendering
    private RenderTexture2D RenderTexture2D;

    public Task DisposeAsync()
    {
      //PhysicsWorld.Instance.Dispose();
      sequentialUpdateSystems?.Dispose();
      sequentialRenderSystems?.Dispose();
      //EcsCreation.Instance.Dispose();
      UnloadRenderTexture(RenderTexture2D);
      return Task.CompletedTask;
    }

    public void Init(SceneManager sceneManager)
    {
      _sceneManager = sceneManager;
    }

    public async Task LoadAsync()
    {
      await Map.LoadMap();

      ResourceManager.LoadResourceTexture2DTileset(ResourceNames.PlayerTileset, "player.png");
      //ResourceManager.LoadResourceTexture2DTileset(ResourceNames.TileTexture, Map.ImageName);

      RenderTexture2D = LoadRenderTexture(gameScreenWidth, gameScreenHeight);

      EcsCreation.CreatePlayer();

      EcsCreation.SetWorldComponent(new CameraComponent(new Camera2D {
        Offset = new()
        {
          X = gameScreenWidth / 2,
          Y = gameScreenHeight / 2
        },
        Rotation = 0,
        Zoom = 2f,
      }));

      // Update systems
      sequentialUpdateSystems = new SequentialSystem<float>(
        new InputSystem(),
        PhysicsWorld.Instance,
        new CameraSystem(EcsCreation.Instance, Map)
       );

      // Rendering systems
      sequentialRenderSystems = new SequentialSystem<float>(
        new AnimationSystem(EcsCreation.Instance),
        new TileRenderingSystem(EcsCreation.Instance, Map.GroundTileData, Map),
        new RenderPipelineSystem(EcsCreation.Instance, Map.TopTileData),
        // Debugging
        new CollisionBodyRenderingSystem(EcsCreation.Instance)
       );
    }

    public void Update()
    {
      if (IsKeyPressed(KeyboardKey.Space))
      {
        sequentialUpdateSystems.IsEnabled = false;
        sequentialRenderSystems.IsEnabled = false;
        _sceneManager?.SwitchToScene(SceneManagerEnum.MenuScene);
        return;
      }
      sequentialUpdateSystems?.Update(GetFrameTime());
    }

    public void Render()
    {
      var screenWidth = (float)GetScreenWidth();
      var screenHeight = (float)GetScreenHeight();

      var scaleX = screenWidth / gameScreenWidth;
      var scaleY = screenHeight / gameScreenHeight;

      BeginTextureMode(RenderTexture2D);
      ClearBackground(Color.Black);
      sequentialRenderSystems?.Update(GetFrameTime());
      EndTextureMode();

      // Render game to screen
      // Draw texture
      BeginDrawing();
      ClearBackground(Color.Black);
      var sourceRec = new Rectangle(0f, 0f, RenderTexture2D.Texture.Width, -RenderTexture2D.Texture.Height);
      var destinationRec = new Rectangle(
        (screenWidth - (gameScreenWidth * scaleX)) * 0.5f,
        (screenHeight - (gameScreenHeight * scaleY)) * 0.5f,
        gameScreenWidth * scaleX,
        gameScreenHeight * scaleY);

      DrawTexturePro(RenderTexture2D.Texture, sourceRec, destinationRec, Vector2.Zero, 0f, Color.White);
      DrawFPS(15, 10);
      EndDrawing();
    }
  }
}
