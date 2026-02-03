using ActivateAnything;
using Optional;
using Splat;
using SQLite;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Utility.Interfaces;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Nodify.Base;
using Utility.Nodify.Base.Abstractions;
using Utility.Reactives;
using Utility.ServiceLocation;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodify.Repository
{
    public class Diagram
    {
        [PrimaryKey]
        public Guid Guid { get; set; }

        [Unique]
        public string Name { get; set; }
    }

    public class Connector
    {
        [PrimaryKey]
        public Guid Guid { get; set; }
        public Guid NodeId { get; set; }
        public string Name { get; set; }
        public bool IsInput { get; set; }
    }

    public class Node
    {
        [PrimaryKey]
        public Guid Guid { get; set; }
        public Guid DiagramId { get; set; }


        [Unique]
        public string Name { get; set; }

    }

    public class Connection
    {
        [PrimaryKey]
        public Guid Guid { get; set; }

        public Guid InputId { get; set; }

        public Guid OutputId { get; set; }
    }

    public class DiagramRepository
    {
        private readonly SQLiteConnection connection;
        private readonly Task initialisationTask;
        private JsonReflectionConverter converter;

        public DiagramRepository(string? dbDirectory = default)
        {
            var collection = new Collection<IReadOnlyTree>();

            converter = new JsonReflectionConverter();
            //Globals.Resolver.Resolve<IObservable<IReadOnlyTree>>().Subscribe(a =>
            //{
            //    collection.Add(a);
            //});

            if (dbDirectory == null)
                connection = new SQLiteConnection("diagram" + "." + "sqlite", false);
            else
            {
                //if file name
                if (string.IsNullOrEmpty(System.IO.Path.GetExtension(dbDirectory)) == false)
                {
                    var directory = Path.GetDirectoryName(dbDirectory);
                    if (!string.IsNullOrEmpty(directory))
                        Directory.CreateDirectory(directory);
                    connection = new SQLiteConnection(dbDirectory, false);
                }
                // if directory name
                else
                {
                    Directory.CreateDirectory(dbDirectory);
                    connection = new SQLiteConnection(Path.Combine(dbDirectory, "data" + "." + "sqlite"), false);
                }
            }

            connection.CreateTable<Diagram>();
            connection.CreateTable<Node>();
            connection.CreateTable<Connector>();
            connection.CreateTable<Connection>();
            initialisationTask = Initialise();
        }

        public bool IsInitialised { get; set; }

        public Task Initialise()
        {
            if (IsInitialised == false)
            {
                var task = new Task(() =>
                {
                    IsInitialised = true;
                });
                task.Start(TaskScheduler.FromCurrentSynchronizationContext());
                return task;
            }
            return Task.CompletedTask;
        }

        public void Track(IDiagramViewModel diagramViewModel)
        {
            //var find = connection.FindWithQuery<Diagram>("SELECT * FROM Diagram WHERE Name = ?", (diagramViewModel as IGetName).Name);
            //var diagram
            //if (find == null)
            //{
            //    Add(new Diagram { Guid = diagramViewModel.Guid(), Name = (diagramViewModel as IGetName).Name });
            //}
            //else
              var  diagramId = diagramViewModel.Guid();

            diagramViewModel
                .Connections
                .AndAdditions<IConnectionViewModel>()
                .Subscribe(a =>
                {
                    if ((a as IGetGuid).Guid != default)
                        return;

                    if ((a.Output as IGetGuid).Guid == default)
                        (a.Output as ISetGuid).Guid = Guid.NewGuid();

                    Add(new Connection() { Guid = Guid.NewGuid(), InputId = (a.Input as IGetGuid).Guid, OutputId = (a.Output as IGetGuid).Guid });
                });

            diagramViewModel
                .Nodes
                .AndAdditions<INodeViewModel>()
                .Subscribe(a =>
                {
                    var key = converter.Convert(a.Data());
                    var nodeId = (a as IGetGuid).Guid;
                    Add(new Node() { DiagramId = diagramId, Guid = nodeId, Name = key });

                    a.Inputs.AndAdditions<IConnectorViewModel>().Subscribe(input =>
                    {
                        if (input is not IPendingConnectorViewModel)
                        {
                            if ((input as IGetGuid).Guid == default)
                                (input as ISetGuid).Guid = Guid.NewGuid();
                            if (input is IGetData { Data: { } data })
                                Add(new Connector() { NodeId = input.Node.Guid(), Guid = (input as IGetGuid).Guid, Name = converter.Convert(data), IsInput = true });
                            else
                                throw new Exception("Input Connector data is null");
                        }
                    });

                    a.Outputs.AndAdditions<IConnectorViewModel>().Subscribe(output =>
                    {
                        if (output is not IPendingConnectorViewModel)
                        {
                            if ((output as IGetGuid).Guid == default)
                                (output as ISetGuid).Guid = Guid.NewGuid();
                            if (output is IGetData { Data: { } data })
                                Add(new Connector() { NodeId = output.Node.Guid(), Guid = (output as IGetGuid).Guid, Name = converter.Convert(data) });
                            else
                                throw new Exception("Output Connector data is null");
                        }
                    });

                });
        }

        public IDiagramViewModel? Convert(IDiagramViewModel diagramViewModel)
        {
            //var find = connection.FindWithQuery<Diagram>("SELECT * FROM Diagram WHERE Key = ?", (diagramViewModel as IGetKey).Key);
            //if (find == null)
            //    //throw new Exception($"Diagram with key '{key}' not found");
            //    return null;

            //var diagramViewModel = new ViewModels.DiagramViewModel(Locator.Current.GetService<DryIoc.IContainer>()) { Key = key, Guid = find.Guid };
            var nodes = connection.Query<Node>("SELECT * FROM Node WHERE DiagramId = ?", diagramViewModel.Guid());

            foreach (var node in nodes)
            {

                this.nodes(diagramViewModel, node, nodes).Subscribe(a => { });
            }
            return diagramViewModel;
        }

        List<INodeViewModel> loadedNodes = new List<INodeViewModel>();
        IObservable<INodeViewModel> nodes(IDiagramViewModel diagramViewModel, Node node, List<Node> nodes)
        {
            return Observable.Create<INodeViewModel>(observer =>
            {
                return converter.ConvertBack(node.Name)
                .Subscribe(data =>
                {
                    INodeViewModel nodeViewModel;
                    if (loadedNodes.Any(a => a.Guid() == node.Guid))
                    {
                        observer.OnNext(loadedNodes.SingleOrDefault(a => a.Guid() == node.Guid));
                        return;
                    }
                    else if (diagramViewModel.Nodes.Any(a => a.Guid() == node.Guid))
                    {
                        nodeViewModel = diagramViewModel.Nodes.SingleOrDefault(a => a.Guid() == node.Guid);
                    }
                    else
                    {

                        nodeViewModel = Globals.Resolver.Resolve<IViewModelFactory>().CreateNode(data);
                        (nodeViewModel as ISetKey).Key = new GuidKey(node.Guid);
                        diagramViewModel.Nodes.Add(nodeViewModel);
                    }
                    loadedNodes.Add(nodeViewModel);
                    observer.OnNext(nodeViewModel);

                    var _outputConnectors = connection.Query<Connector>("SELECT * FROM Connector WHERE NodeId = ? AND IsInput = false", node.Guid);
                    foreach (var _outputConnector in _outputConnectors)
                    {
                        converter
                        .ConvertBack(_outputConnector.Name)
                        .Subscribe(a =>
                        {
                            var outputConnectorViewModel = nodeViewModel.Outputs.SingleOrDefault(a => (a as IGetGuid).Guid == _outputConnector.Guid);
                            outputConnectorViewModel ??= Globals.Resolver.Resolve<IViewModelFactory>().CreateConnector(new ConnectorParameters(_outputConnector.Guid, _outputConnector.IsInput, a.Key, nodeViewModel, a.Data));
                            outputConnectorViewModel.Node = nodeViewModel;
                            nodeViewModel.Outputs.Add(outputConnectorViewModel);

                            var outputConnections = connection.Query<Connection>("SELECT * FROM Connection WHERE OutputId = ?", _outputConnector.Guid);
                            foreach (var outputConnection in outputConnections)
                            {
                                // Find the output connector
                                var inputConnector = connection.Query<Connector>("SELECT * FROM Connector WHERE Guid = ?", outputConnection.InputId).FirstOrDefault();
                                //ConnectorViewModel? outputConnectorViewModel = null;
                                if (inputConnector != null)
                                {
                                    converter
                                    .ConvertBack(inputConnector.Name)
                                    .Subscribe(al =>
                                    {
                                        Node inputNode = connection.Query<Node>("SELECT * FROM Node WHERE Guid = ?", inputConnector.NodeId).Single();
                                        this.nodes(diagramViewModel, inputNode, nodes)
                                        .Subscribe(inputNodeViewModel =>
                                        {

                                            var inputConnectorViewModel = inputNodeViewModel.Inputs.SingleOrDefault(a => (a as IGetGuid).Guid == inputConnector.Guid);

                                            if (inputConnectorViewModel == null)
                                            {
                                                inputConnectorViewModel = Globals.Resolver.Resolve<IViewModelFactory>().CreateConnector(new ConnectorParameters(inputConnector.Guid, inputConnector.IsInput, al.Key, inputNodeViewModel, al.Data));
                                                inputConnectorViewModel.Node = inputNodeViewModel;
                                                inputNodeViewModel.Inputs.Add(inputConnectorViewModel);
                                            }

                                            var connection = Globals.Resolver.Resolve<IViewModelFactory>().CreateConnection(outputConnectorViewModel, inputConnectorViewModel);
                                            (connection as ISetGuid).Guid = outputConnection.Guid;
                                            diagramViewModel.Connections.Add(connection);
                                        });
                                    });

                                }
                            }
                        });

                    }
                    var inputConnectors = connection.Query<Connector>("SELECT * FROM Connector WHERE NodeId = ? AND IsInput = true", node.Guid);

                    foreach (var inputConnector in inputConnectors)
                    {
                        converter
                        .ConvertBack(inputConnector.Name)
                        .Subscribe(al =>
                        {
                            Node inputNode = connection.Query<Node>("SELECT * FROM Node WHERE Guid = ?", inputConnector.NodeId).Single();

                            {
                                var inputConnectorViewModel = nodeViewModel.Inputs.SingleOrDefault(a => (a as IGetGuid).Guid == inputConnector.Guid);

                                if (inputConnectorViewModel == null)
                                {
                                    inputConnectorViewModel = Globals.Resolver.Resolve<IViewModelFactory>().CreateConnector(new ConnectorParameters(inputConnector.Guid, inputConnector.IsInput, al.Key, nodeViewModel, al.Data));
                                    inputConnectorViewModel.Node = nodeViewModel;
                                    nodeViewModel.Inputs.Add(inputConnectorViewModel);
                                }
                                // Find the output node

                            }
                        });

                    }
                });
            });
        }


        bool filter(object instance)
        {
            return instance switch
            {
                Tree => false,
                _ => true
            };
        }





        public bool Add(Diagram diagram)
        {
            initialisationTask.Wait();
            var find = connection.FindWithQuery<Diagram>("SELECT * FROM Diagram WHERE Name = ?", diagram.Name);
            if (find != null)
                return false;
            return connection.Insert(diagram) == 1;
        }

        public bool Add(Node node)
        {
            initialisationTask.Wait();
            var find = connection.FindWithQuery<Node>("SELECT * FROM Node WHERE Name = ?", node.Name);
            if (find != null)
                return false;
            return connection.Insert(node) == 1;
        }

        public bool Add(Connector connector)
        {
            initialisationTask.Wait();
            var find = connection.FindWithQuery<Connector>("SELECT * FROM Connector WHERE Name = ? AND NodeId = ?", connector.Name, connector.NodeId);
            if (find != null)
                return false;
            return connection.Insert(connector) == 1;
        }

        public bool Add(Connection connection)
        {
            initialisationTask.Wait();
            var find = this.connection.FindWithQuery<Connection>("SELECT * FROM Connection WHERE InputId = ? AND OutputId = ?", connection.InputId, connection.OutputId);

            if (find != null)
                return false;
            return this.connection.Insert(connection) == 1;
        }
    }
}