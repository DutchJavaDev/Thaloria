using System.Text.Json.Serialization;

namespace Thaloria.Loaders.Tiled
{
  public sealed class TiledMap
  {
    //[JsonPropertyName("compressionlevel")]
    //public int Compressionlevel { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    //[JsonPropertyName("infinite")]
    //public bool Infinite { get; set; }

    [JsonPropertyName("layers")]
    public List<TiledMapLayer> Layers { get; set; }

    //[JsonPropertyName("nextlayerid")]
    //public int Nextlayerid { get; set; }

    //[JsonPropertyName("nextobjectid")]
    //public int Nextobjectid { get; set; }

    //[JsonPropertyName("orientation")]
    //public string Orientation { get; set; }

    //[JsonPropertyName("renderorder")]
    //public string Renderorder { get; set; }

    //[JsonPropertyName("tiledversion")]
    //public string Tiledversion { get; set; }

    [JsonPropertyName("tileheight")]
    public int Tileheight { get; set; }

    [JsonPropertyName("tilesets")]
    public List<TiledMapTileSet> Tilesets { get; set; }

    [JsonPropertyName("tilewidth")]
    public int Tilewidth { get; set; }

    //[JsonPropertyName("type")]
    //public string Type { get; set; }

    //[JsonPropertyName("version")]
    //public string Version { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }
  }
}
