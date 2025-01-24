﻿using System.Text.Json.Serialization;

namespace Thaloria.Game.Map.Tiled
{
  public sealed class TiledMapLayer
  {
    [JsonPropertyName("data")]
    public List<int> Data { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    //[JsonPropertyName("opacity")]
    //public int Opacity { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    //[JsonPropertyName("visible")]
    //public bool Visible { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    //[JsonPropertyName("x")]
    //public int X { get; set; }

    //[JsonPropertyName("y")]
    //public int Y { get; set; }
    [JsonPropertyName("objects")]
    public List<TiledCollisionObject> Objects { get; set; }
  }
}
