using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utility.PropertyNotifications;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for NodesUserControl.xaml
    /// </summary>
    public partial class NodesUserControl : UserControl
    {
        public NodesUserControl()
        {
            InitializeComponent();
            this.Node.DataContext = new NodeViewModel();
        }



    }

    public class NodeViewModel
    {
        public IEnumerable Input { get; } = new ConnectorViewModel[] {  new ConnectorViewModel { Title = "1" },new ConnectorViewModel { Title = "1" }, new ConnectorViewModel { Title = "3" } };
        public IEnumerable Output { get; } = new ConnectorViewModel[] { new ConnectorViewModel { Title = "1" }, new ConnectorViewModel { Title = "1" }, new ConnectorViewModel { Title = "3" } };
    }

    public class ConnectorViewModel : NotifyPropertyClass
    {
        private string? _title;
        public string? Title
        {
            get => _title;
            set => this.RaisePropertyChanged(ref _title, value);
        }

        private double _value;
        public double Value
        {
            get => _value;
            set => this.RaisePropertyChanged(ref _value, value);
                //.Then(() => ValueObservers.ForEach(o => o.Value = value));
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => this.RaisePropertyChanged(ref _isConnected, value);
        }

        private bool _isInput;
        public bool IsInput
        {
            get => _isInput;
            set => this.RaisePropertyChanged(ref _isInput, value);
        }

        private Point _anchor;
        public Point Anchor
        {
            get => _anchor;
            set => this.RaisePropertyChanged(ref _anchor, value);
        }

        //private OperationViewModel _operation = default!;
        //public OperationViewModel Operation
        //{
        //    get => _operation;
        //    set => SetProperty(ref _operation, value);
        //}

        public List<ConnectorViewModel> ValueObservers { get; } = new List<ConnectorViewModel>();
    }
}
