using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Nodes;

namespace Utility.Nodify.Sandbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }

    public class MainViewModel
    {

        public MainViewModel()
        {
            NodeViewModel nodeViewModel = new() { Inputs = new ObservableCollection<IConnectorViewModel>(), Outputs = new ObservableCollection<IConnectorViewModel>() };

            nodeViewModel.Outputs.Add(new ConnectorViewModel() { Key = "A", Data = new object() });
            nodeViewModel.Outputs.Add(new ConnectorViewModel() { Key = "C", Data = new object() });
            nodeViewModel.Inputs.Add(new ConnectorViewModel() { Key = "B", Data = new object() });
            nodeViewModel.Inputs.Add(new ConnectorViewModel() { Key = "B", Data = new object() });
            nodeViewModel.Inputs.Add(new ConnectorViewModel() { Key = "B", Data = new object() });
            nodeViewModel.Inputs.Add(new ConnectorViewModel() { Key = "B", Data = new object() });

            this.Node = nodeViewModel;
        }

        public NodeViewModel Node { get; }
    }

    public class Command : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object? parameter)
        {
      
        }

        public static ICommand Instance = new Command();
    }

    public class ConnectorTemplateSelector:DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item is PendingConnectorViewModel)
            {
                return PendingTemplate ;
            }
            return DefaultTemplate ?? base.SelectTemplate(item, container);
        }

        public DataTemplate? DefaultTemplate { get; set; }
        public DataTemplate? PendingTemplate { get; set; }

        public static ConnectorTemplateSelector Instance = new ConnectorTemplateSelector();
    }
}