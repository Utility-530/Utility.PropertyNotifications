
using Utility.Nodify.Core;
using Utility.Nodify.Operations;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Windows.Input;
using OperationKeys = Utility.Nodify.Operations.Keys;

using DryIoc;
using Utility.Commands;
using System.Collections.ObjectModel;
using Message = Utility.Nodify.Core.Message;
using Utility.Nodify.ViewModels;
using Utility.Nodify.Models;

namespace Utility.Nodify.Engine.Infrastructure
{
    public class Bootstrapper
    {
        public static IContainer Build(IContainer container)
        {
            container.Register<DiagramsViewModel>();

            container.Register<MainViewModel>();
            container.Register<TabsViewModel, CustomTabsViewModel>();
            container.Register<MessagesViewModel>();
            container.Register<IDiagramViewModel, ViewModels.DiagramViewModel>();
            container.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Next);
            container.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Previous);
            container.RegisterDelegate<ICommand>(c => new Command(() => c.Resolve<IObserver<object>>(OperationKeys.Next).OnNext(default)), serviceKey: OperationKeys.NextCommand);
            container.RegisterDelegate<ICommand>(c => new Command(() => c.Resolve<IObserver<object>>(OperationKeys.Previous).OnNext(default)), serviceKey: OperationKeys.PreviousCommand);
            container.RegisterMany<Dictionary<string, FilterInfo>>();
            container.Register<RangeObservableCollection<Diagram>>(serviceKey: OperationKeys.SelectedDiagram);
            container.RegisterDelegate(c => c.Resolve<IEnumerable<Diagram>>(), serviceKey: OperationKeys.Diagrams);
            container.RegisterMany<Dictionary<Core.Key, NodeViewModel>>(serviceKey: OperationKeys.Nodes);
            container.RegisterMany<Dictionary<Core.Key, ConnectionViewModel>>(serviceKey: OperationKeys.Connections);
            container.RegisterDelegate<IDictionary<string, FilterInfo>>(() => new Dictionary<string, FilterInfo>(), serviceKey: OperationKeys.Filters);
            container.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Future);
            container.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Current);
            container.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Past);
            container.Register<IDiagramFactory, DiagramFactory>();

            return container;
        }
    }

    public class CustomTabsViewModel : TabsViewModel
    {
        private readonly IContainer container;

        public CustomTabsViewModel(IContainer container)
        {
            this.container = container;
        }

        protected override object Content
        {
            get
            {
                var diagram = new Diagram();
                var viewmodel = container.Resolve<IConverter>().Convert(diagram);
                return viewmodel;
            }
        }
    }
}
