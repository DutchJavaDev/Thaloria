using Raylib_cs;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Thaloria.Game.Map
{
  public sealed class CustomTileLoader
  {
    private readonly Dictionary<string, Rectangle> tileLocations = [];

    public void LoadAtlasData(TileAtlas tileAtlas)
    {
      foreach (var sprite in tileAtlas.Sprites)
      {
        var location = new Rectangle 
        {
          Position = new Vector2 
          {
            X = sprite.Position.X, 
            Y = sprite.Position.Y
          },
          Width = sprite.SourceSize.Width,
          Height = sprite.SourceSize.Height
        };
        tileLocations.Add(sprite.NameId,location);
      }
    }

    public Rectangle GetRectangle(string textureName)
    {
      return tileLocations[textureName];
    }
  }

  #region AtlasData
  // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
  public class Atlas
  {
    [JsonPropertyName("imagePath")]
    public string ImagePath { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("spriteCount")]
    public int SpriteCount { get; set; }

    //[JsonPropertyName("isFont")]
    //public bool IsFont { get; set; }

    //[JsonPropertyName("fontSize")]
    //public int FontSize { get; set; }
  }

  public class Origin
  {
    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }
  }

  public class Position
  {
    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }
  }

  public class TileAtlas
  {
    //[JsonPropertyName("software")]
    //public Software Software { get; set; }

    [JsonPropertyName("atlas")]
    public Atlas Atlas { get; set; }

    [JsonPropertyName("sprites")]
    public List<Sprite> Sprites { get; set; }
  }

  //public class Software
  //{
  //  [JsonPropertyName("name")]
  //  public string Name { get; set; }

  //  [JsonPropertyName("url")]
  //  public string Url { get; set; }
  //}

  public class SourceSize
  {
    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
  }

  public class Sprite
  {
    [JsonPropertyName("nameId")]
    public string NameId { get; set; }

    //[JsonPropertyName("origin")]
    //public Origin Origin { get; set; }

    [JsonPropertyName("position")]
    public Position Position { get; set; }

    [JsonPropertyName("sourceSize")]
    public SourceSize SourceSize { get; set; }

    //[JsonPropertyName("padding")]
    //public int Padding { get; set; }

    //[JsonPropertyName("trimmed")]
    //public bool Trimmed { get; set; }

    //[JsonPropertyName("trimRec")]
    //public TrimRec TrimRec { get; set; }
  }

  //public class TrimRec
  //{
  //  [JsonPropertyName("x")]
  //  public int X { get; set; }

  //  [JsonPropertyName("y")]
  //  public int Y { get; set; }

  //  [JsonPropertyName("width")]
  //  public int Width { get; set; }

  //  [JsonPropertyName("height")]
  //  public int Height { get; set; }
  //}
  #endregion
}
