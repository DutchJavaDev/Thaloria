using Raylib_cs;
using Thaloria.Loaders;
using static Raylib_cs.Raylib;
using System.Numerics;
using Thaloria.World;

namespace Thaloria
{
  static class Program
  {
		private static ThaloriaGame Thaloria = new();
	static unsafe void Main(string[] args)
	{
	  var worldTiles = TiledLoader.CreateWorldTiles();
	  
	  var windowWidth = 1600;
	  var windowHeight = 900;

	  // scaling
	  var gameScreenWidth = 640;
	  var gameScreenHeight = 480;

	  SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint);
	  InitWindow(windowWidth,windowHeight,"Thaloria");
	  SetWindowMinSize(320, 180);

	  SetTargetFPS(45);
	  
	  var plainsTexture = LoadTexture("Resources\\plains.png");

	  RenderTexture2D renderTarget = LoadRenderTexture(gameScreenWidth, gameScreenHeight);
	  SetTextureFilter(renderTarget.Texture, TextureFilter.Anisotropic16X);

	  var player = new Rectangle 
	  {
		X = 0,
		Y = 0,
		Width = 10,
		Height = 10
	  };

	  Camera2D camera = new()
	  {
		Target = player.Position,

		Offset = new()
		{
		  X = gameScreenWidth / 2,
		  Y = gameScreenHeight / 2
		},
		Rotation = 0f,
		Zoom = 2.0f
	  };

	  float speed = 250f;
	  int drawCount;

	  while (!WindowShouldClose())
	  {
		var screenWidth = (float)GetScreenWidth();
		var screenHeight = (float)GetScreenHeight();

		var scale = MathF.Min(screenWidth / gameScreenWidth, screenHeight / gameScreenHeight);

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

		BeginTextureMode(renderTarget);
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
		EndTextureMode();

		BeginDrawing();
		ClearBackground(Color.Black);
		var sourceRec = new Rectangle(0f,0f,(float)renderTarget.Texture.Width,(float)-renderTarget.Texture.Height);
		var destinationRec = new Rectangle(
		  (screenWidth - ((float)gameScreenWidth*scale))*0.5f,
		  (screenHeight - ((float)gameScreenHeight*scale))*0.5f,
		  (float)gameScreenWidth*scale,
		  (float)gameScreenHeight*scale);

		DrawTexturePro(renderTarget.Texture,sourceRec,destinationRec,new Vector2(0,0), 0f, Color.White);
		DrawFPS(15,10);
		DrawText($"Tiles: {drawCount}",15,35,25,Color.Red);
		EndDrawing();
	  }

	  UnloadTexture(renderTarget.Texture);
	  UnloadTexture(plainsTexture);
	  CloseWindow();
	}
  }
}
