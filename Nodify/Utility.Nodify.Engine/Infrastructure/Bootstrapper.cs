
using Utility.Nodify.Core;
using Utility.Nodify.Demo.ViewModels;
using Utility.Nodify.Operations.Infrastructure;
using Utility.Nodify.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using OperationKeys = Utility.Nodify.Operations.Keys;
using DemoKeys = Utility.Nodify.Demo.Keys;
using DryIoc;
using Utility.Commands;
using Utility.Models;
using System.Collections.ObjectModel;
using Message = Utility.Nodify.Operations.Message;
using Utility.Nodify.Engine.ViewModels;
using Utility.Infrastructure;

namespace Utility.Nodify.Demo.Infrastructure
{
    public class Bootstrapper
    {
        public static class DiConfiguration
        {
            public static Rules SetRules(Rules rules)
            {
                rules = rules
                    .WithDefaultReuse(Reuse.Singleton)
                    .With(FactoryMethod.ConstructorWithResolvableArguments);
                return rules;
            }

        }
        public static IContainer Build()
        {
            var builder = new Container(DiConfiguration.SetRules);

            RegisterOperations(builder);
            builder.Register<IOperationsFactory, MethodsOperationsFactory>();
            builder.Register<IObserver<BaseViewModel>, Utility.Nodify.Operations.Resolver>();
            //builder.Register<Diagram, Diagram1>();
            builder.Register<DiagramsViewModel>();
            builder.Register<MainViewModel>();
            builder.Register<TabsViewModel>();
            builder.Register<MessagesViewModel>();
            builder.Register<OperationsEditorViewModel>();      
            builder.RegisterInstanceMany<ISubject<PropertyChange>>(new ReplaySubject<PropertyChange>(1), serviceKey: DemoKeys.Pipe);
            builder.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Next);
            builder.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Previous);
            builder.RegisterDelegate<ICommand>(c => new Command(() => c.Resolve<IObserver<object>>(OperationKeys.Next).OnNext(default)), serviceKey: Keys.NextCommand);
            builder.RegisterDelegate<ICommand>(c => new Command(() => c.Resolve<IObserver<object>>(OperationKeys.Previous).OnNext(default)), serviceKey: Keys.PreviousCommand);
            builder.RegisterMany<Dictionary<string, FilterInfo>>();
            builder.Register<RangeObservableCollection<Diagram>>(serviceKey: DemoKeys.SelectedDiagram);
            builder.RegisterDelegate(c => c.Resolve<IEnumerable<Diagram>>(), serviceKey: DemoKeys.Diagrams);
            builder.RegisterMany<Dictionary<Core.Key, NodeViewModel>>(serviceKey: OperationKeys.Nodes);
            builder.RegisterMany<Dictionary<Core.Key, ConnectionViewModel>>(serviceKey: OperationKeys.Connections);
            builder.RegisterMany<Dictionary<string, OperationInfo>>(serviceKey: OperationKeys.Operations);
            builder.RegisterDelegate<IDictionary<string, FilterInfo>>(()=> new Dictionary<string, FilterInfo>(),serviceKey: OperationKeys.Filters);
            builder.RegisterDelegate(c => c.Resolve<IDictionary<string, OperationInfo>>(OperationKeys.Operations).Keys.Select(a => new MenuItemViewModel() { Content = a }));
            builder.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Future);
            builder.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Current);
            builder.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Past);


            OperationNodeViewModel.Observer = builder.Resolve<IObserver<BaseViewModel>>();
            OperationConnectionViewModel.Observer = builder.Resolve<IObserver<BaseViewModel>>();

            builder.RegisterInitializer<BaseViewModel>((a, context) =>
            {
                a.PropertyChanges().Subscribe(oo =>
                {
                    context
                      .Resolve<IObserver<PropertyChange>>(DemoKeys.Pipe)
                      .OnNext(oo);
                });
            });

            return builder;
        }


        private static void RegisterOperations(Container builder)
        {
            builder.RegisterDelegate<IDictionary<string, OperationInfo>>(a =>
            {
                Dictionary<string, OperationInfo> dictionary = new();

                foreach (var container in a.Resolve<IEnumerable<IOperationsFactory>>())
                {
                    foreach (var item in container.GetOperations())
                    {
                        dictionary[item.Title] = item;
                    }
                }

           

                return dictionary;
            });
        }
    }
   
}
