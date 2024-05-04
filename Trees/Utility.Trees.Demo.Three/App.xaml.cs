using Splat;
using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using Utility.Descriptors.Repositorys;
using Utility.Extensions;
using Utility.Keys;
using Utility.Trees.Demo.MVVM.Infrastructure;

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

        static App()
        {
  
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            SQLitePCL.Batteries.Init();
            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(TreeRepository.Instance);
            model = new RootNode();
            model.Initialise().GetAwaiter().GetResult();
            viewModel = new() { Key = model.Key };
            view = new() { Key = model.Key };

            Window window = CreateWindow();
            window.Show();
            base.OnStartup(e);


            // ViewModel
            model.Subscribe((a =>
            {
                if (a.Type == Changes.Type.Add)
                {
                    var clone = a.Value.Clone();
                    var x = new ViewModelTree { Key = clone.Key };
                    var parentMatch = TreeExtensions.MatchDescendant(viewModel, (d => d.Key?.Equals(a.Value.Parent?.Key) == true)) as Tree;
                    if (parentMatch != null)
                        parentMatch.Add(x);
                }
                if (a.Type == Changes.Type.Remove)
                {
                    var match = TreeExtensions.MatchDescendant(viewModel, (d => d.Key.Equals(a.Value.Key))) as Tree;
                    match.Parent.Remove(a.Value);
                }
            }));


            // View
            model.Subscribe(a =>
            {
                if (a.Type == Changes.Type.Add)
                {
                    var clone = a.Value.Clone();
                    var guid = ((GuidKey)a.Value.Key)?.Value;
                    if (guid == null)
                        return;
                    var type = TreeRepository.Instance.GetType(guid.Value);
                    Tree tree = null;
                    if (type != null)
                    {
                        var instance = Activator.CreateInstance(type);
                        //var rootDescriptor = new RootDescriptor(type);
                        //var data = await DescriptorFactory.ToValue(instance, rootDescriptor, guid.Value);
                        //var reflectionNode = new ReflectionNode(data);
                        //reflectionNode.RefreshChildrenAsync();

                        tree = new Tree { Key = clone.Key, Data = instance };

                    }
                    else
                    {
                        tree = new Tree { Key = clone.Key, Data = null };
                    }

                    var parentMatch = TreeExtensions.MatchDescendant(view, (d => d.Key.Equals(a.Value.Parent?.Key))) as Tree;
                    if (parentMatch != null)
                        parentMatch.Add(tree);
                }
                if (a.Type == Changes.Type.Remove)
                {
                    var match = TreeExtensions.MatchDescendant(view, (d => d.Key.Equals(a.Value.Key))) as Tree;
                    match.Parent.Remove(a.Value);
                }
            });
        }
    }
}
