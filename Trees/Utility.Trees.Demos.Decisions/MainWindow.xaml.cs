using Splat;
using System.Windows;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.Pipes;
using Utility.Trees.Decisions;

namespace Utility.Trees.Demos.Decisions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DecisionTreeX dTree;

        public MainWindow()
        {
            InitializeComponent();
            dTree = new DecisionTreeX(new Decision<decimal>(a => a > 1), Guid.NewGuid().ToString(),
            a =>
            {
                return a / 2m;
            })

            {
                new DecisionTreeX(new Decision<decimal>(a => a > 5), Guid.NewGuid().ToString(),a =>
            {
                return a;
            }),
                new DecisionTreeX(new Decision<decimal>(a => a > 3), Guid.NewGuid().ToString(), a =>
            {
                return a;
            })
            };

            Tree_View.ItemsSource = new[] { dTree };

            Queue_View.Content = Pipe.Instance;
            Splat.Locator.CurrentMutable.RegisterLazySingleton<PipeController>(() => new());
            Splat.Locator.CurrentMutable.RegisterConstant(dTree);


            Pipe_View.Content = Splat.Locator.Current.GetService<PipeController>();


            this.Loaded += MainWindow_Loaded;
            Spinner_Control.ValueChanged += Spinner_Control_ValueChanged;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Spinner_Control.Value = 4;
        }

        private void Spinner_Control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            dTree.Input = (decimal)e.NewValue;
            Pipe.Instance.Queue(new ForwardItem(dTree, dTree.Input, new List<string>()));
        }
    }


 

 
}