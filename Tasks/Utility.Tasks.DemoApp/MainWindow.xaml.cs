using Utility.Tasks.DemoApp.Infrastructure;
using Utility.Tasks.DemoApp.ViewModel;
using Utility.Tasks.View;
using ReactiveUI;
using Splat;
using System.Windows;
using Utility.View;

namespace DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
 
            InitializeComponent();


            FactoryViewModelViewHost.ViewModel = Locator.Current.GetService<DemoFactoryViewModel>();
            TPLViewModelViewHost.ViewModel = Locator.Current.GetService<DemoTPLViewModel>();
    
        }
    }
}

