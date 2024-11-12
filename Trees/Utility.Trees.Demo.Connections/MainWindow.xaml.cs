using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Extensions;

namespace Utility.Trees.Demo.Connections
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConnectionsViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            var tree = new ViewModelTree(new ViewModel() { Name = "root" },
                     new ViewModelTree(new ViewModel { Name = "A" },
                     new ViewModelTree(new ViewModel { Name = "B" }),
                     new ViewModelTree(new ViewModel { Name = "C" }),
                     new ViewModelTree(new ViewModel { Name = "D" })));

            TreeView.ItemsSource = tree.Items;

            var a = new Service { Name = "Service A", Func = new Func<object, object>(a => a) };
            var b = new Service { Name = "Service B", Func = new Func<object, object>(a => a) };

            _viewModel = new ConnectionsViewModel()
            {
                ServiceModel = new() { a, b },
                Tree = tree
            };


            tree.Subscribe(t =>
            {
                foreach (var connection in _viewModel.Connections.Where(a => a.ViewModelName == t.Name && a.Movement == Movement.FromViewModelToService))
                {
                    if (connection == null)
                        return;
                    var serviceName = connection.ServiceName;
                    var service = _viewModel.ServiceModel.Single(a => a.Name == serviceName);
                    service.OnNext(t.Value);
                }
            });

            a.Subscribe(a =>
            {
                foreach (var connection in _viewModel.Connections.Where(a => a.ServiceName == "Service A" && a.Movement == Movement.ToViewModelFromService))
                {
                    var viewModelName = connection.ViewModelName;
                    var viewModel = tree.MatchDescendant(a => (a.Data as ViewModel).Name == viewModelName);
                    (viewModel.Data as ViewModel).Value = a;
                    (viewModel.Data as ViewModel).RaisePropertyChanged(nameof(ViewModel.Value));
                }
            });

            b.Subscribe(b =>
            {
                foreach (var connection in _viewModel.Connections.Where(a => a.ServiceName == "Service B" && a.Movement == Movement.ToViewModelFromService))
                {
                    var viewModelName = connection.ViewModelName;
                    var viewModel = tree.MatchDescendant(a => (a.Data as ViewModel).Name == viewModelName);
                    (viewModel.Data as ViewModel).Value = b;
                    (viewModel.Data as ViewModel).RaisePropertyChanged(nameof(ViewModel.Value));
                }
            });

            DataContext = _viewModel;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var x = new Connector()
            {
                TreeView = TreeView,
                TreeView2 = TreeView2,
                viewModel = _viewModel,
                Container = this
            };
            x.Loaded();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
