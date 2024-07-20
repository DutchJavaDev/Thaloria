using System.Text.Json.Serialization;

namespace Thaloria.Game.Map.Tiled
{
  public sealed class TiledMapTileSet
  {
    //[JsonPropertyName("firstgid")]
    //public int Firstgid { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }
  }
}
