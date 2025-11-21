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

        public static INodeSource Change(Model<string> dataFileModel)
        {
            var engine = dictionary.Get(dataFileModel.Guid, a =>
            {
                var engine = new NodeEngine(new TreeRepository(Path.Combine(dataFileModel.Value + ".sqlite")));
                Globals.Register.Register<INodeSource>(engine);  
                return engine;
            });
            return engine;
        }
    }

}