using Splat;
using System;
using System.Reflection;
using System.Windows;
using Utility.Descriptors;
using Utility.Descriptors.Repositorys;
using Utility.Nodes.Reflections;
using Utility.Trees.Demo.MVVM.Infrastructure;

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

        ReflectionNode model;
        //ViewModelTree viewModel;
        //Tree view;
        //Tree data;

        static App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            SQLitePCL.Batteries.Init();
            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(TreeRepository.Instance);

            //{
            //    var rootPropertyDescriptor = new RootDescriptor(typeof(Model), name: "model");
            //    DescriptorFactory.CreateRoot(rootPropertyDescriptor, guid).Wait();
            //}

            Window window = CreateWindow();
            window.Show();
            base.OnStartup(e);


            // ViewModel
        
        }
    }
}
