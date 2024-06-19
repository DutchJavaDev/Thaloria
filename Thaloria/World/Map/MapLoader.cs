using Raylib_cs;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thaloria.World.Map
{
    public sealed class MapLoader(string mapName)
    {
        private static readonly string MapFileExtension = ".tmj";
        private static readonly string TilesetFileExtension = ".tsj";
        private static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();

        // This will be different per map
        private static readonly int[] GroundLayerIdIGnoreds = [0];
        private static readonly int[] TopLayerIdIgnores = [0];

        public string ImageName { get; private set; } 
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public int TileSetImageWidth { get; private set; }
        public int TileSetImageHeight { get; private set; }
        public List<TileData> TileData { get; private set; }
        private List<Layer> Layers;

        public async Task LoadMap()
        {
            if (Layers is null || Layers.Count == 0)
            {
                var mapResourcePath = CreateResourcePath($"{mapName}{MapFileExtension}");

                var tiledExport = await DeserilizeResouceFromStream<TiledExport>(mapResourcePath);

                MapWidth = tiledExport.Width * tiledExport.Tilewidth;
                MapHeight = tiledExport.Height * tiledExport.Tileheight;
                TileWidth = tiledExport.Tilewidth;
                TileHeight = tiledExport.Tileheight;

                Layers = tiledExport.Layers;

                // Using a single tileset
                var tileSetName = tiledExport.Tilesets[0].Source.Split('.')[0];
                var tileSetResourcePath = CreateResourcePath($"{tileSetName}{TilesetFileExtension}");

                var tileSetImage = await DeserilizeResouceFromStream<TileSetImage>(tileSetResourcePath);

                TileSetImageWidth = tileSetImage.Imagewidth;
                TileSetImageHeight = tileSetImage.Imageheight;
                ImageName = tileSetImage.ImageName;

                LoadTileData();
            }
        }

        private void LoadTileData()
        {
            TileData = [];

            var tiledMapWidth = MapWidth / TileWidth;
            var tiledMapHeight = MapHeight / TileHeight;

            var groundLayer = Layers.Where(i => i.Name == "ground").First();
            var topLayer = Layers.Where(i => i.Name == "top").First();

            for (int x = 0; x < tiledMapWidth; x++)
            {
                for (int y = 0; y < tiledMapHeight; y++)
                {
                    var groundTileId = GetTileId(x, y, groundLayer.Data, groundLayer.Width, groundLayer.Height);

                    if (!GroundLayerIdIGnoreds.Contains(groundTileId))
                    {
                        var xposition = x * TileWidth;
                        var yposition = y * TileHeight;

                        var textureVector2 = GetTexturePosition(groundTileId, TileWidth, TileHeight, TileSetImageWidth);

                        var texturePostion = new Rectangle
                        {
                            Position = textureVector2,
                            Width = TileWidth,
                            Height = TileHeight,
                        };

                        TileData.Add(new(groundLayer.Id, groundTileId, texturePostion, new(xposition, yposition)));
                    }

                    var topTileId = GetTileId(x, y, topLayer.Data, topLayer.Width, topLayer.Height);

                    if (!TopLayerIdIgnores.Contains(topTileId))
                    {
                        var xposition = x * TileWidth;
                        var yposition = y * TileHeight;

                        var textureVector2 = GetTexturePosition(topTileId, TileWidth, TileHeight, TileSetImageWidth);

                        var texturePostion = new Rectangle
                        {
                            Position = textureVector2,
                            Width = TileWidth,
                            Height = TileHeight,
                        };

                        TileData.Add(new(topLayer.Id, topTileId, texturePostion, new(xposition, yposition)));
                    }
                }
            }
        }

        private static int GetTileId(int x, int y, List<int> data, int Width, int Height)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return -1;

            int index = y * Width + x;
            return data[index];
        }

        private static Vector2 GetTexturePosition(int tileId, int mapWidth, int mapHeight, int textureWidth)
        {
            // Calculate the number of columns in the texture
            int cols = textureWidth / mapWidth;

            // start at 0 not 1
            // zero based index?, need to find out why I actually need this
            // but works so don't touch
            tileId--;

            // Calculate the column and row based on the image ID
            int col = tileId % cols;
            int row = tileId / cols;

            // Calculate the x and y coordinates
            int x = col * mapWidth;
            int y = row * mapHeight;

            return new(x, y);
        }

        private static async Task<T> DeserilizeResouceFromStream<T>(string path) where T : class
        {
            using var resourceStream = CurrentAssembly.GetManifestResourceStream(path);

            using var resourceStreamReader = new StreamReader(resourceStream);

            return JsonSerializer.Deserialize<T>(await resourceStreamReader?.ReadToEndAsync());
        }

        private static string CreateResourcePath(string name)
        {
            return $"Thaloria.Resources.{name}";
        }
    }

    /// Tiled class data
    public readonly struct TileData(int layerId, int tileId, Rectangle texturePos, Vector2 renderPos)
    {
        public readonly short LayerId = (short)layerId;
        public readonly short TileId = (short)tileId;
        public readonly Rectangle TexturePosition = texturePos;
        public readonly Vector2 RenderPosition = renderPos;
    }

    public class TiledExport
    {
        //[JsonPropertyName("compressionlevel")]
        //public int Compressionlevel { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        //[JsonPropertyName("infinite")]
        //public bool Infinite { get; set; }

        [JsonPropertyName("layers")]
        public List<Layer> Layers { get; set; }

        //[JsonPropertyName("nextlayerid")]
        //public int Nextlayerid { get; set; }

        //[JsonPropertyName("nextobjectid")]
        //public int Nextobjectid { get; set; }

        //[JsonPropertyName("orientation")]
        //public string Orientation { get; set; }

        //[JsonPropertyName("renderorder")]
        //public string Renderorder { get; set; }

        //[JsonPropertyName("tiledversion")]
        //public string Tiledversion { get; set; }

        [JsonPropertyName("tileheight")]
        public int Tileheight { get; set; }

        [JsonPropertyName("tilesets")]
        public List<Tileset> Tilesets { get; set; }

        [JsonPropertyName("tilewidth")]
        public int Tilewidth { get; set; }

        //[JsonPropertyName("type")]
        //public string Type { get; set; }

        //[JsonPropertyName("version")]
        //public string Version { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }

    public class Layer
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

        //[JsonPropertyName("type")]
        //public string Type { get; set; }

        //[JsonPropertyName("visible")]
        //public bool Visible { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        //[JsonPropertyName("x")]
        //public int X { get; set; }

        //[JsonPropertyName("y")]
        //public int Y { get; set; }
    }

    public class Tileset
    {
        //[JsonPropertyName("firstgid")]
        //public int Firstgid { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }
    }

    public class TileSetImage
    {
        //[JsonPropertyName("columns")]
        //public int Columns { get; set; }

        [JsonPropertyName("image")]
        public string ImageName { get; set; }

        [JsonPropertyName("imageheight")]
        public int Imageheight { get; set; }

        [JsonPropertyName("imagewidth")]
        public int Imagewidth { get; set; }

        //[JsonPropertyName("margin")]
        //public int Margin { get; set; }

        //[JsonPropertyName("name")]
        //public string Name { get; set; }

        //[JsonPropertyName("spacing")]
        //public int Spacing { get; set; }

        [JsonPropertyName("tilecount")]
        public int Tilecount { get; set; }

        //[JsonPropertyName("tiledversion")]
        //public string Tiledversion { get; set; }

        //[JsonPropertyName("tileheight")]
        //public int Tileheight { get; set; }

        //[JsonPropertyName("tilewidth")]
        //public int Tilewidth { get; set; }

        //[JsonPropertyName("type")]
        //public string Type { get; set; }

        //[JsonPropertyName("version")]
        //public string Version { get; set; }
    }
}
