using Splat;
using System.Reactive.Linq;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Editor;
using Utility.Nodes.Meta;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;
using Utility.Services.Meta;

namespace Utility.Nodes.Demo.Filters.Services
{
    public record ComboServiceInputParam() : Param<ComboService>(nameof(ComboService.Change), "dataFileModel");
    public record ComboServiceOutputParam() : Param<ComboService>(nameof(ComboService.Change));


    public class ComboService
    {
        public static DataFileModel Change(DataFileModel dataFileModel)
        {
            return dataFileModel;
        }
    }
}