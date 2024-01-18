using Utility.Collections;
using Application = System.Windows.Application;
using Utility.Infrastructure;
using Resolver = Utility.PropertyTrees.Services.Resolver;
using System.Windows;
using System.Threading;
using DryIoc;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Utility.Nodes;
using Utility.PropertyTrees.WPF.Meta;
using Utility.Observables.Generic;
using Utility.Models;

namespace Utility.PropertyTrees.WPF.Demo.Basic
{
    public partial class App : Application
    {
   

        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();
            Collection.Context = BaseObject.Context = SynchronizationContext.Current ?? throw new System.Exception("sd w3w");

            var property = Initialise(out var container);

            var window = new Window { Content = container.Resolve<ViewController>().CreateContent(property) };
            window.Show();
            base.OnStartup(e);

            //#if DEBUG
            //            Tracing.Enable();
            //#endif

        
        }

        private RootModelProperty Initialise(out IContainer container)
        {

            container = BootStrapper.Build();
            var resolver = new Resolver(container);
            BaseObject.Resolver = resolver;
            resolver.Initialise();
            var property = container.Resolve<RootModelProperty>();
            return property;
        }

        private static IEnumerable<T> SelectNewItems<T>(NotifyCollectionChangedEventArgs args)
        {
            return args.NewItems?.Cast<T>() ?? Array.Empty<T>();
        }



    }


    public class ViewController : BaseObject
    {


        public ViewController()
        {

        }

        public override Key Key => new(Guids.ViewController, nameof(ViewController), typeof(ViewController));

        public TreeView CreateContent(ValueNode valueNode)
        {

            TreeView treeView = new();
            //CreateSelected(valueNode);
            Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, valueNode))
                .Subscribe();


            return treeView;


        }
    }

}