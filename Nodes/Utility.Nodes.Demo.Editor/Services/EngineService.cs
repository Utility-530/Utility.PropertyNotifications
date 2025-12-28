using System.IO;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Meta;
using Utility.Repos;
using Utility.Services.Meta;
using Utility.ServiceLocation;

namespace Utility.Nodes.Demo.Filters.Services
{
    public record EngineServiceInputParam() : Param<EngineService>(nameof(EngineService.Change), "dataFileModel");
    public record EngineServiceOutputParam() : Param<EngineService>(nameof(EngineService.Change));

    public class EngineService
    {
        private static readonly Dictionary<Guid, NodeEngine> dictionary = [];

        public static INodeRoot Change((Guid guid, string path) dataFile)
        {
            var engine = dictionary.Get(dataFile.guid, a =>
            {
                var engine = new NodeEngine(new TreeRepository(dataFile.path), new ValueRepository(Path.Combine(dataFile.path)));
                Globals.Register.Register<INodeRoot>(engine);  
                return engine;
            });
            return engine;
        }
    }

}