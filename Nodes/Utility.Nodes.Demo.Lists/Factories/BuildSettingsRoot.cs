using Utility.Entities.Comms;
using Utility.Enums;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Meta;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        public INodeViewModel BuildSettingsRoot()
        {
            return 
                new Model(() =>
                [
                    new Model(() => [new CommandModel<ResetEvent> { Name = Reset }],
                    attach : n => {
                        n.IsExpanded = true;
                        n.Orientation = Enums.Orientation.Horizontal;
                    })
                    { Name = controls }])
                {
                    Name = nameof(BuildSettingsRoot),
                    IsExpanded = true,
                    Guid = settingsRootGuid,
                    Orientation = Orientation.Vertical
                };
        }
    }
}