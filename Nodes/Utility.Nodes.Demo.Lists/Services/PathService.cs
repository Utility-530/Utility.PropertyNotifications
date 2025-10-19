using Utility.Models;
using Utility.Services.Meta;

namespace Utility.Nodes.Demo.Lists.Services
{

    public record BasePathParam() :Param<PathService>(nameof(PathService.FullPath), "basePath");
    public record FilePathParam() :Param<PathService>(nameof(PathService.FullPath), "fileName");
    public record FullPathParam() :Param<PathService>(nameof(PathService.FullPath));


    public class PathService
    {
        public static string FullPath(string basePath, string fileName)=> System.IO.Path.Combine(basePath, fileName);
    }
}
