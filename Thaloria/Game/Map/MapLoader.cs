using Raylib_cs;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using Thaloria.Game.Map.Tiled;
using Thaloria.Game.Physics;
using Thaloria.Loaders;

namespace Thaloria.Game.Map
{
  public sealed class MapLoader(string mapName)
  {
    private static readonly string GroundLayerName = "ground";
    private static readonly string TopLayerName = "top";
    private static readonly string CollisionLayerObjectsName = "collision";
    private static readonly string ObjectsLayerName = "objects";
    private static readonly string MapFileExtension = ".tmj";
    private static readonly string TilesetFileExtension = ".tsj";
    private static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();

    private string ImageName = string.Empty;
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
    private TiledMapTile[]? TileCollisionData = [];
    private readonly CustomTileLoader CustomTileLoader = new();
    private TiledMapTileSet? currentMapTileSet;
    private TiledMap? tiledMap;

    public TiledCollisionObject GetTileCollisionObjectByName(string name)
    {
      var @object = Layers.Find(i => i.Name == ObjectsLayerName);

      return @object.Objects.Find(i => i.Name.Equals(name));
    }

    public IEnumerable<TiledCollisionObject> GetObjectsByBame(string name)
    {
      var @object = Layers.Find(i => i.Name == ObjectsLayerName);

      return @object.Objects.Where(i => i.Name.Equals(name));
    }

    public async Task LoadMap()
    {
      if (Layers is null || Layers.Count == 0)
      {
        var mapResourcePath = CreateResourcePath("Maps",$"{mapName}{MapFileExtension}");

        tiledMap = await DeserilizeResouceFromStream<TiledMap>(mapResourcePath);

        MapWidth = tiledMap.Width * tiledMap.Tilewidth;
        MapHeight = tiledMap.Height * tiledMap.Tileheight;
        TileWidth = tiledMap.Tilewidth;
        TileHeight = tiledMap.Tileheight;

        Layers = tiledMap.Layers;

        // Using a single tileset
        //var tileSetName = tiledExport.Tilesets[0].Source.Trim().Split(@"../Tiled/")[1].Split('.')[0];

        //var tilesetNames = tiledExport.Tilesets.Select(i => i.Source.Trim().Split(@"../Tiled/")[1].Split('.')[0]);

        foreach (var tileset in tiledMap.Tilesets)
        {
          currentMapTileSet = tileset;

          await LoadTileSet(currentMapTileSet);
        }

        LoadCollisionObjects();
      }
    }
    private async Task LoadTileSet(TiledMapTileSet tileset)
    {
      var tilesetName = tileset.Source.Trim().Split(@"../Tiled/")[1].Split('.')[0];

      var tileSetResourcePath = CreateResourcePath("Maps", $"{tilesetName}{TilesetFileExtension}");

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

      ResourceManager.LoadResourceTexture2DTileset(ImageName, ImageName);

      tileset.Loaded = true;
    }
    private void LoadTileData()
    {
      TileData ??= [];

      var tiledMapWidth = MapWidth / TileWidth;
      var tiledMapHeight = MapHeight / TileHeight;

      var groundLayer = Layers.First(i => i.Name == GroundLayerName);
      var topLayer = Layers.First(i => i.Name == TopLayerName);

      for (int x = 0; x < tiledMapWidth; x++)
      {
        for (int y = 0; y < tiledMapHeight; y++)
        {
          var nextTilemap = tiledMap?.Tilesets?.Where(i => !i.Source.Equals(currentMapTileSet?.Source) && !i.Loaded).FirstOrDefault();

          // Ground layer
          LoadGroundLayer(x,y,groundLayer,nextTilemap);

          // Top layer
          LoadTopLayer(x,y,topLayer,nextTilemap);
        }
      }
    }
    private void LoadGroundLayer(int x, int y, TiledMapLayer groundLayer, TiledMapTileSet? nextTilemap) 
    {
      // Ground layer
      var groundTileId = GetTileId(x, y, groundLayer.Data, groundLayer.Width, groundLayer.Height);
      var xposition = x * TileWidth;
      var yposition = y * TileHeight;

      if (nextTilemap != null)
      {
        if (groundTileId >= nextTilemap.Firstgid)
        {
          return;
        }
      }
      else
      {
        if (groundTileId < currentMapTileSet?.Firstgid)
        {
          return;
        }
      }

      if (groundTileId != 0)
      {
        // water sheet / ground layer animated tiles
        if (groundTileId >= currentMapTileSet?.Firstgid)
        {
          var groundTileMetaData = TileCollisionData?.FirstOrDefault(i => i.TileId == (groundTileId) - currentMapTileSet.Firstgid);

          if (groundTileMetaData != null)
          {
            if (groundTileMetaData.HasAnimation)
            {
              var ids = groundTileMetaData?.Animations?.Select(i => i.TileId).ToArray();
              var frames = new List<Rectangle>();
              for (int i = 0; i < ids?.Length; i++)
              {
                var id = ids[i];

                var frameTexturePosition = GetTexturePosition(id + 1, TileWidth, TileHeight, TileSetImageWidth);
                frames.Add(new()
                {
                  Position = frameTexturePosition,
                  Width = TileWidth,
                  Height = TileHeight,
                });
              }

              // Why do i need to do +1
              AddCollisionBodies((groundTileId - currentMapTileSet.Firstgid)+1, xposition, yposition);

              TileData.Add(new(groundLayer.Id, groundTileId, new(), new(xposition, yposition), ImageName, true, [.. frames]));
              return;
            }
          }
        }

        AddTile(groundLayer.Id, groundTileId, xposition, yposition);
      }

    }
    private void LoadTopLayer(int x, int y, TiledMapLayer topLayer, TiledMapTileSet? nextTilemap) 
    {
      var topTileId = GetTileId(x, y, topLayer.Data, topLayer.Width, topLayer.Height);

      if (topTileId == 0)
        return;

      if (nextTilemap != null)
      {
        if (topTileId >= nextTilemap.Firstgid)
          return;
      }
      else
      {
        if (topTileId < currentMapTileSet?.Firstgid)
        {
          return;
        }
      }

      // Need to subtract one because the exported index are off lol
      var topTileMetaData = TileCollisionData?.FirstOrDefault(i => i.TileId == topTileId - 1);

      var hasParentId = false;

      var xposition = x * TileWidth;
      var yposition = y * TileHeight;

      // Get texture location
      if (topTileMetaData != null && !string.IsNullOrEmpty(topTileMetaData.TextureName))
      {
        // Check if it has parent_id, if it does then skip
        hasParentId = topTileMetaData.TryGetIntProperty("parent_id", out _);

        if (hasParentId)
        {
          return;
        }

        var texturePostion = CustomTileLoader.GetRectangle(topTileMetaData.TextureName);

        TileData.Add(new(topLayer.Id, topTileId, texturePostion, new(xposition, yposition), ImageName));
      }
      else
      {
        // Check if it has parent_id, if it does then skip
        // Hate this double check...
        if (topTileId == 0 || topTileMetaData != null && topTileMetaData.TryGetIntProperty("parent_id", out _))
        {
          return;
        } // This still needed?

        AddTile(topLayer.Id, topTileId, xposition, yposition);
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

      AddCollisionBodies(tileId, xposition, yposition);

      TileData.Add(new(layerId, tileId, texturePostion, new(xposition, yposition),ImageName));
    }
    private void AddCollisionBodies(int tileId, int xposition, int yposition)
    {
      var collisonBodies = TileCollisionData?.Where(i => i.TileId == tileId - 1).FirstOrDefault()?.CollisionGroup?.CollisionObjects;
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
    }
    private static int GetTileId(int x, int y, List<int> data, int Width, int Height)
    {
      if (x < 0 || x >= Width || y < 0 || y >= Height)
        return -1;

      int index = y * Width + x;
      return data[index];
    }
    private static Vector2 GetTexturePosition(int tileId, int tileWidth, int tileHeight, int textureWidth)
    {
      // Calculate the number of columns in the texture
      int cols = textureWidth / tileWidth;

      // start at 0 not 1
      // zero based index?, need to find out why I actually need this
      // but works so don't touch
      tileId--;

      // Calculate the column and row based on the image ID
      int col = tileId % cols;
      int row = tileId / cols;

      // Calculate the x and y coordinates
      int x = col * tileWidth;
      int y = row * tileHeight;

      return new(x, y);
    }
    private static async Task<T?> DeserilizeResouceFromStream<T>(string path) where T : class
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
  public readonly struct TileData(int layerId, int tileId, Rectangle texturePos, 
    Vector2 renderPos, string textureName, bool 
    hasAnimation = false, Rectangle[]? renderFrames = default)
  {
    public readonly Guid guid = Guid.NewGuid();
    public readonly short LayerId = (short)layerId;
    public readonly short TileId = (short)tileId;
    public readonly Rectangle TexturePosition = texturePos;
    public readonly Vector2 RenderPosition = renderPos;
    public readonly string TextureName = textureName;
    public readonly bool HasAnimation = hasAnimation;
    public readonly Rectangle[] RenderFrames = renderFrames ?? [];
  }
}
