using System.Text.Json.Serialization;

namespace Thaloria.Game.Map.Tiled
{
  public sealed class TiledMapTileCollisionGroup
  {
    //[JsonPropertyName("draworder")]
    //public string draworder { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    //[JsonPropertyName("name")]
    //public string name { get; set; }

    [JsonPropertyName("objects")]
    public TiledMapTileCollisionObject[] CollisionObjects { get; set; } = [];

    //[JsonPropertyName("opacity")]
    //public int opacity { get; set; }

    //[JsonPropertyName("type")]
    //public string type { get; set; }

    //[JsonPropertyName("visible")]
    //public bool visible { get; set; }

    //[JsonPropertyName("x")]
    //public int x { get; set; }

    //[JsonPropertyName("y")]
    //public int y { get; set; }
  }
}
