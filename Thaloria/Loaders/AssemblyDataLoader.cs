using System.Reflection;
using System.Text.Json;

namespace Thaloria.Loaders;

public static class AssemblyDataLoader
{
    public static string CreateMapResourcePath(string fileName)
    {
        return CreateResourcePath("Maps", fileName);
    }
    public static string CreateFontResourcePath(string fileName)
    {
        return CreateResourcePath("Fonts", fileName);
    }
    public static string CreateTilesetResourcePath(string fileName)
    {
        return CreateResourcePath("Tilesets", fileName);
    }
    public static MemoryStream? GetResourceStream(string path)
    {
        using var resourceStream = Program.CurrentAssembly.GetManifestResourceStream(path);

        var stream = new MemoryStream();
        
        resourceStream.CopyTo(stream);

        return stream;
    }
    public static async Task<T?> DeserilizeResouceFromStream<T>(string path) where T : class
    {
        await using var resourceStream = Program.CurrentAssembly.GetManifestResourceStream(path);

        using var resourceStreamReader = new StreamReader(resourceStream);

        return JsonSerializer.Deserialize<T>(await resourceStreamReader.ReadToEndAsync());
    }
    private static string CreateResourcePath(string folder, string fileName)
    {
        var path = $"Thaloria.Resources.{folder}.{fileName}";
        
        if(Program.CurrentAssembly.GetManifestResourceNames().Contains(path))
        {
            return path;
        }
        throw new ResourceNotFoundExeception($"{path} Could not be found in the assembly.");
    }
}

internal class ResourceNotFoundExeception : Exception
{
    public ResourceNotFoundExeception(string message) : base(message)
    { }
}