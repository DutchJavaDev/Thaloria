using Raylib_cs;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thaloria.Loaders
{
  internal sealed class TiledLoader
  {
    private static readonly int GroundLayerId = 0;
    private static readonly int[] GroundLayerIdIGnoreds = [0];

    private static readonly int TopLayerId = 1;
    private static readonly int[] TopLayerIdIgnores = [0];


    public static int WorldWidth;
    public static int WorldHeight;

    public static IList<TileReference> CreateWorldTiles()
    {
      var assembly  = Assembly.GetExecutingAssembly();

      // First get the map
      var mapName = CreateResourcePath("map.tmj");
      using var mapStream = assembly.GetManifestResourceStream(mapName);
      
      TiledExport mapData;
      using (var tileExportReader = new StreamReader(mapStream))
      {
        mapData = JsonSerializer.Deserialize<TiledExport>(tileExportReader.ReadToEnd());
      }

      // Get tileset info
      var tilesetName = CreateResourcePath($"{mapData.Tilesets[0].Source.Split('.')[0]}.tsj");
      using var tileSetStream = assembly.GetManifestResourceStream(tilesetName);
      TileSetImage tilesetImage;
      using (var tileSetImageReader = new StreamReader(tileSetStream))
      {
        tilesetImage = JsonSerializer.Deserialize<TileSetImage>(tileSetImageReader.ReadToEnd());
      }
      
      var tiles = new List<TileReference>();

      WorldWidth = mapData.Width * mapData.Tilewidth;
      WorldHeight = mapData.Height * mapData.Tileheight;

      // Create tiles
      for (var x = 0; x < mapData.Width; x++)
      {
        for (var y = 0; y < mapData.Height; y++)
        {
          for (var layerId = 0; layerId < mapData.Layers.Count; layerId++)
          {
            var layer = mapData.Layers[layerId];
            
            var tileId = GetValue(x,y,layer.Data,layer.Width,layer.Height);

            if (layerId == GroundLayerId && GroundLayerIdIGnoreds.Contains(tileId))
              continue;

            if (layerId == TopLayerId && TopLayerIdIgnores.Contains(tileId))
              continue;

            var tileX = x * mapData.Tilewidth;

            var tileY = y * mapData.Tileheight;

            var texturePosition = GetImageCoordinates(tileId, mapData.Tilewidth, mapData.Tileheight, tilesetImage.Imagewidth);

            var texturePos = new Rectangle 
            {
              X = texturePosition.x,
              Y = texturePosition.y,
              Width = mapData.Tilewidth,
              Height = mapData.Tileheight
            };

            var renderPos = new Vector2(tileX, tileY);

            tiles.Add(new TileReference(layerId, tileId, texturePos, renderPos));
          }
        }
      }

      return tiles;
    }

    private static string CreateResourcePath(string name)
    {
      return $"Thaloria.Resources.{name}";
    }

    static int GetValue(int x, int y, List<int> data, int Width, int Height)
    {
      if (x < 0 || x >= Width || y < 0 || y >= Height)
        return -1;

      int index = y * Width + x;
      return data[index];
    }

    static (int x, int y) GetImageCoordinates(int imageId, int imageWidth, int imageHeight, int textureWidth)
    {
      // Calculate the number of columns in the texture
      int cols = textureWidth / imageWidth;

      // start at 0 not 1
      var indexOffset = 1;

      // Calculate the column and row based on the image ID
      int col = (imageId - indexOffset) % cols;
      int row = imageId / cols;

      // Calculate the x and y coordinates
      int x = col * imageWidth;
      int y = row * imageHeight;

      return (x, y);
    }

    public readonly struct TileReference(int layerId, int tileId, Rectangle texturePos, Vector2 renderPos)
    {
      public readonly int LayerId = layerId;
      public readonly int TileId = tileId;
      public readonly Rectangle TexturePosition = texturePos;
      public readonly Vector2 RenderPosition = renderPos;
    }

    public class Layer
    {
      [JsonPropertyName("data")]
      public List<int> Data { get; set; }

      [JsonPropertyName("height")]
      public int Height { get; set; }

      [JsonPropertyName("id")]
      public int Id { get; set; }

      [JsonPropertyName("name")]
      public string Name { get; set; }

      [JsonPropertyName("opacity")]
      public int Opacity { get; set; }

      [JsonPropertyName("type")]
      public string Type { get; set; }

      [JsonPropertyName("visible")]
      public bool Visible { get; set; }

      [JsonPropertyName("width")]
      public int Width { get; set; }

      [JsonPropertyName("x")]
      public int X { get; set; }

      [JsonPropertyName("y")]
      public int Y { get; set; }
    }

    public class TiledExport
    {
      [JsonPropertyName("compressionlevel")]
      public int Compressionlevel { get; set; }

      [JsonPropertyName("height")]
      public int Height { get; set; }

      [JsonPropertyName("infinite")]
      public bool Infinite { get; set; }

      [JsonPropertyName("layers")]
      public List<Layer> Layers { get; set; }

      [JsonPropertyName("nextlayerid")]
      public int Nextlayerid { get; set; }

      [JsonPropertyName("nextobjectid")]
      public int Nextobjectid { get; set; }

      [JsonPropertyName("orientation")]
      public string Orientation { get; set; }

      [JsonPropertyName("renderorder")]
      public string Renderorder { get; set; }

      [JsonPropertyName("tiledversion")]
      public string Tiledversion { get; set; }

      [JsonPropertyName("tileheight")]
      public int Tileheight { get; set; }

      [JsonPropertyName("tilesets")]
      public List<Tileset> Tilesets { get; set; }

      [JsonPropertyName("tilewidth")]
      public int Tilewidth { get; set; }

      [JsonPropertyName("type")]
      public string Type { get; set; }

      [JsonPropertyName("version")]
      public string Version { get; set; }

      [JsonPropertyName("width")]
      public int Width { get; set; }
    }

    public class Tileset
    {
      [JsonPropertyName("firstgid")]
      public int Firstgid { get; set; }

      [JsonPropertyName("source")]
      public string Source { get; set; }
    }

    public class TileSetImage
    {
      [JsonPropertyName("columns")]
      public int Columns { get; set; }

      [JsonPropertyName("image")]
      public string Image { get; set; }

      [JsonPropertyName("imageheight")]
      public int Imageheight { get; set; }

      [JsonPropertyName("imagewidth")]
      public int Imagewidth { get; set; }

      [JsonPropertyName("margin")]
      public int Margin { get; set; }

      [JsonPropertyName("name")]
      public string Name { get; set; }

      [JsonPropertyName("spacing")]
      public int Spacing { get; set; }

      [JsonPropertyName("tilecount")]
      public int Tilecount { get; set; }

      [JsonPropertyName("tiledversion")]
      public string Tiledversion { get; set; }

      [JsonPropertyName("tileheight")]
      public int Tileheight { get; set; }

      [JsonPropertyName("tilewidth")]
      public int Tilewidth { get; set; }

      [JsonPropertyName("type")]
      public string Type { get; set; }

      [JsonPropertyName("version")]
      public string Version { get; set; }
    }

  }
}
