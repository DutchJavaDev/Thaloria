using Raylib_cs;
using System.Numerics;
using Thaloria.Game.ECS.Class;
using Thaloria.Game.Map.Tiled;
using Thaloria.Game.Physics;
using Thaloria.Loaders;

namespace Thaloria.Game.Map
{
  public sealed class MapLoader(string mapName)
  {
    private static readonly string TileLayerName = "tilelayer";
    private static readonly string CollisionLayerObjectsName = "collision";
    private static readonly string ObjectsLayerName = "objects";
    private static readonly string MapFileExtension = ".tmj";
    private static readonly string TilesetFileExtension = ".tsj";

    private string ImageName = string.Empty;
    public int MapWidth { get; private set; } = 0;
    public int MapHeight { get; private set; } = 0;
    public int TileWidth { get; private set; } = 0;
    public int TileHeight { get; private set; } = 0;
    public int TileSetImageWidth { get; private set; } = 0;
    public int TileSetImageHeight { get; private set; } = 0;
    public List<TileData> TileData { get; private set; } = [];
    public TileData[] GroundTileData => TileData.Where(i => i.LayerId == 1).ToArray();
    public TileData[] TopTileData => TileData.Where(i => i.LayerId >= 2).ToArray();

    private List<TiledMapLayer> Layers = [];
    private TiledMapTile[]? TileCollisionData = [];
    private readonly CustomTileLoader CustomTileLoader = new();
    private TiledMap? tiledMap;

    public TiledCollisionObject GetObjectByName(string name)
    {
      var @object = Layers.Find(i => i.Name == ObjectsLayerName);

      return @object.Objects.FirstOrDefault(i => i.Name.Equals(name));
    }
    public IEnumerable<TiledCollisionObject> GetObjectsByBame(string name)
    {
      var @object = Layers.Find(i => i.Name == ObjectsLayerName);

      return @object.Objects.Where(i => i.Name.Equals(name));
    }
    public async Task LoadMap()
    {
      if (Layers.Count == 0)
      {
        var mapResourcePath = AssemblyDataLoader.CreateMapResourcePath($"{mapName}{MapFileExtension}");

        tiledMap = await AssemblyDataLoader.DeserilizeResouceFromStream<TiledMap>(mapResourcePath);

        MapWidth = tiledMap.Width * tiledMap.Tilewidth;
        MapHeight = tiledMap.Height * tiledMap.Tileheight;
        TileWidth = tiledMap.Tilewidth;
        TileHeight = tiledMap.Tileheight;

        Layers = tiledMap.Layers;

        foreach (var tileset in tiledMap.Tilesets)
        { 
          await LoadTileSet(tileset);
        }

        LoadTileLayers();

        LoadCollisionObjects();
      }
    }
    private async Task LoadTileSet(TiledMapTileSet tileset)
    {
      var tilesetName = tileset.Source.Trim().Split(@"../Tiled/")[1].Split('.')[0];

      var tileSetResourcePath = AssemblyDataLoader.CreateMapResourcePath($"{tilesetName}{TilesetFileExtension}");

      var tileSetImage = await AssemblyDataLoader.DeserilizeResouceFromStream<TiledMapTileSetImage>(tileSetResourcePath);

      if (tileSetImage != null)
      {
        TileSetImageWidth = tileSetImage.Imagewidth;
        TileSetImageHeight = tileSetImage.Imageheight;
        ImageName = tileSetImage.ImageName.Trim().Split(@"../Tilesets/")[1];
        TileCollisionData = tileSetImage.Tiles;
      }

      // Load TileAtlas data
      var tileAtlasPath = AssemblyDataLoader.CreateTilesetResourcePath($"{ImageName.Split('.')[0]}.json");
      var tileAtlas = await AssemblyDataLoader.DeserilizeResouceFromStream<TileAtlas>(tileAtlasPath);
      if (tileAtlas != null) CustomTileLoader.LoadAtlasData(tileAtlas);

      ResourceManager.LoadResourceTexture2DTileset(ImageName, ImageName);
    }
    private void LoadTileLayers()
    {
      var tiledMapWidth = MapWidth / TileWidth;
      var tiledMapHeight = MapHeight / TileHeight;

      var tileLayers = Layers.Where(i => i.Type == TileLayerName);

      foreach (var tileLayer in tileLayers)
      {
        LoadLayer(tileLayer, tiledMapWidth, tiledMapHeight);
      }
    }
    private void LoadLayer(TiledMapLayer layer, int mapWidth, int mapHeight)
    {
      for (int x = 0; x < mapWidth; x++)
      {
        for (int y = 0; y < mapHeight; y++)
        {
          var tileId = GetTileId(x,y,layer.Data,layer.Width,layer.Height);

          if (tileId == 0)
            continue;

          var xposition = x * TileWidth;
          var yposition = y * TileHeight;

          var tileMetaData = TileCollisionData?.FirstOrDefault(i => i.TileId == tileId-1);

          if(tileMetaData != null)
          {
            // Check for animation
            if (tileMetaData.HasAnimation)
            {
              var frameIds = tileMetaData?.Animations?.Select(i => i.TileId).ToArray();

              if (frameIds != null)
              {
                var frames = frameIds.Select(id => new Rectangle 
                {
                  Position = GetTexturePosition(id + 1, TileWidth, TileHeight, TileSetImageWidth),
                  Width = TileWidth, 
                  Height = TileHeight,
                }).ToArray();

                var renderPosition = new Vector2(xposition,yposition);

                if (tileMetaData != null)
                {
                  _ = tileMetaData.TryGetBoolProperty("fixed_animation", out bool fixedAnimation);

                  var tile = new TileData(layer.Id, tileId, new(), renderPosition, ImageName, true, frames, fixedAnimation);
              
                  TileData.Add(tile);
              
                  AddCollisionBodies(tileId, xposition, yposition, tile.TileGuid);
                }
              }

              continue;
            }

            var hasParentId = tileMetaData.TryGetIntProperty("parent_id", out _);
            
            // Check for texture location -> parent_id
            if (!string.IsNullOrEmpty(tileMetaData.TextureName))
            {
              if (hasParentId)
              {
                continue;
              }

              var texturePostion = CustomTileLoader.GetRectangle(tileMetaData.TextureName);

              TileData.Add(new(layer.Id, tileId, texturePostion, new(xposition, yposition), ImageName));
            }
            else
            {
              AddTile(layer.Id, tileId, xposition, yposition);
            }
          }
          else
          {
            // Add tile
            AddTile(layer.Id, tileId, xposition, yposition);
          }
        }
      }
    }
    // Use for buildings etc
    private void LoadCollisionObjects()
    {
      var collisionLayer = Layers.First(i => i.Name == CollisionLayerObjectsName);

      foreach (var obj in collisionLayer.Objects)
      {
        var vertices = obj.Vertices;

        var width = (float)obj.Width;
        var height = (float)obj.Height;
        var x = (float)obj.X + width / 2;
        var y = (float)obj.Y + height / 1.5f;

        PhysicsWorld.Instance.CreateChainBody(x, y, vertices);
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

      TileData.Add(new(layerId, tileId, texturePostion, new(xposition, yposition), ImageName));
    }
    private void AddCollisionBodies(int tileId, int xposition, int yposition, Guid guid = default)
    {
      var collisonBodies = TileCollisionData?.Where(i => i.TileId == tileId - 1).FirstOrDefault()?.CollisionGroup?.CollisionObjects;
      var hasCollisionBodies = collisonBodies?.Length > 0;

      var tag = new TagObject 
      {
        TileTag = guid
      };

      if (hasCollisionBodies && collisonBodies != null)
      {
        foreach (var obj in collisonBodies)
        {
          if (obj.Polygons != null)
          {
            var vertices = obj.Vertices;

            var width = (float)obj.Width;
            var height = (float)obj.Height;
            var x = (float)(xposition + obj.RelativeX) + width / 2;
            var y = (float)(yposition + obj.RelativeY) + height / 1.5f;

            PhysicsWorld.Instance.CreateChainBody(x, y, vertices, tag, CollisionResolver.GetObjectOnCollisionEventHandler(tileId));
          }
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
  }
  /// Tiled class data
  public readonly struct TileData(int layerId, int tileId, Rectangle texturePos,
    Vector2 renderPos, string textureName, bool
    hasAnimation = false, Rectangle[]? renderFrames = default, bool fixedAnimation = false)
  {
    public readonly Guid TileGuid = Guid.NewGuid();
    public readonly short LayerId = (short)layerId;
    public readonly short TileId = (short)tileId;
    public readonly Rectangle TexturePosition = texturePos;
    public readonly Vector2 RenderPosition = renderPos;
    public readonly string TextureName = textureName;
    public readonly bool HasAnimation = hasAnimation;
    public readonly Rectangle[] RenderFrames = renderFrames ?? [];
    public readonly bool FixedAnimation = fixedAnimation;
  }
}
