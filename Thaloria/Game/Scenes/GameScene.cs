using Thaloria.Game.Interface;
using Thaloria.Game.Map;
using DefaultEcs;
using DefaultEcs.System;
using static Raylib_cs.Raylib;
using Raylib_cs;
using Thaloria.Game.ECS.Systems;
using Thaloria.Game.ECS.Components;
using System.Numerics;

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
    private World? _world;
    private ISystem<float>? sequentialUpdateSystems;
    private ISystem<float>? sequentialRenderSystems;

    // Rendering
    private Texture2D TileTexture;
    private RenderTexture2D RenderTexture2D;

    public Task Dispose()
    {
      sequentialUpdateSystems.Dispose();
      sequentialRenderSystems.Dispose();
      _world?.Dispose();
      UnloadTexture(TileTexture);
      UnloadRenderTexture(RenderTexture2D);
      return Task.CompletedTask;
    }

    public void Init(SceneManager sceneManager)
    {
      _sceneManager = sceneManager;
    }

    public async Task Load()
    {
      await Map.LoadMap();

      TileTexture = LoadTexture($"Resources\\Tilesets\\{Map.ImageName}");
      SetTextureFilter(TileTexture, TextureFilter.Anisotropic16X);

      RenderTexture2D = LoadRenderTexture(gameScreenWidth, gameScreenHeight);
      SetTextureFilter(RenderTexture2D.Texture, TextureFilter.Anisotropic16X);

      _world = new World();

      // Hmmm ?
      _world.Set(TileTexture);

      _world.SetMaxCapacity<CameraComponent>(1);
      _world.SetMaxCapacity<PlayerComponent>(1);

      // Temp
      _world.CreateEntity().Set(new PlayerComponent(new(0,0,10,10)));

      _world.Set(new CameraComponent(new Camera2D {
        Offset = new()
        {
          X = gameScreenWidth / 2,
          Y = gameScreenHeight / 2
        },
        Rotation = 0,
        Zoom = 2f,
      }));

      
      // Let the map load in the entities

      // Update systems
      sequentialUpdateSystems = new SequentialSystem<float>(
        new InputSystem(_world),
        new CameraSystem(_world, Map)
       );

      // Rendering systems
      sequentialRenderSystems = new SequentialSystem<float>(
        new GroundRenderingSystem(_world, Map),
        new TopRenderingSystem(_world, Map)
       );
    }

    public void Update()
    {
      if (IsKeyPressed(KeyboardKey.Space))
      {
        sequentialUpdateSystems.IsEnabled = false;
        sequentialRenderSystems.IsEnabled = false;
        _sceneManager.SwitchToScene(SceneManagerEnum.MenuScene);
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
      var sourceRec = new Rectangle(0f, 0f, (float)RenderTexture2D.Texture.Width, (float)-RenderTexture2D.Texture.Height);
      var destinationRec = new Rectangle(
        (screenWidth - ((float)gameScreenWidth * scaleX)) * 0.5f,
        (screenHeight - ((float)gameScreenHeight * scaleY)) * 0.5f,
        (float)gameScreenWidth * scaleX,
        (float)gameScreenHeight * scaleY);

      DrawTexturePro(RenderTexture2D.Texture, sourceRec, destinationRec, Vector2.Zero, 0f, Color.White);
      DrawFPS(15, 10);
      EndDrawing();
    }
  }
}
