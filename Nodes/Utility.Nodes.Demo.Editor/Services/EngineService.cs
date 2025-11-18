using System.IO;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Models.Trees;
using Utility.Nodes.Meta;
using Utility.Repos;
using Utility.Services.Meta;

namespace Utility.Nodes.Demo.Filters.Services
{
    public record EngineServiceInputParam() : Param<EngineService>(nameof(EngineService.Change), "dataFileModel");
    public record EngineServiceOutputParam() : Param<EngineService>(nameof(EngineService.Change));

    public class EngineService
    {
        static Dictionary<Guid, NodeEngine> dictionary = new();


        public static INodeSource Change(DataFileModel dataFileModel)
        {
            var engine = dictionary.Get(dataFileModel.Guid, a =>
            new NodeEngine(new TreeRepository(Path.Combine(dataFileModel.FilePath, dataFileModel.FileName + ".sqlite")))
            );
            return engine;
        }
    }

}