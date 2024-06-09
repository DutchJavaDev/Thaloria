using Raylib_cs;
using static Raylib_cs.Raymath;
using Thaloria.Loaders;
using static Raylib_cs.Raylib;

namespace Thaloria
{
  static class Program
  {
    static unsafe void Main(string[] args)
    {
      var worldTiles = TiledLoader.CreateWorldTiles();
      
      var windowWidth = 1280;
      var windowHeight = 720;

      var playerWidth = 10;
      var playerHeight = 10;

      SetConfigFlags(ConfigFlags.VSyncHint);

      InitWindow(windowWidth, windowHeight, "Thaloria");
      
      SetTargetFPS(60);

      var plainsTexture = LoadTexture("Resources\\plains.png");

      var player = new Rectangle 
      {
        X = windowWidth / 2 - playerWidth / 2,
        Y = windowHeight / 2 - playerHeight / 2,
        Width = playerWidth,
        Height = playerHeight
      };

      Camera2D camera = new Camera2D();
      camera.Target = player.Position;

      camera.Offset = new() {
        X = windowWidth/2,
        Y = windowHeight/2
      };
      camera.Rotation = 0f;
      camera.Zoom = 2.0f;

      float speed = 100f;
      int drawCount = 0;

      while (!WindowShouldClose())
      {
        var force = GetFrameTime() * speed;

        if (IsKeyDown(KeyboardKey.A))
        {
          player.X -= force;
        }

        if(IsKeyDown(KeyboardKey.D))
        {
          player.X += force;
        }

        if (IsKeyDown(KeyboardKey.W)) 
        { 
          player.Y -= force; 
        }

        if (IsKeyDown(KeyboardKey.S))
        {
          player.Y += force;
        }

        camera.Target = player.Position;

        // Clamp camera to 0 when going left
        if (camera.Target.X - camera.Offset.X / 2 < 0)
        {
          camera.Target.X = camera.Offset.X / 2;
        }

        // Clamp camera to max width when going right
        if (camera.Target.X + camera.Offset.X / 2 > TiledLoader.WorldWidth)
        {
          camera.Target.X = TiledLoader.WorldWidth - camera.Offset.X / 2;
        }

        // Clamp camera to 0 when going up
        if (camera.Target.Y - camera.Offset.Y / 2 < 0)
        {
          camera.Target.Y = camera.Offset.Y / 2;
        }

        // Clamp camera to max height when going down
        if (camera.Target.Y + camera.Offset.Y / 2 > TiledLoader.WorldHeight)
        {
          camera.Target.Y = TiledLoader.WorldHeight - camera.Offset.Y / 2;
        }

        var cameraView = new Rectangle(camera.Target.X-camera.Offset.X/2, camera.Target.Y-camera.Offset.Y/2, camera.Offset.X, camera.Offset.Y);

        BeginDrawing();
        ClearBackground(Color.Black);
        BeginMode2D(camera);
        drawCount = 0;
        foreach (var tile in worldTiles)
        {
          var x = tile.RenderPosition.X;
          var y = tile.RenderPosition.Y;
          var width = tile.TexturePosition.Width;
          var height = tile.TexturePosition.Height;

          if (CheckCollisionRecs(cameraView, new Rectangle(x, y, width, height)))
          { 
            DrawTextureRec(plainsTexture, tile.TexturePosition, tile.RenderPosition, Color.White);
            drawCount++;
          }
        }
        DrawRectangleLinesEx(cameraView, 1, Color.Yellow);
        DrawRectangleRec(player, Color.Green);
        EndMode2D();
        DrawFPS(15,10);
        DrawText($"Tiles drawn: {drawCount}",150,150,18,Color.Red);
        EndDrawing();
      }
      UnloadTexture(plainsTexture);
      CloseWindow();
    }
  }
}
