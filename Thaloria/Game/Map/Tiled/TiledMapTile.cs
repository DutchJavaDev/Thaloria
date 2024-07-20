using System.Text.Json.Serialization;

namespace Thaloria.Game.Map.Tiled
{
  public sealed class TiledMapTile
  {
    [JsonPropertyName("id")]
    public int TileId { get; set; }

    [JsonPropertyName("type")]
    public string? TextureName { get; set; } = string.Empty;

    [JsonPropertyName("objectgroup")]
    public TiledMapTileCollisionGroup? CollisionGroup { get; set; }

    [JsonPropertyName("properties")]
    public List<TiledMapTileProperty>? Properties { get; set; }

    public bool TryGetIntProperty(string name, out int value)
    {
      var property = Properties?.FirstOrDefault(i => i.Name == name);

      if (property != null)
      {
        value = int.Parse(property.Value?.ToString() ?? string.Empty);
        return true;
      }

      value = default;
      return false;
    }
  }
}
