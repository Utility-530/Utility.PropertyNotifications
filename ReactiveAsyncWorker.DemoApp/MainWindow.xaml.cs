using ReactiveAsyncWorker.DemoApp.Infrastructure;
using ReactiveAsyncWorker.DemoApp.ViewModel;
using ReactiveAsyncWorker.Wpf.View;
using ReactiveUI;
using Splat;
using System.Windows;

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

            var defaultViewLocator = Locator.Current.GetService<ReactiveUI.IViewLocator>();
            Locator.CurrentMutable.RegisterLazySingleton<IViewLocator>(() => new ConventionBasedViewLocator(defaultViewLocator, new[] { typeof(CollectionView) }, typeof(DefaultView)));

            FactoryViewModelViewHost.ViewModel = Locator.Current.GetService<DemoFactoryViewModel>();
            TPLViewModelViewHost.ViewModel = Locator.Current.GetService<DemoTPLViewModel>();
    
        }
    }
}

