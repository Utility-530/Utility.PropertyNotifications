using System.IO;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Meta;
using Utility.Repos;
using Utility.Services.Meta;

namespace Utility.Nodes.Demo.Filters.Services
{
    public record ComboServiceInputParam() : Param<ComboService>(nameof(ComboService.Change), "dataFileModel");
    public record ComboServiceOutputParam() : Param<ComboService>(nameof(ComboService.Change));

    public class ComboService
    {
        public static IObservable<INodeViewModel> Change(DataFileModel dataFileModel)
        {
            return new NodeEngine(new TreeRepository(Path.Combine(dataFileModel.FilePath, dataFileModel.FileName + ".sqlite")))
                .Create(dataFileModel.TableName, dataFileModel.Guid, s => new ProliferationModel() { Name = s });
        }      
    }
}