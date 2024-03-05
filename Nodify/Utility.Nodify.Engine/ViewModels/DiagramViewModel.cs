//using Autofac;
using DryIoc;
using Utility.Nodify.Core;
using Utility.Nodify.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using Utility.Nodify.Operations.Infrastructure;
using Utility.Nodify.Demo.Infrastructure;

namespace Utility.Nodify.Demo.ViewModels
{
    public class DiagramViewModel : Core.DiagramViewModel
    {
        private readonly IContainer container;
        private MenuViewModel menu;

       // private IDictionary<string, OperationInfo> operations => container.Resolve<IDictionary<string, OperationInfo>>(Keys.Operations);
       private INodeSource operations => container.Resolve<INodeSource>(Keys.Operations);

        public DiagramViewModel(IContainer container) : base()
        {
            this.container = container;
        }

        public MenuViewModel Menu
        {
            get
            {
                if (menu == null)
                {
                    menu = new MenuViewModel() { Items = new RangeObservableCollection<MenuItemViewModel>(MenuItems) };
                    menu.Selected += OperationsMenu_Selected;
                }
                return menu;
            }
        }

        protected IEnumerable<MenuItemViewModel> MenuItems => container.Resolve<IEnumerable<MenuItemViewModel>>();

        protected void OperationsMenu_Selected(Point location, MenuItemViewModel menuItem)
        {

            NodeViewModel op = container.Resolve<Converter>().Convert(operations.Find(menuItem.Content));
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

        protected void OnOperationsMenuClosed()
        {
            PendingConnection.IsVisible = false;
            Menu.Closed -= OnOperationsMenuClosed;
        }

    }
}
