using System.Collections.ObjectModel;
using System.Drawing;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;
using MoreLinq;
using Utility.Collections;
using Utility.Commands;
using Utility.Enums;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Base.Abstractions;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.ServiceLocation;
using Utility.Interfaces;
using Splat;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;

namespace Utility.Nodes
{
    public class DiagramViewModel : NodeViewModel, IDiagramViewModel
    {
        private INodeViewModel menu;
        private RangeObservableCollection<INodeViewModel> _selectedOperations = [];
        private RangeObservableCollection<IConnectionViewModel> connections = new();

        public int GridColumn = 1;
        private PendingConnectionViewModel pendingConnection = new PendingConnectionViewModel();

      
        public DiagramViewModel()
        {
            Connections = connections;

            // Initialize commands
            CreateConnectionCommand = new Command<IConnectorViewModel>(
                _ => CreateConnection(PendingConnection.Output, PendingConnection.Input),
                _ => Globals.Resolver.Resolve<IViewModelFactory>().CanCreateConnection(PendingConnection.Output, PendingConnection.Input));
            StartConnectionCommand = new Command<object>(_ => pendingConnection.IsVisible = true);
            DisconnectConnectorCommand = new Command<IConnectorViewModel>(DisconnectConnector);
            DeleteSelectionCommand = new Command(DeleteSelection);
            GroupSelectionCommand = new Command(GroupSelectedOperations, () => SelectedNodes.Count > 0);

            // Setup collection behaviors
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

            if (Nodes is ThreadSafeObservableCollection<INodeViewModel> _threadSafe)
                _threadSafe.WhenAdded(x =>
                {
                    if (x.Inputs is ThreadSafeObservableCollection<IConnectorViewModel> _tt_)
                        _tt_.WhenRemoved(RemoveConnection);

                    void RemoveConnection(IConnectorViewModel i)
                    {
                        var c = Connections.Where(con => con.Input == i || con.Output == i).ToArray();
                        c.ForEach(con => connections.Remove(con));
                    }
                })
                .WhenRemoved(x =>
                {
                    foreach (var input in x.Inputs)
                    {
                        DisconnectConnector(input);
                    }

                    if (x.Outputs.Any())
                    {
                        foreach (var output in x.Outputs)
                        {
                            DisconnectConnector(output);
                        }
                    }
                });
        }

        public INodeViewModel Menu
        {
            get
            {
                if (menu == null)
                {
                    //menu = new MenuViewModel() { Items = new RangeObservableCollection<IMenuViewModel>() };
                    var menuFactory = Globals.Resolver.Resolve<IMenuFactory>();
                    menu = Globals.Resolver.Resolve<IMenuFactory>().CreateMenu();
                    menuFactory
                        .Subscribe(a => OperationsMenu_MenuItemSelected(a.Item1, a.Item2));
                }
                return menu;
            }
        }

        public ICollection<INodeViewModel> SelectedNodes => _selectedOperations;

        public override ICollection<IConnectionViewModel> Connections { get; set; }

        public IConnectionViewModel PendingConnection { get => pendingConnection; set => pendingConnection = value as PendingConnectionViewModel; }
        public ICommand StartConnectionCommand { get; }
        public ICommand CreateConnectionCommand { get; }
        public ICommand DisconnectConnectorCommand { get; }
        public ICommand DeleteSelectionCommand { get; }
        public ICommand GroupSelectionCommand { get; }

        protected void OperationsMenu_MenuItemSelected(PointF location, object menuItem)
        {
            var nodeViewModel = Globals.Resolver.Resolve<IFactory<INodeViewModel>>().Create(menuItem);

            IConnectorViewModel connector = null;
            var pending = PendingConnection;

            if (menuItem is IType { Type: { } type })
            {
                if (pendingConnection.IsVisible)
                {
                    connector = nodeViewModel.Inputs.FirstOrDefault(a => a is IGetData { Data: ParameterInfo { ParameterType: Type { } _type } }? _type == type: throw new Exception(" Sdsd"));
                }
            }
            else
            {
                throw new Exception("R 44d s");
            }

            if (pending.Output != null && Globals.Resolver.Resolve<IViewModelFactory>().CanCreateConnection(pending.Output, connector))
            {
                CreateConnection(pending.Output, connector);
                nodeViewModel.Location = location;
                Nodes.Add(nodeViewModel);
            }
            menu.Close();
        }

        protected virtual void CreateConnection(IConnectorViewModel source, IConnectorViewModel? target)
        {
            if (target == null)
            {
                pendingConnection.IsVisible = true;
                OpenAt(pendingConnection.TargetLocation);
                Menu.Closed += OnOperationsMenuClosed;
                return;
            }

            if (target is PendingConnectorViewModel pending)
            {
                pending.IsDropDownOpen = true;
                if (source is IGetData { Data: ParameterInfo { ParameterType: Type type } })
                {
                    pending.Data = type;
                }
                else if (source is IGetData { Data: PropertyInfo { PropertyType: { } _type } })
                {
                    pending.Data = _type;
                }
                pending.Node.Inputs.Additions<ConnectorViewModel>().Take(1).Subscribe(connector =>
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

            pendingConnection.IsVisible = false;
            DisconnectConnector(input);

            var connectionViewModel = Globals.Resolver.Resolve<IViewModelFactory>().CreateConnection(output, input);
            //Globals.Resolver.RegisterInstanceMany(connectionViewModel);
            connections.Add(connectionViewModel);
        }

        protected void OnOperationsMenuClosed()
        {
            pendingConnection.IsVisible = false;
            Menu.Closed -= OnOperationsMenuClosed;
        }

        public override void OpenAt(PointF mouseLocation)
        {
            menu.Clear();
            var operations = Globals.Resolver.Resolve<IEnumerableFactory>();

            if (PendingConnection.Output != null)
            {
                var x = operations?
                    .Create(PendingConnection)
                    .Cast<object>()
                    .Select(a => Globals.Resolver.Resolve<IMenuFactory>().Create(a))
                    .ToArray();
                if (x == null)
                    return;
                foreach (var y in x)
                    menu.Add(y);
            }
            else if (operations != null)
            {
                var x = operations
                    .Create(null)
                    .Cast<object>()                
                    .Select(a => Globals.Resolver.Resolve<IMenuFactory>().Create(a))
                    .ToArray();

                foreach (var y in x)
                    menu.Add(y);
            }
            menu.OpenAt(mouseLocation);
        }

        protected void DisconnectConnector(IConnectorViewModel connector)
        {
            var connections = Connections.Where(c => c.Input == connector || c.Output == connector).ToList();
            connections.ForEach(c => connections.Remove(c));
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