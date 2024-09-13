using Raylib_cs;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using Thaloria.Game.Map.Tiled;
using Thaloria.Game.Physics;

namespace Thaloria.Game.Map
{
  public sealed class MapLoader(string mapName)
  {
    private static readonly string GroundLayerName = "ground";
    private static readonly string TopLayerName = "top";
    private static readonly string CollisionLayerObjectsName = "collision";
    private static readonly string MapFileExtension = ".tmj";
    private static readonly string TilesetFileExtension = ".tsj";
    private static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();

    public string ImageName { get; private set; } = string.Empty;
    public int MapWidth { get; private set; } = 0;
    public int MapHeight { get; private set; } = 0;
    public int TileWidth { get; private set; } = 0;
    public int TileHeight { get; private set; } = 0;
    public int TileSetImageWidth { get; private set; } = 0;
    public int TileSetImageHeight { get; private set; } = 0;
    public List<TileData> TileData { get; private set; } = [];
    public TileData[] GroundTileData => TileData.Where(i => i.LayerId == 1).ToArray();
    public TileData[] TopTileData => TileData.Where(i => i.LayerId == 2).ToArray();
    
    private List<TiledMapLayer> Layers = [];
    private TiledMapTile[] TileCollisionData = [];
    private readonly CustomTileLoader CustomTileLoader = new();

    public async Task LoadMap()
    {
      if (Layers is null || Layers.Count == 0)
      {
        var mapResourcePath = CreateResourcePath("Maps",$"{mapName}{MapFileExtension}");

        var tiledExport = await DeserilizeResouceFromStream<TiledMap>(mapResourcePath);

        MapWidth = tiledExport.Width * tiledExport.Tilewidth;
        MapHeight = tiledExport.Height * tiledExport.Tileheight;
        TileWidth = tiledExport.Tilewidth;
        TileHeight = tiledExport.Tileheight;

        Layers = tiledExport.Layers;

        // Using a single tileset
        var tileSetName = tiledExport.Tilesets[0].Source.Trim().Split(@"../Tiled/")[1].Split('.')[0];
        var tileSetResourcePath = CreateResourcePath("Maps",$"{tileSetName}{TilesetFileExtension}");

        var tileSetImage = await DeserilizeResouceFromStream<TiledMapTileSetImage>(tileSetResourcePath);

        TileSetImageWidth = tileSetImage.Imagewidth;
        TileSetImageHeight = tileSetImage.Imageheight;
        ImageName = tileSetImage.ImageName.Trim().Split(@"../Tilesets/")[1];
        TileCollisionData = tileSetImage.Tiles;

        // Load TileAtlas data
        var tileAtlasPath = CreateResourcePath("Tilesets", $"{ImageName.Split('.')[0]}.json");
        var tileAtlas = await DeserilizeResouceFromStream<TileAtlas>(tileAtlasPath);
        CustomTileLoader.LoadAtlasData(tileAtlas);

        LoadTileData();
        LoadCollisionObjects();
      }
    }

    private void LoadTileData()
    {
      TileData = [];

      var tiledMapWidth = MapWidth / TileWidth;
      var tiledMapHeight = MapHeight / TileHeight;

      var groundLayer = Layers.First(i => i.Name == GroundLayerName);
      var topLayer = Layers.First(i => i.Name == TopLayerName);

      var xposition = 0;
      var yposition = 0;

      for (int x = 0; x < tiledMapWidth; x++)
      {
        for (int y = 0; y < tiledMapHeight; y++)
        {
          // Ground layer
          var groundTileId = GetTileId(x, y, groundLayer.Data, groundLayer.Width, groundLayer.Height);
          xposition = x * TileWidth;
          yposition = y * TileHeight;
          AddTile(groundLayer.Id, groundTileId, xposition, yposition);

          // Top layer
          var topTileId = GetTileId(x, y, topLayer.Data, topLayer.Width, topLayer.Height);
          
          // Need to subtract one because the exported index are off lol
          var tileMetaData = TileCollisionData.FirstOrDefault(i => i.TileId == topTileId-1);
          
          var hasParentId = false;
          var parentId = 0;

          xposition = x * TileWidth;
          yposition = y * TileHeight;

          // Get texture location
          if (tileMetaData != null && !string.IsNullOrEmpty(tileMetaData.TextureName))
          {
            // Check if it has parent_id, if it does then skip
            hasParentId = tileMetaData.TryGetIntProperty("parent_id", out _);
            
            if (hasParentId)
            {
              continue;
            }

            var texturePostion = CustomTileLoader.GetRectangle(tileMetaData.TextureName);

            TileData.Add(new(topLayer.Id, topTileId, texturePostion, new(xposition, yposition)));
          }
          else
          {
            // Check if it has parent_id, if it does then skip
            // Hate this double check...
            if (topTileId == 0 || tileMetaData != null && tileMetaData.TryGetIntProperty("parent_id", out _))
            {
              continue;
            }

            AddTile(topLayer.Id, topTileId, xposition, yposition);
          }
        }
      }
    }

    private void LoadCollisionObjects()
    {
      var collisionLayer = Layers.First(i => i.Name == CollisionLayerObjectsName);

      foreach (var obj in collisionLayer.Objects)
      {
        var width = (float)obj.Width;
        var height = (float)obj.Height;
        var x = (float)obj.X + width / 2;
        var y = (float)obj.Y + height / 1.5f;

        PhysicsWorld.Instance.CreateStaticBody(x,y,width,height);
      }
    }

    private void AddTile(int layerId, int tileId, int xposition, int yposition)
    {
      var textureVectorPosition = GetTexturePosition(tileId, TileWidth, TileHeight, TileSetImageWidth);

      var texturePostion = new Rectangle
      {
        Position = textureVectorPosition,
        Width = TileWidth,
        Height = TileHeight,
      };

      var collisonBodies = TileCollisionData.Where(i => i.TileId == tileId-1).FirstOrDefault()?.CollisionGroup?.CollisionObjects;
      var hasCollisionBodies = collisonBodies?.Length > 0;

      if (hasCollisionBodies)
      {
        foreach (var obj in collisonBodies)
        {
          var width = (float)obj.Width;
          var height = (float)obj.Height;
          var x = (float)(xposition + obj.RelativeX) + width / 2;
          var y = (float)(yposition + obj.RelativeY) + height / 1.5f;

          PhysicsWorld.Instance.CreateStaticBody(x, y, width, height);
        }
      }

      TileData.Add(new(layerId, tileId, texturePostion, new(xposition, yposition)));
    }

    private static int GetTileId(int x, int y, List<int> data, int Width, int Height)
    {
      if (x < 0 || x >= Width || y < 0 || y >= Height)
        return -1;

      int index = y * Width + x;
      return data[index];
    }

    private static Vector2 GetTexturePosition(int tileId, int mapWidth, int mapHeight, int textureWidth)
    {
      // Calculate the number of columns in the texture
      int cols = textureWidth / mapWidth;

      // start at 0 not 1
      // zero based index?, need to find out why I actually need this
      // but works so don't touch
      tileId--;

      // Calculate the column and row based on the image ID
      int col = tileId % cols;
      int row = tileId / cols;

      // Calculate the x and y coordinates
      int x = col * mapWidth;
      int y = row * mapHeight;

      return new(x, y);
    }

    private static async Task<T> DeserilizeResouceFromStream<T>(string path) where T : class
    {
      using var resourceStream = CurrentAssembly.GetManifestResourceStream(path);

      using var resourceStreamReader = new StreamReader(resourceStream);

      return JsonSerializer.Deserialize<T>(await resourceStreamReader?.ReadToEndAsync());
    }

    private static string CreateResourcePath(string mapName, string name)
    {
      return $"Thaloria.Resources.{mapName}.{name}";
    }
  }

  /// Tiled class data
  public readonly struct TileData(int layerId, int tileId, Rectangle texturePos, Vector2 renderPos)
  {
    public readonly short LayerId = (short)layerId;
    public readonly short TileId = (short)tileId;
    public readonly Rectangle TexturePosition = texturePos;
    public readonly Vector2 RenderPosition = renderPos;
  }
}
