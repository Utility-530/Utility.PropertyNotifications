using DryIoc;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using Utility.Nodify.Operations.Infrastructure;
using System.Drawing;
using Utility.Enums;
using Splat;
using Utility.Interfaces.NonGeneric;
using Utility.Trees;
using Utility.Reactives;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Nodify.Base.Abstractions;
using MoreLinq;
using System.Collections.Generic;
using System.Windows.Input;
using Utility.Collections;
using Utility.Commands;
using Utility.PropertyNotifications;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Nodes
{
    public class DiagramViewModel : NotifyPropertyClass, IDiagramViewModel
    {
        private readonly IContainer container;
        private IMenuViewModel menu;
        private RangeObservableCollection<INodeViewModel> nodes = [];
        private RangeObservableCollection<INodeViewModel> _selectedOperations = [];

        public int GridColumn = 1;

        private INodeSource operations
        {
            get
            {
                try
                {
                    return container.Resolve<INodeSource>();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public DiagramViewModel(IContainer container)
        {
            this.container = container;

            // Initialize commands
            CreateConnectionCommand = new Command<IConnectorViewModel>(
                _ => CreateConnection(PendingConnection.Output, PendingConnection.Input),
                _ => container.Resolve<IViewModelFactory>().CanCreateConnection(PendingConnection.Output, PendingConnection.Input));
            StartConnectionCommand = new Command<object>(_ => PendingConnection.IsVisible = true);
            DisconnectConnectorCommand = new Command<IConnectorViewModel>(DisconnectConnector);
            DeleteSelectionCommand = new Command(DeleteSelection);
            GroupSelectionCommand = new Command(GroupSelectedOperations, () => SelectedNodes.Count > 0);

            // Setup collection behaviors
            nodes.WhenAdded(x => x.Diagram = this);

            if (Connections is ThreadSafeObservableCollection<IConnectionViewModel> threadSafe)
                threadSafe.WhenAdded(c =>
                {
                    c.Input.IsConnected = true;
                    c.Output.IsConnected = true;
                })
                .WhenRemoved(c =>
                {
                    var ic = Connections.Count(con => con.Input == c.Input || con.Output == c.Input);
                    var oc = Connections.Count(con => con.Input == c.Output || con.Output == c.Output);

                    if (ic == 0)
                    {
                        c.Input.IsConnected = false;
                    }

                    if (oc == 0)
                    {
                        c.Output.IsConnected = false;
                    }
                });

            if (nodes is ThreadSafeObservableCollection<INodeViewModel> _threadSafe)
                _threadSafe.WhenAdded(x =>
                {
                    if (x.Input is ThreadSafeObservableCollection<IConnectorViewModel> _tt_)
                        _tt_.WhenRemoved(RemoveConnection);

                    void RemoveConnection(IConnectorViewModel i)
                    {
                        var c = Connections.Where(con => con.Input == i || con.Output == i).ToArray();
                        c.ForEach(con => Connections.Remove(con));
                    }
                })
                .WhenRemoved(x =>
                {
                    foreach (var input in x.Input)
                    {
                        DisconnectConnector(input);
                    }

                    if (x.Output.Any())
                    {
                        foreach (var output in x.Output)
                        {
                            DisconnectConnector(output);
                        }
                    }
                });
        }

        public Guid Guid { get; set; }

        public virtual string Key { get; set; }

        public IMenuViewModel Menu
        {
            get
            {
                if (menu == null)
                {
                    //menu = new MenuViewModel() { Items = new RangeObservableCollection<IMenuViewModel>() };
                    menu = container.Resolve<IMenuFactory>().CreateMenu();
                    menu.Subscribe(a => OperationsMenu_MenuItemSelected(a.Item1, a.Item2));
                }
                return menu;
            }
        }

        public Arrangement Arrangement { get; set; }

        public ObservableCollection<INodeViewModel> Nodes => nodes;

        public ICollection<INodeViewModel> SelectedNodes => _selectedOperations;

        public ObservableCollection<IConnectionViewModel> Connections { get; } = new RangeObservableCollection<IConnectionViewModel>();

        public PendingConnectionViewModel PendingConnection { get; set; } = new PendingConnectionViewModel();

        public ICommand StartConnectionCommand { get; }
        public ICommand CreateConnectionCommand { get; }
        public ICommand DisconnectConnectorCommand { get; }
        public ICommand DeleteSelectionCommand { get; }
        public ICommand GroupSelectionCommand { get; }

        protected void OperationsMenu_MenuItemSelected(PointF location, IMenuItemViewModel menuItem)
        {
            var nodeViewModel = operations.Find(menuItem.Content);
            IConnectorViewModel connector = null;
            var pending = PendingConnection;

            if (menuItem.Content is IType { Type: { } type })
            {
                if (pending.IsVisible)
                {
                    connector = nodeViewModel.Input.FirstOrDefault(a => a is IGetData { Data: IType { Type: { } _type } } && _type == type);
                }
            }

            if (pending.IsVisible)
            {
                connector = nodeViewModel.Input.FirstOrDefault();
            }

            if (pending.Output != null && container.Resolve<IViewModelFactory>().CanCreateConnection(pending.Output, pending.Input))
            {
                CreateConnection(pending.Output, connector);
                nodeViewModel.Location = location;
                Nodes.Add(nodeViewModel);
            }
        }

        protected virtual void CreateConnection(IConnectorViewModel source, IConnectorViewModel? target)
        {
            if (target == null)
            {
                PendingConnection.IsVisible = true;
                OpenAt(PendingConnection.TargetLocation);
                Menu.Closed += OnOperationsMenuClosed;
                return;
            }

            if (target is PendingConnectorViewModel pending)
            {
                pending.IsDropDownOpen = true;
                if (source is IGetData { Data : ParameterInfo { ParameterType: Type type } })
                {
                    pending.Data = type;
                }
                else if (source is IGetData{ Data: PropertyInfo { PropertyType: { } _type } })
                {
                    pending.Data = _type;
                }
                pending.Node.Input.Additions<ConnectorViewModel>().Take(1).Subscribe(connector =>
                {
                    CreateConnection(source, connector);
                });
                return;
            }
            else if (target is ConnectorViewModel connector)
            {

            }

            var input = source.IsInput ? source : target;
            var output = target.IsInput ? source : target;

            PendingConnection.IsVisible = false;
            DisconnectConnector(input);


            var connectionViewModel = container.Resolve<IViewModelFactory>().CreateConnection(input, output);
            container.RegisterInstanceMany(connectionViewModel);
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
            if (PendingConnection.Output != null)
            {
                var x = operations?
                    .Filter(PendingConnection)
                    .Select(a => container.Resolve<IMenuFactory>().Create(a)) //new MenuItemViewModel() { Content = a, Guid = a.Guid });
                    .ToArray();
                if (x == null)
                    return;
                foreach (var y in x)
                    menu.Items.Add(y);
            }
            else if (operations != null)
            {
                var x = operations
                    .Filter(null)
           .Select(a => container.Resolve<IMenuFactory>().Create(a))
                    .ToArray();

                foreach (var y in x)
                    menu.Items.Add(y);
            }
            menu.OpenAt(mouseLocation);
        }

        protected void DisconnectConnector(IConnectorViewModel connector)
        {
            var connections = Connections.Where(c => c.Input == connector || c.Output == connector).ToList();
            connections.ForEach(c => Connections.Remove(c));
        }

        protected void DeleteSelection()
        {
            var selected = SelectedNodes.ToList();
            selected.ForEach(o => Nodes.Remove(o));
        }

        protected void GroupSelectedOperations()
        {
            //var selected = SelectedOperations.ToList();
            //var bounding = selected.GetBoundingBox(50);

            //Operations.Add(new OperationGroupViewModel
            //{
            //    Title = "Operations",
            //    Location = bounding.Location,
            //    GroupSize = new Size(bounding.Width, bounding.Height)
            //});
        }
    }
}