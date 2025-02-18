﻿using nkast.Aether.Physics2D.Common;
using System.Text.Json.Serialization;

namespace Thaloria.Game.Map.Tiled
{
  public sealed class TiledMapTileCollisionObject
  {
    [JsonPropertyName("height")]
    public double Height { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    //[JsonPropertyName("name")]
    //public string name { get; set; }

    //[JsonPropertyName("rotation")]
    //public int rotation { get; set; }

    //[JsonPropertyName("type")]
    //public string type { get; set; }

    //[JsonPropertyName("visible")]
    //public bool visible { get; set; }

    [JsonPropertyName("width")]
    public double Width { get; set; }

    [JsonPropertyName("x")]
    public double RelativeX { get; set; }

    [JsonPropertyName("y")]
    public double RelativeY { get; set; }

    [JsonPropertyName("polygon")]
    public PolygonVector[]? Polygons { get; set; }

    public Vector2[]? Vertices => Polygons?.Select(i => new Vector2(i.Xf, i.Yf)).ToArray();
  }

  public sealed class PolygonVector 
  {
    [JsonPropertyName("x")]
    public double X {  get; set; }
    [JsonPropertyName("y")]
    public double Y { get; set; }
    public float Xf => (float) X;
    public float Yf => (float) Y;
  }
}
