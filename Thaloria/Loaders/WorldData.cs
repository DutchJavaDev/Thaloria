using System.Numerics;
using Thaloria.Game.Helpers;
using Thaloria.Loaders.Tiled;

namespace Thaloria.Loaders;

public sealed class WorldData
{
    public IList<TileData> TileData { get; set; } = [];
    public Vector2 PlayerPosition { get; set; } = Vector2.Zero;
    // When loading use as name/filename combined 
    public string TileSetImageNamePath { get; set; } = string.Empty;
    public int TileSetImageWidth { get; set; } = 0;
    public int TileSetImageHeight { get; set; } = 0;
    public IList<TiledMapTile> TileCollision { get; set; } = [];
    public List<CollisionBody> TileCollisionBodies { get; set; } = [];
    public List<NpcSpawnData> NpcSpawnData { get; set; } = [];
}

public readonly struct CollisionBody
{
    public readonly int TileId {init; get;}
    public readonly nkast.Aether.Physics2D.Common.Vector2[] Vertices {init; get;}
    public readonly Vector2 Position {init; get;}
}

public readonly struct NpcSpawnData
{
    public readonly int NpcType {init; get;}
    public readonly int Amount {init; get;}
    public readonly Vector2 Position {init; get;}
}
