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
using System.Drawing;
using Utility.Nodify.Models;
using Utility.Nodify.Engine;
using Utility.Enums;
using Splat;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodify.ViewModels
{
    public class DiagramViewModel : Models.DiagramViewModel
    {
        private readonly IContainer container;
        private MenuViewModel menu;

        public int GridColumn = 1;
        private INodeSource operations
        {
            get
            {
                try
                {
                    return Splat.Locator.Current.GetService<INodeSource>();
                }
                catch (Exception ex)
                {
                    return null;

                }
            }
        }

        public DiagramViewModel(IContainer container) : base()
        {
            this.container = container;
        }

        public override string Key
        {
            get => base.Key;
            set
            {
                base.Key = value;
                container.Resolve<IDiagramFactory>().Build(this);
            }
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

        public Arrangement Arrangement { get; set; }

        protected void OperationsMenu_MenuItemSelected(PointF location, MenuItemViewModel menuItem)
        {
            var nodeViewModel = operations.Find(menuItem.Content);
            IConnectorViewModel connector = null;
            var pending = PendingConnection;

            if (menuItem.Content is IType { Type: { } type })
            {
                if (pending.IsVisible)
                {
                     connector = pending.Input.IsInput ? nodeViewModel.Output.FirstOrDefault() : nodeViewModel.Input.FirstOrDefault(a => (a.Data as IType)?.Type == type);
                }
            }

            if (pending.IsVisible)
            {
                connector = pending.Input.IsInput ? nodeViewModel.Output.FirstOrDefault() : nodeViewModel.Input.FirstOrDefault();
            }

            if (connector != null && CanCreateConnection(pending.Input, connector))
            {
                CreateConnection(pending.Input, connector);
                nodeViewModel.Location = location;
                Nodes.Add(nodeViewModel);
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

        public void OpenAt(PointF mouseLocation)
        {
            menu.Items.Clear();
            if (PendingConnection.Input != null)
            {
                var x = operations?
                    .Filter(PendingConnection)
                    .Select(a => new MenuItemViewModel() { Content = a, Guid = a.Guid })
                    .ToArray();
                if (x == null)
                    return;
                foreach (var y in x)
                    menu.Items.Add(y);
            }
            else
            {
                var x = operations
                    .Filter(null)
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
