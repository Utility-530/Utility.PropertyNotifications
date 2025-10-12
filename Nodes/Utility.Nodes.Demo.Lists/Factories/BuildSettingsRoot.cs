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


        public IObservable<INodeViewModel> BuildSettingsRoot()
        {
            return nodeSource.Create(nameof(BuildSettingsRoot),
                settingsRootGuid,
                str =>

                new Model(() =>
                [
                    new Model(() => [new CommandModel<ResetEvent> { Name = Reset }],
                    n => {
                        n.IsExpanded = true;
                        n.Orientation = Enums.Orientation.Horizontal;
                    })
                    { Name = controls }])
                {
                    Name = str,
                    IsExpanded = true,
                    Orientation = Orientation.Vertical
                });

        }
    }
}
