using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using Utility.Descriptors.Repositorys;
using Utility.Extensions;
using Utility.Keys;
using Utility.Trees.Demo.MVVM.Infrastructure;
using Utility.WPF.Templates;

namespace Utility.Trees.Demo.MVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Assembly[] Assemblies => new Assembly[] { typeof(Utility.WPF.Library.Class1).Assembly };
        public static Assembly[] SystemAssemblies => new Assembly[] { new Utility.Meta.SystemAssembly() };

        RootNode model;
        ViewModelTree viewModel;
        Tree view;
        Tree data;

        static App()
        {

        }

        protected override void OnStartup(StartupEventArgs e)
        {

            SQLitePCL.Batteries.Init();
            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(TreeRepository.Instance);
         
            Window window = CreateWindow();
            window.Show();
            base.OnStartup(e);


            // ViewModel
        
        }
    }
}
