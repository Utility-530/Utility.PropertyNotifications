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

namespace Utility.PropertyTrees.WPF.Demo.Basic
{
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();
            Collection.Context = BaseObject.Context = SynchronizationContext.Current ?? throw new System.Exception("sd w3w");

            var property = Initialise();
            ListBox listBox = new()
            {
                ItemsSource = property.Children
            };

            //property
            //    .Children
            //    .Subscribe(item =>
            //    {
            //        if (item is not NotifyCollectionChangedEventArgs args)
            //            throw new Exception("rev re");

            //        foreach(INode node in SelectNewItems<INode>(args))
            //        {

            //        }
            //    },
            //    e =>
            //    {

            //    },
            //    () =>
            //    {
            //        //state.OnNext(State.Completed);
            //    }
            //  );


            var window = new Window { Content = listBox };
            window.Show();
            base.OnStartup(e);

//#if DEBUG
//            Tracing.Enable();
//#endif
        }

        private RootModelProperty Initialise()
        {

            var container = BootStrapper.Build();
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


}