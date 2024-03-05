
using Utility.Nodify.Core;
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
using System.Collections.ObjectModel;
using Message = Utility.Nodify.Operations.Message;
using Utility.Nodify.Engine.ViewModels;
using Utility.ViewModels.Base;
using DynamicData;
using Utility.Helpers;
using Utility.Collections;
using Utility.Helpers.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Operations.Operations;

namespace Utility.Nodify.Demo.Infrastructure
{
    public class Bootstrapper
    {


        public static IContainer Build(IContainer container)
        {
   

            //RegisterOperations(builder);
     
            container.Register<IObserver<BaseViewModel>, Operations.Resolver>();
            //builder.Register<Diagram, Diagram1>();
            container.Register<DiagramsViewModel>();
            container.Register<Converter>();
            container.Register<MainViewModel>();
            container.Register<TabsViewModel, CustomTabsViewModel>();
            container.Register<MessagesViewModel>();
            container.Register<DiagramViewModel, ViewModels.DiagramViewModel>();
            container.RegisterInstanceMany<ISubject<PropertyChange>>(new ReplaySubject<PropertyChange>(1), serviceKey: DemoKeys.Pipe);
            container.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Next);
            container.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Previous);
            container.RegisterDelegate<ICommand>(c => new Command(() => c.Resolve<IObserver<object>>(OperationKeys.Next).OnNext(default)), serviceKey: Keys.NextCommand);
            container.RegisterDelegate<ICommand>(c => new Command(() => c.Resolve<IObserver<object>>(OperationKeys.Previous).OnNext(default)), serviceKey: Keys.PreviousCommand);
            container.RegisterMany<Dictionary<string, FilterInfo>>();
            container.Register<RangeObservableCollection<Diagram>>(serviceKey: DemoKeys.SelectedDiagram);
            container.RegisterDelegate(c => c.Resolve<IEnumerable<Diagram>>(), serviceKey: DemoKeys.Diagrams);
            container.RegisterMany<Dictionary<Core.Key, NodeViewModel>>(serviceKey: OperationKeys.Nodes);
            container.RegisterMany<Dictionary<Core.Key, ConnectionViewModel>>(serviceKey: OperationKeys.Connections);
            //builder.RegisterMany<Dictionary<string, OperationInfo>>(serviceKey: OperationKeys.Operations);
            container.RegisterDelegate<IDictionary<string, FilterInfo>>(() => new Dictionary<string, FilterInfo>(), serviceKey: OperationKeys.Filters);
            container.RegisterDelegate(c => c.Resolve<INodeSource>(OperationKeys.Operations).FindAll().Select(a => new MenuItemViewModel() { Content = a.Name }));
            container.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Future);
            container.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Current);
            container.Register<RangeObservableCollection<Message>>(serviceKey: OperationKeys.Past);


            OperationNodeViewModel.Observer = container.Resolve<IObserver<BaseViewModel>>();
            OperationConnectionViewModel.Observer = container.Resolve<IObserver<BaseViewModel>>();

            container.RegisterInitializer<BaseViewModel>((a, context) =>
            {
                a.PropertyChanges().Subscribe(oo =>
                {
                    context
                      .Resolve<IObserver<PropertyChange>>(DemoKeys.Pipe)
                      .OnNext(oo);
                });
            });

            return container;
        }


        //private static void RegisterOperations(Container builder)
        //{
        //    builder.RegisterDelegate<IDictionary<string, OperationInfo>>(a =>
        //    {
        //        Dictionary<string, OperationInfo> dictionary = new();

        //        foreach (var container in a.Resolve<IEnumerable<INodeSource>>())
        //        {
        //            foreach (var item in container.GetOperations())
        //            {
        //                dictionary[item.Title] = item;
        //            }
        //        }

        //        return dictionary;
        //    });
        //}
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
                var viewmodel = container.Resolve<Converter>().Convert(diagram);
                return viewmodel;
            }
        }
    }


    public class Converter
    {
        private readonly IContainer container;

        public Converter(IContainer container)
        {
            this.container = container;
        }

        public Core.DiagramViewModel Convert(Diagram diagram)
        {
            var diagramViewModel = container.Resolve<DiagramViewModel>();
            List<ConnectorViewModel> connectors = new();

            foreach (var node in diagram.Nodes)
            {
                NodeViewModel nodeViewModel = Convert(node);

                connectors.AddRange(nodeViewModel.Input);
                connectors.AddRange(nodeViewModel.Output);
                diagramViewModel.Nodes.Add(nodeViewModel);
            }

            foreach (var connection in diagram.Connections)
            {
                var connectionViewModel = new OperationConnectionViewModel
                {
                    Input = connectors.SingleOrDefault(a => a.Title == connection.Input),
                    Output = connectors.SingleOrDefault(a => a.Title == connection.Output)
                };
                diagramViewModel.Connections.Add(connectionViewModel);
            }

            return diagramViewModel;
        }

        public  NodeViewModel Convert(Node node)
        {
            var nodeViewModel = new OperationNodeViewModel
            {
                Title = node.Name,
                Location = node.Location
            };
            var methodInfo = TypeHelper.DeserialiseMethod(node.Content);
            nodeViewModel.Core = new MethodOperation(methodInfo);
            var inputs = node.Inputs.Select(a => new ConnectorViewModel() { Title = a.Key, IsInput = true, Node = nodeViewModel }).ToArray();
            var outputs = node.Outputs.Select(a => new ConnectorViewModel() { Title = a.Key, Node = nodeViewModel }).ToArray();
            nodeViewModel.Input.AddRange(inputs);
            nodeViewModel.Output.AddRange(outputs);

            return nodeViewModel;
        }


        public Diagram ConvertBack(Core.DiagramViewModel diagramViewModel)
        {
            var diagram = container.Resolve<Diagram>();

            List<Connector> connectors = new();
            //Diagram diagram = new();
            foreach (var nodeViewModel in diagramViewModel.Nodes)
            {
                var node = new Node
                {
                    Name = nodeViewModel.Title,
                    Location = nodeViewModel.Location,
                    Inputs = new Collection<Connector>(),
                    Outputs = new Collection<Connector>()
                };
                if (nodeViewModel is OperationNodeViewModel { Core: ISerialise serialise } operationNode)
                {
                    node.Content = serialise.ToString();
                }

                var inputs = nodeViewModel.Input.Select(a => new Connector() { Key = a.Title, IsInput = a.IsInput }).ToArray();
                var outputs = nodeViewModel.Output.Select(a => new Connector() { Key = a.Title, IsInput = a.IsInput }).ToArray();
                node.Inputs.AddRange(inputs);
                node.Outputs.AddRange(outputs);
                connectors.AddRange(inputs);
                connectors.AddRange(outputs);
               
                    diagram.Nodes.AddOrReplaceBy(a=>a.Name, node);
                
               
              
            }

            foreach (var connectionViewModel in diagramViewModel.Connections)
            {
                var connection = new Connection
                {
                    Input = connectionViewModel.Input?.Title,
                    Output = connectionViewModel.Output?.Title
                };
                //diagram.Connections.Add(connection);
                diagram.Connections.AddOrReplaceBy(a => a.Input + a.Output, connection);
            }

            return diagram;
        }
    }
}
