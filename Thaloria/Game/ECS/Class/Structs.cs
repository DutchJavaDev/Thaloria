namespace Thaloria.Game.ECS.Class
{
  public class TagObject
  {
    public string Name { get; set; } = string.Empty;
    public int EntityTag {  get; set; }
    public Guid TileTag { get; set; } = Guid.Empty;
    public Dictionary<int, object> Data { get; set; } = [];
  }
}
