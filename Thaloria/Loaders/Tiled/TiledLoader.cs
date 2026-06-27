using System.Collections.Concurrent;
using System.Numerics;
using Raylib_cs;

namespace Thaloria.Loaders.Tiled;

// Does the same as MapLoader.cs, but simpler
public sealed class TiledLoader
{
    private static readonly string MapName = "Thaloria.tmj";

    public async Task<WorldData> LoadMapAsync()
    {
        var worldData = new WorldData();
        var customTileLoader = new CustomTileLoader();
        
        var mapResourcePath = AssemblyDataLoader.CreateMapResourcePath(MapName);
        var tiledMap = await AssemblyDataLoader.DeserilizeResouceFromStreamAsync<TiledMap>(mapResourcePath);
        var mapWidth = tiledMap?.Width * tiledMap?.Tilewidth;
        var mapHeight = tiledMap?.Height * tiledMap?.Tileheight;
        var tileWidth = tiledMap?.Tilewidth ?? 0;
        var tileHeight = tiledMap?.Tileheight ?? 0;
        var tileMapLayers = tiledMap?.Layers;
        
        // Tilesets
        await LoadTileSetAsync(tiledMap?.Tilesets, worldData, customTileLoader);
        
        // Objects
        var objects = tileMapLayers?.FirstOrDefault(i => i.Name == "objects")?.Objects;
        
        // Player spawn
        var playerSpawn = objects?.FirstOrDefault(i => i.Name == "player_spawn");
        
        worldData.PlayerPosition = new Vector2(playerSpawn.Xf, playerSpawn.Yf);
        
        // NPC spawn
        var npcSpawns = objects?.Where(i => i.Name == "npc_spawn").ToList();
        
        foreach (var spawn in npcSpawns)
        {
            spawn.TryGetIntProperty("npc", out var npcType);
            spawn.TryGetIntProperty("amount", out var amount);
            
            worldData.NpcSpawnData.Add(new NpcSpawnData()
            {
                NpcType = npcType,
                Amount = amount,
                Position = new Vector2(spawn.Xf, spawn.Yf)
            });
        }
        
        // Collision objects
        LoadCollisionObjects(worldData, tileMapLayers?.FirstOrDefault(i => i.Name == "collision") ?? new TiledMapLayer());
        
        // Tile layers
        var width = (mapWidth / tileWidth) ?? 0;
        var height = (mapHeight / tileHeight) ?? 0;
        var layers = tileMapLayers?.Where(i => i.Type == "tilelayer").ToList();
        
        await LoadTileLayersAsync(width, height, tileWidth,tileHeight, worldData, customTileLoader, layers ?? []);
        
        return worldData;
    }
    private static void LoadCollisionObjects(WorldData worldData, TiledMapLayer collisionLayer)
    {
        var collisionObjects = collisionLayer.Objects;
        
        foreach (var obj in collisionObjects)
        {
            if (obj.Polygons != null)
            {
                var vertices = obj.Vertices;
                
                var width = (float)obj.Width;
                var height = (float)obj.Height;
                var x = (float)obj.X + width / 2;
                var y = (float)obj.Y + height / 1.5f;
                
                worldData.TileCollisionBodies.Add(new CollisionBody()
                {
                    TileId = -1,
                    Vertices = vertices ?? [],
                    Position = new Vector2(x, y)
                });
            }
        }
    }
    private static async Task LoadTileSetAsync(List<TiledMapTileSet>? tilesets, WorldData worldData, CustomTileLoader customTileLoader)
    {
        // There is only one tileset in Thaloria, if issues arise, refactor this
        var tileSet = tilesets?.FirstOrDefault();
        
        var source = tileSet?.Source.Trim().Split(@"../Tiled/")[1].Split('.')[0];
            
        var tileSetResourcePath = AssemblyDataLoader.CreateMapResourcePath($"{source}.tsj");
            
        var tileSetImage = await AssemblyDataLoader.DeserilizeResouceFromStreamAsync<TiledMapTileSetImage>(tileSetResourcePath);
        
        var imageName = tileSetImage?.ImageName.Trim().Split(@"../Tilesets/")[1];
        
        worldData.TileSetImageNamePath = imageName ?? string.Empty;
        worldData.TileSetImageWidth = tileSetImage?.Imagewidth ?? 0;
        worldData.TileSetImageHeight = tileSetImage?.Imageheight ?? 0;
        worldData.TileCollision = tileSetImage?.Tiles ?? [];
        
        // Atlas
        var tileAtlasPath = AssemblyDataLoader.CreateTilesetResourcePath($"{imageName?.Split('.')[0]}.json");
        var tileAtlas = await AssemblyDataLoader.DeserilizeResouceFromStreamAsync<TileAtlas>(tileAtlasPath);
        
        customTileLoader.LoadAtlasData(tileAtlas?? new TileAtlas
        {
            Atlas = new Atlas(),
            Sprites = []
        });
    }
    private static async Task LoadTileLayersAsync(int mapWidth, int mapHeight, int tileWidth, int tileHeight, WorldData worldData, CustomTileLoader customTileLoader, IList<TiledMapLayer> mapLayers)
    {
        var concurrentTileDataBag = new ConcurrentBag<TileData>();
        //var concurrentCollisionBodyBag = new ConcurrentBag<CollisionBody>();
        
        var tasks = mapLayers.Select(layer => Task.Run(() =>
            {
                var layerWidth = mapWidth;
                var layerHeight = mapHeight;
                var tileLayer = layer;

                for (var x = 0; x < layerWidth; x++)
                {
                    for (var y = 0; y < layerHeight; y++)
                    {
                        var tileId = GetTileId(x, y, tileLayer.Data, layerWidth, layerHeight);
                        
                        if (tileId == 0)
                            continue;
                        
                        var xposition = x * tileWidth;
                        var yposition = y * tileHeight;
                        
                        var tileMetaData = worldData.TileCollision.FirstOrDefault(i => i.TileId == tileId - 1);
                        
                        if(tileMetaData != null)
                        {
                            // Check for animation
                            if (tileMetaData.HasAnimation)
                            {
                                var frameIds = tileMetaData?.Animations?.Select(i => i.TileId).ToArray();

                                if (frameIds != null)
                                {
                                    var frames = frameIds.Select(id => new Rectangle 
                                    {
                                        Position = GetTexturePosition(id + 1, tileWidth, tileHeight, worldData.TileSetImageWidth),
                                        Width = tileWidth, 
                                        Height = tileHeight,
                                    }).ToArray();

                                    var renderPosition = new Vector2(xposition,yposition);

                                    if (tileMetaData != null)
                                    {
                                        _ = tileMetaData.TryGetBoolProperty("fixed_animation", out bool fixedAnimation);

                                        var tile = new TileData(layer.Id, tileId, new(), renderPosition, worldData.TileSetImageNamePath, true, frames, fixedAnimation);
              
                                        // add it to world data, hmmm this can be a sticky one
                                        // create a temporary list and add it to the world data after all the tasks are done
                                        concurrentTileDataBag.Add(tile);
                                        // Get collision bodies
                                        var bodies = GetCollisionBodies(tileId, xposition, yposition, worldData.TileCollision.Where(i => i.TileId == tileId-1).FirstOrDefault()?.CollisionGroup?.CollisionObjects, tile.TileGuid);
                                        worldData.TileCollisionBodies.AddRange(bodies);
                                        //AddCollisionBodies(tileId, xposition, yposition, tile.TileGuid);
                                    }
                                }

                                continue;
                            }

                            var hasParentId = tileMetaData.TryGetIntProperty("parent_id", out _);
            
                            // Check for texture location -> parent_id
                            if (!string.IsNullOrEmpty(tileMetaData.TextureName))
                            {
                                if (hasParentId)
                                {
                                    continue;
                                }

                                var texturePostion = customTileLoader.GetRectangle(tileMetaData.TextureName);

                                concurrentTileDataBag.Add(new(layer.Id, tileId, texturePostion, new(xposition, yposition), worldData.TileSetImageNamePath));
                            }
                            else
                            {
                                AddTile(concurrentTileDataBag, worldData, tileWidth, tileHeight,worldData.TileSetImageWidth,layer.Id, tileId, xposition, yposition);
                            }
                        }
                        else
                        {
                            // Add tile
                            AddTile(concurrentTileDataBag, worldData, tileWidth, tileHeight, worldData.TileSetImageWidth, layer.Id, tileId, xposition, yposition);
                        }
                    }
                }
            }))
            .ToList();

        await Task.WhenAny(tasks);
    }
    private static void AddTile(ConcurrentBag<TileData> concurrentBag, WorldData worldData, int tileWidth, int tileHeight, int textureWidth, int layerId, int tileId, int xposition, int yposition)
    {
        var textureVectorPosition = GetTexturePosition(tileId, tileWidth, tileHeight, textureWidth);
        var texturePosition = new Rectangle
        {
            Position = textureVectorPosition,
            Width = tileWidth,
            Height = tileHeight
        };
        concurrentBag.Add(new(layerId, tileId, texturePosition, new(xposition, yposition), worldData.TileSetImageNamePath));
        
        var bodies = GetCollisionBodies(tileId, xposition, yposition, worldData.TileCollision.Where(i => i.TileId == tileId-1).FirstOrDefault()?.CollisionGroup?.CollisionObjects);
        worldData.TileCollisionBodies.AddRange(bodies);
    }
    private static int GetTileId(int x, int y, List<int> data, int width, int height)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return -1;

        int index = y * width + x;
        return data[index];
    }
    private static Vector2 GetTexturePosition(int tileId, int tileWidth, int tileHeight, int textureWidth)
    {
        // Calculate the number of columns in the texture
        int cols = textureWidth / tileWidth;

        // start at 0 not 1
        // zero based index?, need to find out why I actually need this
        // but works so don't touch
        tileId--;

        // Calculate the column and row based on the image ID
        int col = tileId % cols;
        int row = tileId / cols;

        // Calculate the x and y coordinates
        int x = col * tileWidth;
        int y = row * tileHeight;

        return new(x, y);
    }
    private static List<CollisionBody> GetCollisionBodies(int tileId, int xposition, int yposition, TiledMapTileCollisionObject[] bodies, Guid guid = default)
    {
        var collisionBodies = new List<CollisionBody>();

        if (bodies == null || !bodies.Any()) return collisionBodies;
        
        foreach (var obj in bodies)
        {
            if (obj.Polygons != null)
            {
                var vertices = obj.Vertices;

                var width = (float)obj.Width;
                var height = (float)obj.Height;
                var x = (float)(xposition + obj.RelativeX) + width / 2;
                var y = (float)(yposition + obj.RelativeY) + height / 1.5f;

                collisionBodies.Add(new CollisionBody()
                {
                    TileId = tileId,
                    Vertices = vertices ?? [],
                    Position = new Vector2(x, y)
                });
            }
        }

        return collisionBodies;
    }
}