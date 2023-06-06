using UniversalConverter.Diagnostics;
using Utility.Collections;
using Application = System.Windows.Application;
using Utility.Infrastructure;
using static Utility.Observables.Generic.ObservableExtensions;
using DryIoc;
using Resolver = Utility.PropertyTrees.Services.Resolver;
using Utility.PropertyTrees.WPF.Demo.Views;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class App : Application
    {
        private RootModelProperty rootModelProperty;
        //RootModel rootModel => (RootModel)rootModelProperty.Data;

        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            Collection.Context = BaseObject.Context = SynchronizationContext.Current ?? throw new System.Exception("sd w3w");

            var container = new BootStrapper().Build();

            var resolver = new Resolver(container);
            BaseObject.Resolver = resolver;
            resolver.Initialise();

            var main = container.Resolve<IModelController>();
            rootModelProperty = container.Resolve<RootModelProperty>();
            var treeView = new TreeView();

            var grid = new Grid();
            grid.RowDefinitions.AddRange(new[] {
                new RowDefinition { Height = new GridLength(30, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) } });

            main.TreeView(rootModelProperty, treeView)
                .Subscribe(treeView =>
            {
                //modelViewModel.IsConnected = true;
                //System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            });
            var subGrid = new Grid() { };
            var button = new CommandView();
            Grid.SetRow(subGrid, 1);
            grid.Children.Add(button);
            grid.Children.Add(subGrid);
            subGrid.Children.Add(treeView);
            //button.Click += Button_Click;
            var window = new Window { Content = grid };
            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(window, true);
            window.Show();

            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(controlWindow, true);

            //var window2 = new Window { Content = container.Resolve<ILogger>() };
            //window2.Show();


            base.OnStartup(e);

#if DEBUG
            Tracing.Enable();
#endif
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    //rootModel.Events.Add(new ServerEvent(ServerEventType.Data));
        //}
    }

    public record Request;
    public record RefreshRequest() : Request;
    public record ConnectRequest() : Request;
    public record ScreensaverRequest() : Request;
    public record PrizeWheelRequest() : Request;
    public record LeaderboardRequest() : Request;
}