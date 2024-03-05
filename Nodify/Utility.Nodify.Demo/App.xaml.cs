using DryIoc;
using Utility.Nodify.Core;
using Utility.Nodify.Demo.Infrastructure;
using System;
using System.Windows;
using Utility.Nodify.Operations.Infrastructure;
using Utility.Nodify.Engine.ViewModels;
using Utility.Descriptors;
using Utility.Interfaces.NonGeneric;
using System.Reactive.Linq;
using Utility.Descriptors.Repositorys;
using Utility.Changes;
using Splat;
using System.Reflection;
using NetFabric.Hyperlinq;
using System.Linq;
using System.Collections.Generic;
using Utility.ViewModels.Base;
using Utility.Helpers;
using System.Diagnostics;

namespace Utility.Nodify.Demo
{
    public partial class App : Application
    {
        IContainer container;

        Guid guid = Guid.Parse("25ee5731-11cf-4fc1-a925-50272fb99bba");
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(TreeRepository.Instance);

            var diagram = new Diagram();
            Helper.Load(diagram, "diagram_test", guid);

            base.OnStartup(e);

            container = new Container(DiConfiguration.SetRules);
            container.Register<INodeSource, MethodsOperationsFactory>(serviceKey: Keys.Operations);

            _ = Bootstrapper.Build(container);
            //container.RegisterInstance(diagram);
            //container.RegisterDelegate<Diagram>(Helpers.ToDiagram);
            container.RegisterInstance<Diagram>(diagram);
            container.Register<BaseViewModel, BooleanViewModel>();
            container.Register<BaseViewModel, ViewModel>();
            //container.Register<OperationInterfaceNodeViewModel>();
            //container.Register<InterfaceViewModel>();

            //container.Register<INodeSour, CustomOperationsFactory>();
            //container.Register<INodeSour, InterfaceOperationsFactory>();


            DockWindow dockWindow = new()
            {
                DataContext = container.Resolve<MainViewModel>()
            };

            //Window window = new()
            //{
            //    Content = container.Resolve<InterfaceViewModel>()
            //};

            dockWindow.Show();
            //window.Show();
        }


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

        protected override void OnExit(ExitEventArgs e)
        {
            var diagramViewModel = container.Resolve<DiagramViewModel>();
            var diagram = container.Resolve<Converter>().ConvertBack(diagramViewModel);
            Helper.Load(diagram, "diagram_test", guid);

            base.OnExit(e);
        }
    }

    public static class Helper
    {
        public static void Load(object instance, string name, Guid guid)
        {
            // associate guid with new proto
            var proto = TreeRepository.Instance.CreateProto(guid, name, instance.GetType()).Result;
            //TreeRepository.Instance.Name = name;
            var propertyData = new RootDescriptor(instance) { Guid = guid };
            propertyData.VisitDescendants(a => { });
        }

        public static void VisitDescendants(this IMemberDescriptor tree, Action<IMemberDescriptor> action)
        {
            action(tree);

            tree.Children
                .Cast<Change<IMemberDescriptor>>()
                .Subscribe(a =>
                {
                    if (a.Type == Changes.Type.Add)
                    {
                        VisitDescendants(a.Value, action);
                        Trace.WriteLine(a.Value.ParentType + " " + a.Value.Type?.Name + " " + a.Value.Name);
                    }
                    else
                    {

                    }
                });
        }

    }


    public static class A
    {
        private static System.Type type = typeof(A);

        public static int _A()
        {
            return 0;
        }

        public static int _B()
        {
            return 2;
        }

        public static int _C()
        {
            return 2;
        }

        public static int Sum(int a, int b)
        {
            return a + b;
        }

        public static int Multiply(int a, int b)
        {
            return a * b;
        }

        public record ViewModel(int Sum, int Multiplication);

        public static (ParameterInfo, ParameterInfo)[] Connections => new[] {
            (ReturnParameter(nameof(_A)), Parameter(nameof(Sum), "a")),
            //(ReturnParameter(nameof(_B)), Parameter(nameof(Sum), "b")),
            //(ReturnParameter(nameof(Sum)), Parameter(nameof(Multiply), "a")),
            //(ReturnParameter(nameof(_C)), Parameter(nameof(Multiply), "b"))
        };

        static ParameterInfo Parameter(string methodName, string name)
        {
            if (name == null)
                return type.GetMethod(methodName).ReturnParameter;
            var parameters = type.GetMethod(methodName).GetParameters();

            foreach (var parameter in parameters)
            {
                if (parameter.Name == name)
                {
                    return parameter;
                }

            }
            throw new Exception("s39d sed333 dd");
        }

        static ParameterInfo ReturnParameter(string methodName)
        {
            return type.GetMethod(methodName).ReturnParameter;
        }
    }

    class Helpers
    {
        public static Diagram ToDiagram()
        {
            List<Connector> connectors = new();
            List<Connection> connections = new();
            List<Node> nodes = new();

            foreach (var method in typeof(A).GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var node = ToNode(method);
                connectors.AddRange(node.Inputs);
                connectors.AddRange(node.Outputs);
                nodes.Add(node);
            }
            foreach (var (a, b) in A.Connections)
            {
                var connectionViewModel = ToConnection(a, b);
                connections.Add(connectionViewModel);
            }

            return new Diagram() { Nodes = nodes, Connections = connections };
        }

        public static Node ToNode(MethodInfo methodInfo)
        {
            var nodeViewModel = new Node
            {
                Name = methodInfo.Name,             //nodeViewModel.Action = new Action(() => { nodeViewModel.Output.Single().Value= methodInfo.Invoke(null, nodeViewModel.Input.Select(a=>a.Value).ToArray());  });
                Content = methodInfo.Serialise(),
                Inputs = methodInfo.GetParameters().Select(a => new Connector() { Key = methodInfo.Name + "." + a.Name }).ToArray(),
                Outputs = new[] { new Connector() { Key = methodInfo.Name + "." + methodInfo.ReturnParameter.Name } }
            };

            return nodeViewModel;
        }

        public static Connection ToConnection(ParameterInfo source, ParameterInfo destination)
        {
            //var input = connectors.SingleOrDefault(a => a.Title == source.Member.Name + "." + source.Name);
            //var output = connectors.SingleOrDefault(a => a.Title == destination.Member.Name + "." + destination.Name);
            //var input = new  ConnectorViewModel() { Node = nodeViewModel, Title = "out" };
            //var output = new  ConnectorViewModel() { Node = nodeViewModel, Title = "out" };
            //var connectors = methodInfo.GetParameters().Select(a => new ConnectorViewModel() { Node = nodeViewModel, Title = a.Name });
            return new Connection() { Input = source.Member.Name + "." + source.Name, Output = destination.Member.Name + "." + destination.Name };

        }
    }

    public class MethodsOperationsFactory : INodeSource
    {
        Lazy<ICollection<Node>> nodes = new(() => ToNodes(typeof(A)).ToList());

        public Node Find(string key)
        {
            return nodes.Value.Single(a => a.Name == key);
        }

        public IEnumerable<Node> FindAll()
        {
            return nodes.Value;
        }

        public static IEnumerable<Node> ToNodes(System.Type type)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                yield return ToNode(method);
            }
        }

        public static Node ToNode(MethodInfo methodInfo)
        {
            var nodeViewModel = new Node
            {
                Name = methodInfo.Name,             //nodeViewModel.Action = new Action(() => { nodeViewModel.Output.Single().Value= methodInfo.Invoke(null, nodeViewModel.Input.Select(a=>a.Value).ToArray());  });
                Content = methodInfo.Serialise(),
                Inputs = methodInfo.GetParameters().Select(a => new Connector() { Key = methodInfo.Name + "." + a.Name }).ToArray(),
                Outputs = new[] { new Connector() { Key = methodInfo.Name + "." + methodInfo.ReturnParameter.Name } }
            };

            return nodeViewModel;
        }

    }
}
