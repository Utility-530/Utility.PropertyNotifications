using Utility.Models;

namespace Utility.Nodes.Demo.Lists.Services
{

    public record BasePathParam() : MethodParameter<PathService>(nameof(PathService.FullPath), "basePath");
    public record FilePathParam() : MethodParameter<PathService>(nameof(PathService.FullPath), "fileName");
    public record FullPathParam() : MethodParameter<PathService>(nameof(PathService.FullPath));


    public class PathService
    {
        public static string FullPath(string basePath, string fileName)=> System.IO.Path.Combine(basePath, fileName);
    }
}
