using System.Text.Json.Serialization;
using nkast.Aether.Physics2D.Common;

namespace Thaloria.Loaders.Tiled
{
  public sealed class TiledCollisionObject
  {
    [JsonPropertyName("height")]
    public double Height { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("rotation")]
    public int Rotation { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("visible")]
    public bool Visible { get; set; }

    [JsonPropertyName("width")]
    public double Width { get; set; }

    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonIgnore]
    public float Xf => (float)X;

    [JsonPropertyName("y")]
    public double Y { get; set; }

    [JsonIgnore]
    public float Yf => (float)Y;

    [JsonPropertyName("polygon")]
    public PolygonVector[]? Polygons { get; set; }

    public Vector2[]? Vertices => Polygons?.Select(i => new Vector2(i.Xf, i.Yf)).ToArray();

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

    public bool TryGetStringProperty(string name, out string value)
    {
      var property = Properties?.FirstOrDefault(x => x.Name == name);
      if (property != null)
      {
        value = property.Value?.ToString();
        return true;
      }

      value = string.Empty;
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
}
