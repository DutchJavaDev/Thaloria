using System.Text.Json.Serialization;

namespace Thaloria.Loaders.Tiled
{
  public sealed class TiledMapTileSetImage
  {
    //[JsonPropertyName("columns")]
    //public int Columns { get; set; }

    [JsonPropertyName("image")]
    public string ImageName { get; set; }

    [JsonPropertyName("imageheight")]
    public int Imageheight { get; set; }

    [JsonPropertyName("imagewidth")]
    public int Imagewidth { get; set; }

    [JsonPropertyName("tiles")]
    public TiledMapTile[] Tiles { get; set; }
    //[JsonPropertyName("margin")]
    //public int Margin { get; set; }

    //[JsonPropertyName("name")]
    //public string Name { get; set; }

    //[JsonPropertyName("spacing")]
    //public int Spacing { get; set; }

    [JsonPropertyName("tilecount")]
    public int Tilecount { get; set; }

    //[JsonPropertyName("tiledversion")]
    //public string Tiledversion { get; set; }

    //[JsonPropertyName("tileheight")]
    //public int Tileheight { get; set; }

    //[JsonPropertyName("tilewidth")]
    //public int Tilewidth { get; set; }

    //[JsonPropertyName("type")]
    //public string Type { get; set; }

    //[JsonPropertyName("version")]
    //public string Version { get; set; }
  }
}
