//using Autofac;
using DryIoc;
using Utility.Nodify.Core;
using Utility.Nodify.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace Utility.Nodify.Demo.ViewModels
{
    public class OperationsEditorViewModel : EditorViewModel
    {
        private readonly IContainer container;

        private IDictionary<string, OperationInfo> operations => container.Resolve<IDictionary<string, OperationInfo>>(Keys.Operations);

        public OperationsEditorViewModel(IContainer container) : base(container.Resolve<RangeObservableCollection<Diagram>>(Keys.SelectedDiagram).FirstOrDefault())
        {
            this.container = container;
        }

        protected override IEnumerable<MenuItemViewModel> MenuItems => container.Resolve<IEnumerable<MenuItemViewModel>>();

        protected override void OperationsMenu_Selected(Point location, MenuItemViewModel menuItem)
        {
            NodeViewModel op = OperationFactory.CreateNode(operations[menuItem.Content]);
            op.Location = location;

            Nodes.Add(op);

            var pending = PendingConnection;
            if (pending.IsVisible)
            {
                var connector = pending.Source.IsInput ? op.Output.FirstOrDefault() : op.Input.FirstOrDefault();
                if (connector != null && CanCreateConnection(pending.Source, connector))
                {
                    CreateConnection(pending.Source, connector);
                }
            }
        }


        protected override void CreateConnection(ConnectorViewModel source, ConnectorViewModel? target)
        {
            if (target == null)
            {
                PendingConnection.IsVisible = true;
                Menu.OpenAt(PendingConnection.TargetLocation);
                Menu.Closed += OnOperationsMenuClosed;
                return;
            }

            var input = source.IsInput ? source : target;
            var output = target.IsInput ? source : target;

            PendingConnection.IsVisible = false;

            DisconnectConnector(input);

            Connections.Add(new OperationConnectionViewModel
            {
                Input = input,
                Output = output
            });
        }
    }
}
