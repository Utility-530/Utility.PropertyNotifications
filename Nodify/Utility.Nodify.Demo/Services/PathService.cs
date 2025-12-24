using Utility.Models;
using Utility.Services.Meta;

namespace Utility.Nodify.Demo.Services
{

    public record BasePathParam() : Param<PathService>(nameof(PathService.FullPath), "basePath");
    public record FilePathParam() : Param<PathService>(nameof(PathService.FullPath), "fileName");
    public record FullPathParam() : Param<PathService>(nameof(PathService.FullPath));


    public class PathService
    {
        public static string FullPath(string basePath, string fileName)=> System.IO.Path.Combine(basePath, fileName);
    }
}
