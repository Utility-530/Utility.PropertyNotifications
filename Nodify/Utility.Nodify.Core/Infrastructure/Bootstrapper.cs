
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

namespace Utility.Nodify.Models.Infrastructure
{
    public class Bootstrapper
    {
        public static IContainer Build(IContainer container)
        {

            container.Register<IDiagramViewModel, DiagramViewModel>();
            container.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Next);
            container.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Previous);
            container.RegisterDelegate<ICommand>(c => new Command(() => c.Resolve<IObserver<object>>(OperationKeys.Next).OnNext(default)), serviceKey: OperationKeys.NextCommand);
            container.RegisterDelegate<ICommand>(c => new Command(() => c.Resolve<IObserver<object>>(OperationKeys.Previous).OnNext(default)), serviceKey: OperationKeys.PreviousCommand);
            //container.RegisterMany<Dictionary<string, FilterInfo>>();
            container.Register<RangeObservableCollection<Diagram>>(serviceKey: OperationKeys.SelectedDiagram);
            container.RegisterDelegate(c => c.Resolve<IEnumerable<Diagram>>(), serviceKey: OperationKeys.Diagrams);
            container.RegisterMany<Dictionary<string, NodeViewModel>>(serviceKey: OperationKeys.Nodes);
            container.RegisterMany<Dictionary<string, ConnectionViewModel>>(serviceKey: OperationKeys.Connections);
            //container.RegisterDelegate<IDictionary<string, FilterInfo>>(() => new Dictionary<string, FilterInfo>(), serviceKey: OperationKeys.Filters);
            container.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Future);
            container.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Current);
            container.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Past);


            return container;
        }
    }

}
