using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Utility.Trees.Extensions.Async;
using Utility.Pipes;
using Utility.Trees.Abstractions;

namespace Utility.Trees.Demo.Connections
{
    public record ConnectionQueueItem(ConnectionModel Connection, Service Service, Change Change) : QueueItem
    {
        public override void Invoke()
        {
            Service.OnNext(new Change(Connection.ParameterName, Change.Value));
        }
    }
    
    public record TreeQueueItem(IReadOnlyTree Tree, object Value) : QueueItem
    {
        public override void Invoke()
        {
            (Tree.Data as ViewModel).Value = Value;
            (Tree.Data as ViewModel).RaisePropertyChanged(nameof(ViewModel.Value));
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModelTree tree = getTree();
            List<Service> ms = typeof(Methods).ToServices().ToList();

            ConnectionsViewModel _viewModel = new ()
            {
                ServiceModel = ms,
                Tree = tree
            };

            tree.Subscribe(t =>
            {
           
                foreach (var connection in _viewModel.Connections.Where(a => a.ViewModelName == t.Name && a.Movement == Movement.FromViewModelToService))
                {
                    var serviceName = connection.ServiceName;
                    var service = _viewModel.ServiceModel.Single(a => a.Name == serviceName);                    
                    Pipe.Instance.Queue(new ConnectionQueueItem(connection, service, t));
                }
            });

            pipe_view.Content = Locator.Current.GetService<PipeController>();
            queue_view.Content = Pipe.Instance;

            foreach (var service in ms)
            {
                service.Subscribe(a =>
                {
                    foreach (var connection in _viewModel.Connections.Where(a => a.ServiceName == service.Name && a.Movement == Movement.ToViewModelFromService))
                    {
                        var viewModelName = connection.ViewModelName;
                        tree.Descendant(a => (a.tree.Data as ViewModel).Name == viewModelName)
                        .Subscribe(viewModel =>
                        {
                            //(viewModel.Data as ViewModel).Value = a;
                            //(viewModel.Data as ViewModel).RaisePropertyChanged(nameof(ViewModel.Value));

                            Pipe.Instance.Queue(new TreeQueueItem(viewModel.NewItem, a));
                        });
                    }
                });
            }

            DataContext = _viewModel;
            this.Loaded += (s,e)=> MainWindow_Loaded(_viewModel);
        }

        private static ViewModelTree getTree()
        {
            return new ViewModelTree(new ViewModel() { Name = "root" },
                     new ViewModelTree(new ViewModel { Name = "A" },
                     new ViewModelTree(new ViewModel { Name = "B" }),
                     new ViewModelTree(new ViewModel { Name = "C" }),
                     new ViewModelTree(new ViewModel { Name = "D" })));
        }



        private void MainWindow_Loaded(ConnectionsViewModel  _viewModel)
        {
            //var x = new ConnectionEditor()
            var x = new ConnectionsService()
            {
                TreeView = TreeView,
                TreeView2 = TreeView2,
                viewModel = _viewModel,
                Container = Grid
            };
            x.Loaded();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
