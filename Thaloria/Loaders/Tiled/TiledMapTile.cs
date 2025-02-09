using System.Text.Json.Serialization;

namespace Thaloria.Loaders.Tiled
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
    
    [JsonPropertyName("animation")]
    public List<Animation>? Animations { get; set; }

    public bool HasAnimation => Animations != null && Animations.Any();

    public bool TryGetStringProperty(string name, out string value)
    {
      var property = Properties?.FirstOrDefault(i => i.Name == name);

      if(property != null)
      {
        value = property.Value.ToString() ?? string.Empty;
        return true;
      }

      value = string.Empty;
      return false;
    }

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

    public bool TryGetBoolProperty(string name, out bool value)
    {
      var property = Properties?.FirstOrDefault(x => x.Name == name);

      if (property != null) 
      {
        value = bool.Parse(property.Value?.ToString() ?? string.Empty);
        return true;
      }

      value = false;
      return false;
    }
  }

  public sealed class Animation
  {
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
    [JsonPropertyName("tileid")]
    public int TileId { get; set; }
  }
}
