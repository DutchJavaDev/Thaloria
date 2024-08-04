using System.Text.Json.Serialization;

namespace Thaloria.Game.Map.Tiled
{
  public sealed class TiledMapTileProperty
  {
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }
  }
}
