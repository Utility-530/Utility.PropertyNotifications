using Splat;
using System;
using System.Reflection;
using System.Windows;
using Utility.Descriptors;
using Utility.Nodes.Reflections;
using Utility.Repos;
using Utility.Trees.Demo.MVVM.Infrastructure;
using Utility.Trees.Demo.MVVM.Views;

namespace Utility.Trees.Demo.MVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Guid guid = new Guid("5262625e-53c5-47c0-8351-a3be4111988e");

        public static Assembly[] Assemblies => new Assembly[] { typeof(Utility.WPF.Library.Class1).Assembly };
        public static Assembly[] SystemAssemblies => new Assembly[] { new Utility.Meta.SystemAssembly() };


        static App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            SQLitePCL.Batteries.Init();
            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(PipeRepository.Instance2);
            Locator.CurrentMutable.RegisterConstant<PipeRepository>(PipeRepository.Instance2);
            Locator.CurrentMutable.RegisterLazySingleton<PipeController>(() => new ());
            Locator.CurrentMutable.RegisterLazySingleton<Model>(() => new ());

            //{
            //    var rootPropertyDescriptor = new RootDescriptor(typeof(Model), name: "model");
            //    DescriptorFactory.CreateRoot(rootPropertyDescriptor, guid).Wait();
            //}

            Initialise();
            //InitialiseModelOld();
            var window = new Window { Content = MainView.Instance };
            window.Show();
            base.OnStartup(e);


            // ViewModel

        }
    }
}
