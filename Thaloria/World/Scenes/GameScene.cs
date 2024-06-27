using Raylib_cs;
using static Raylib_cs.Raylib;
using Thaloria.World.Interface;
using Thaloria.World.Map;
using System.Numerics;

namespace Thaloria.World.Scenes
{
  public sealed class GameScene : IScene
  {
    public SceneManagerEnum SceneReference => SceneManagerEnum.GameScene;
    private SceneManager? _sceneManager;
    private readonly MapLoader Map = new("map");

    //Rendering / scaling
    private readonly int gameScreenWidth = 640;
    private readonly int gameScreenHeight = 480;
    private Texture2D tileTexture;
    private RenderTexture2D renderTarget;
    private Camera2D camera;
    private Rectangle CameraView = new();

    // Util
    private Rectangle Player = new()
    {
      X = 0,
      Y = 0,
      Width = 10,
      Height = 10
    };
    private readonly float Speed = 250f;

    public Task Dispose()
    {
      UnloadTexture(tileTexture);
      UnloadRenderTexture(renderTarget);
      return Task.CompletedTask;
    }

    public void Init(SceneManager sceneManager)
    {
      _sceneManager = sceneManager;
    }

    public async Task Load()
    {
      await Map.LoadMap();
      tileTexture = LoadTexture($"Resources\\{Map.ImageName}");
      renderTarget = LoadRenderTexture(gameScreenWidth, gameScreenHeight);
      SetTextureFilter(renderTarget.Texture, TextureFilter.Bilinear);

      camera = new()
      {
        Target = Player.Position,

        Offset = new()
        {
          X = gameScreenWidth / 2,
          Y = gameScreenHeight / 2
        },
        Rotation = 0f,
        Zoom = 2.0f
      };
    }

    public void Update()
    {
      if (IsKeyPressed(KeyboardKey.Enter))
      {
        _sceneManager?.SwitchToScene(SceneManagerEnum.MenuScene);
      }

      var force = GetFrameTime() * Speed;

      if (IsKeyDown(KeyboardKey.A))
      {
        Player.X -= force;
      }

      if (IsKeyDown(KeyboardKey.D))
      {
        Player.X += force;
      }

      if (IsKeyDown(KeyboardKey.W))
      {
        Player.Y -= force;
      }

      if (IsKeyDown(KeyboardKey.S))
      {
        Player.Y += force;
      }

      camera.Target = Player.Position;

      // Clamp camera to 0 when going left
      if (camera.Target.X - camera.Offset.X / 2 < 0)
      {
        camera.Target.X = camera.Offset.X / 2;
      }

      // Clamp camera to max width when going right
      if (camera.Target.X + camera.Offset.X / 2 > Map.MapWidth)
      {
        camera.Target.X = Map.MapWidth - camera.Offset.X / 2;
      }

      // Clamp camera to 0 when going up
      if (camera.Target.Y - camera.Offset.Y / 2 < 0)
      {
        camera.Target.Y = camera.Offset.Y / 2;
      }

      // Clamp camera to max height when going down
      if (camera.Target.Y + camera.Offset.Y / 2 > Map.MapHeight)
      {
        camera.Target.Y = Map.MapHeight - camera.Offset.Y / 2;
      }

      CameraView.X = camera.Target.X - camera.Offset.X / 2;
      CameraView.Y = camera.Target.Y - camera.Offset.Y / 2;
      CameraView.Width = camera.Offset.X;
      CameraView.Height = camera.Offset.Y;
    }

    public void Render()
    {
      var screenWidth = (float)GetScreenWidth();
      var screenHeight = (float)GetScreenHeight();

      var scaleX = screenWidth / gameScreenWidth;
      var scaleY = screenHeight / gameScreenHeight;

      // Draw to texture
      BeginTextureMode(renderTarget);
      ClearBackground(Color.Black);
      BeginMode2D(camera);
      var drawCount = 0;
      foreach (var tile in Map.TileData)
      {
        var x = tile.RenderPosition.X;
        var y = tile.RenderPosition.Y;

        if (CheckCollisionRecs(CameraView, new Rectangle(x, y, Map.TileWidth, Map.TileHeight)))
        {
          DrawTextureRec(tileTexture, tile.TexturePosition, tile.RenderPosition, Color.White);

          if (tile.HasCollisionBodys)
          {
            for (var i = 0; i < tile.CollisionBodys.Length; i++)
            {
              var body = tile.CollisionBodys[i];
              DrawRectangleLinesEx(body, 0.5f, Color.Red);
            }
          }
          drawCount++;
        }
      }
      DrawRectangleRec(Player, Color.Green);
      EndMode2D();
      EndTextureMode();

      // Draw texture
      BeginDrawing();
      ClearBackground(Color.Black);
      var sourceRec = new Rectangle(0f, 0f, (float)renderTarget.Texture.Width, (float)-renderTarget.Texture.Height);
      var destinationRec = new Rectangle(
        (screenWidth - ((float)gameScreenWidth * scaleX)) * 0.5f,
        (screenHeight - ((float)gameScreenHeight * scaleY)) * 0.5f,
        (float)gameScreenWidth * scaleX,
        (float)gameScreenHeight * scaleY);

      DrawTexturePro(renderTarget.Texture, sourceRec, destinationRec, new Vector2(0, 0), 0f, Color.White);
      DrawFPS(15, 10);
      DrawText($"DrawCount: {drawCount}/{Map.TileData.Count}", 15, 35, 25, Color.Red);
      EndDrawing();
    }
  }
}
