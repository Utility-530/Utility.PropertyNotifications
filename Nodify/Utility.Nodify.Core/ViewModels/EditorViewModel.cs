using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Utility.Collections;
using Utility.Commands;
using Utility.Models;

namespace Utility.Nodify.Core
{
    public class EditorViewModel : BaseViewModel
    {
        private RangeObservableCollection<NodeViewModel> _operations = new(), _messages = new();
        private RangeObservableCollection<NodeViewModel> _selectedOperations = new();
        private MenuViewModel menu;

        public EditorViewModel(Diagram diagram)
        {   

            CreateConnectionCommand = new Command<ConnectorViewModel>(
                _ => CreateConnection(PendingConnection.Source, PendingConnection.Target),
                _ => CanCreateConnection(PendingConnection.Source, PendingConnection.Target));
            StartConnectionCommand = new Command<object>(_ => PendingConnection.IsVisible = true);
            DisconnectConnectorCommand = new Command<ConnectorViewModel>(DisconnectConnector);
            DeleteSelectionCommand = new Command(DeleteSelection);
            GroupSelectionCommand = new Command(GroupSelectedOperations, () => SelectedNodes.Count > 0);

            Connections.WhenAdded(c =>
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

                //c.Output.ValueObservers.Remove(c.Input);
            });

            Nodes.WhenAdded(x =>
            {
                x.Input.WhenRemoved(RemoveConnection);

                if (x is InputNodeViewModel ci)
                {
                    ci.Output.WhenRemoved(RemoveConnection);
                }

                void RemoveConnection(ConnectorViewModel i)
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
     

            foreach (var node in diagram.Nodes)
                Nodes.Add(node);

            foreach (var connection in diagram.Connections)
                Connections.Add(connection);
        }


        protected virtual IEnumerable<MenuItemViewModel> MenuItems
        {
            get
            {
                yield break;
            }
        }


        protected virtual void OperationsMenu_Selected(Point location, MenuItemViewModel menuItem)
        {

        }

 
        //private void Op_InputChanged(NodeViewModel obj)
        //{
        //    Messages.Add(obj);
        //}


        public RangeObservableCollection<NodeViewModel> Nodes
        {
            get => _operations;
            set => Set(ref _operations, value);
        }

        public RangeObservableCollection<NodeViewModel> SelectedNodes
        {
            get => _selectedOperations;
            set => Set(ref _selectedOperations, value);
        }

        public RangeObservableCollection<ConnectionViewModel> Connections { get; } = new RangeObservableCollection<ConnectionViewModel>();
        public PendingConnectionViewModel PendingConnection { get; set; } = new PendingConnectionViewModel();
        public MenuViewModel Menu
        {
            get
            {
                if (menu == null)
                {
                    menu = new MenuViewModel();
                    menu.Selected += OperationsMenu_Selected;
                    menu.Items.AddRange(MenuItems);

                }
                return menu;
            }
        }

        public ICommand StartConnectionCommand { get; }
        public ICommand CreateConnectionCommand { get; }
        public ICommand DisconnectConnectorCommand { get; }
        public ICommand DeleteSelectionCommand { get; }
        public ICommand GroupSelectionCommand { get; }

        protected void DisconnectConnector(ConnectorViewModel connector)
        {
            var connections = Connections.Where(c => c.Input == connector || c.Output == connector).ToList();
            connections.ForEach(c => Connections.Remove(c));
        }

        protected bool CanCreateConnection(ConnectorViewModel source, ConnectorViewModel? target)
            => target == null || (source != target && source.Node != target.Node && source.IsInput != target.IsInput);

        protected virtual void CreateConnection(ConnectorViewModel source, ConnectorViewModel? target)
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

            Connections.Add(new ConnectionViewModel
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

