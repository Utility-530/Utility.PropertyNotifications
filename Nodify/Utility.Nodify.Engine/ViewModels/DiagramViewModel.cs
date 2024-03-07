//using Autofac;
using DryIoc;
using Utility.Nodify.Core;
using System.Linq;
using System.Windows;
using System;
using System.Collections.ObjectModel;
using Utility.Nodify.Operations.Infrastructure;
using Utility.Nodify.Engine.Infrastructure;
using MenuItem = Utility.Nodify.Operations.Infrastructure.MenuItem;

namespace Utility.Nodify.Engine.ViewModels
{
    public class DiagramViewModel : Core.DiagramViewModel
    {
        private readonly IContainer container;
        private MenuViewModel menu;
        private INodeSource operations => container.Resolve<INodeSource>();

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
                    menu = new MenuViewModel() { Items = new RangeObservableCollection<MenuItemViewModel>() };
                    menu.Subscribe(a => OperationsMenu_MenuItemSelected(a.Item1, a.Item2));
                }
                return menu;
            }
        }

        protected void OperationsMenu_MenuItemSelected(Point location, MenuItemViewModel menuItem)
        {
            var _menuItem = new MenuItem(menuItem.Content, menuItem.Guid);
            var node = operations.Find(_menuItem);
            INodeViewModel nodeViewModel = container.Resolve<IConverter>().Convert(node);
            nodeViewModel.Location = location;
            Nodes.Add(nodeViewModel);

            var pending = PendingConnection;
            if (pending.IsVisible)
            {
                var connector = pending.Source.IsInput ? nodeViewModel.Output.FirstOrDefault() : nodeViewModel.Input.FirstOrDefault();
                if (connector != null && CanCreateConnection(pending.Source, connector))
                {
                    CreateConnection(pending.Source, connector);
                }
            }
        }

        protected override void CreateConnection(IConnectorViewModel source, IConnectorViewModel? target)
        {
            if (target == null)
            {
                PendingConnection.IsVisible = true;
                OpenAt(PendingConnection.TargetLocation);
                Menu.Closed += OnOperationsMenuClosed;
                return;
            }

            var input = source.IsInput ? source : target;
            var output = target.IsInput ? source : target;

            PendingConnection.IsVisible = false;
            DisconnectConnector(input);
            var connectionViewModel = new ConnectionViewModel
            {
                Input = input,
                Output = output
            };
            container.RegisterInstanceMany<IConnectionViewModel>(connectionViewModel);
            Connections.Add(connectionViewModel);
        }

        protected void OnOperationsMenuClosed()
        {
            PendingConnection.IsVisible = false;
            Menu.Closed -= OnOperationsMenuClosed;
        }

        public void OpenAt(Point mouseLocation)
        {
            menu.Items.Clear();
            if (PendingConnection.Source != null)
            {
                var x = operations
                    .Filter(PendingConnection.Source.IsInput, PendingConnection.Source.Type)
                    .Select(a => new MenuItemViewModel() { Content = a.Key, Guid = a.Guid })
                    .ToArray();

                foreach (var y in x)
                    menu.Items.Add(y);
            }
            else
            {
                var x = operations
                    .Filter(null, null)
                    .Select(a => new MenuItemViewModel() { Content = a.Key, Guid = a.Guid })
                    .ToArray();

                foreach (var y in x)
                    menu.Items.Add(y);
            }
            menu.OpenAt(mouseLocation);
        }

    }





    public record PendingConnector
    {
        public bool IsInput { get; set; }
        public Type Type { get; set; }
    }
}
