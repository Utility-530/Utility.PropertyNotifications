using ActivateAnything;
using DryIoc;
using Optional;
using Splat;
using SQLite;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Utility.Extensions;
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
        public string Key { get; set; }
    }

    public class Connector
    {
        [PrimaryKey]
        public Guid Guid { get; set; }
        public Guid NodeId { get; set; }
        public string Key { get; set; }
        public bool IsInput { get; set; }
    }

    public class Node
    {
        [PrimaryKey]
        public Guid Guid { get; set; }
        public Guid DiagramId { get; set; }


        [Unique]
        public string Key { get; set; }

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
        private readonly DryIoc.IContainer container;
        private Converter converter;

        public DiagramRepository(DryIoc.IContainer container, string? dbDirectory = default)
        {
            var collection = new Collection<IReadOnlyTree>();

            converter = new Converter();
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
            this.container = container;
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
            var find = connection.FindWithQuery<Diagram>("SELECT * FROM Diagram WHERE Key = ?", (diagramViewModel as IGetKey).Key);

            var diagramId = Guid.NewGuid();
            if (find == null)
            {
                Add(new Diagram { Guid = diagramId, Key = (diagramViewModel as IGetKey).Key });
            }
            else
                diagramId = find.Guid;

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
                    Add(new Node() { DiagramId = diagramId, Guid = nodeId, Key = key });

                    a.Input.AndAdditions<IConnectorViewModel>().Subscribe(input =>
                    {
                        if (input is not IPendingConnectorViewModel)
                        {
                            if ((input as IGetGuid).Guid == default)
                                (input as ISetGuid).Guid = Guid.NewGuid();
                            if (input is IGetData { Data: { } data })
                                Add(new Connector() { NodeId = input.Node.Guid, Guid = (input as IGetGuid).Guid, Key = converter.Convert(data), IsInput = true });
                            else
                                throw new Exception("Input Connector data is null");
                        }
                    });

                    a.Output.AndAdditions<IConnectorViewModel>().Subscribe(output =>
                    {
                        if (output is not IPendingConnectorViewModel)
                        {
                            if ((output as IGetGuid).Guid == default)
                                (output as ISetGuid).Guid = Guid.NewGuid();
                            if (output is IGetData { Data: { } data })
                                Add(new Connector() { NodeId = output.Node.Guid, Guid = (output as IGetGuid).Guid, Key = converter.Convert(data) });
                            else
                                throw new Exception("Output Connector data is null");
                        }
                    });

                });
        }

        public IDiagramViewModel? Convert(IDiagramViewModel diagramViewModel)
        {
            var find = connection.FindWithQuery<Diagram>("SELECT * FROM Diagram WHERE Key = ?", (diagramViewModel as IGetKey).Key);
            if (find == null)
                //throw new Exception($"Diagram with key '{key}' not found");
                return null;

            //var diagramViewModel = new ViewModels.DiagramViewModel(Locator.Current.GetService<DryIoc.IContainer>()) { Key = key, Guid = find.Guid };
            var nodes = connection.Query<Node>("SELECT * FROM Node WHERE DiagramId = ?", find.Guid);

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
                return converter.ConvertBack(node.Key)
                .Subscribe(data =>
                {
                    INodeViewModel nodeViewModel;
                    if (loadedNodes.Any(a => a.Guid == node.Guid))
                    {
                        observer.OnNext(loadedNodes.SingleOrDefault(a => a.Guid.ToString() == node.Guid.ToString()));
                        return;
                    }
                    else if (diagramViewModel.Nodes.Any(a => a.Guid == node.Guid))
                    {
                        nodeViewModel = diagramViewModel.Nodes.SingleOrDefault(a => a.Guid.ToString() == node.Guid.ToString());
                    }
                    else
                    {
                        //nodeViewModel = new NodeViewModel() { Key = data.Key, Data = data.Data };
                        nodeViewModel = container.Resolve<IViewModelFactory>().CreateNode(data);
                        (nodeViewModel as ISetKey).Key = new GuidKey(node.Guid);
                        diagramViewModel.Nodes.Add(nodeViewModel);
                    }
                    loadedNodes.Add(nodeViewModel);
                    observer.OnNext(nodeViewModel);

                    var _outputConnectors = connection.Query<Connector>("SELECT * FROM Connector WHERE NodeId = ? AND IsInput = false", node.Guid);
                    foreach (var _outputConnector in _outputConnectors)
                    {
                        converter
                        .ConvertBack(_outputConnector.Key)
                        .Subscribe(a =>
                        {
                            var outputConnectorViewModel = nodeViewModel.Output.SingleOrDefault(a => (a as IGetGuid).Guid == _outputConnector.Guid);
                            outputConnectorViewModel ??= container.Resolve<IViewModelFactory>().CreateConnector(new ConnectorParameters(_outputConnector.Guid, true, a.Key, nodeViewModel, a.Data));
                            outputConnectorViewModel.Node = nodeViewModel;
                            nodeViewModel.Output.Add(outputConnectorViewModel);

                            var outputConnections = connection.Query<Connection>("SELECT * FROM Connection WHERE OutputId = ?", _outputConnector.Guid);
                            foreach (var outputConnection in outputConnections)
                            {
                                // Find the output connector
                                var inputConnector = connection.Query<Connector>("SELECT * FROM Connector WHERE Guid = ?", outputConnection.InputId).FirstOrDefault();
                                //ConnectorViewModel? outputConnectorViewModel = null;
                                if (inputConnector != null)
                                {
                                    converter
                                    .ConvertBack(inputConnector.Key)
                                    .Subscribe(al =>
                                    {
                                        Node inputNode = connection.Query<Node>("SELECT * FROM Node WHERE Guid = ?", inputConnector.NodeId).Single();
                                        this.nodes(diagramViewModel, inputNode, nodes)
                                        .Subscribe(inputNodeViewModel =>
                                        {

                                            var inputConnectorViewModel = inputNodeViewModel.Input.SingleOrDefault(a => (a as IGetGuid).Guid == inputConnector.Guid);

                                            if (inputConnectorViewModel == null)
                                            {
                                                inputConnectorViewModel = container.Resolve<IViewModelFactory>().CreateConnector(new ConnectorParameters(inputConnector.Guid, false, al.Key, inputNodeViewModel, al.Data));
                                                inputConnectorViewModel.Node = inputNodeViewModel;
                                                inputNodeViewModel.Input.Add(inputConnectorViewModel);
                                            }

                                            var connection = container.Resolve<IViewModelFactory>().CreateConnection(outputConnectorViewModel, inputConnectorViewModel);
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
                        .ConvertBack(inputConnector.Key)
                        .Subscribe(al =>
                        {
                            Node inputNode = connection.Query<Node>("SELECT * FROM Node WHERE Guid = ?", inputConnector.NodeId).Single();

                            {
                                var inputConnectorViewModel = nodeViewModel.Input.SingleOrDefault(a => (a as IGetGuid).Guid == inputConnector.Guid);

                                if (inputConnectorViewModel == null)
                                {
                                    inputConnectorViewModel = container.Resolve<IViewModelFactory>().CreateConnector(new ConnectorParameters(inputConnector.Guid, false, al.Key, nodeViewModel, al.Data));
                                    inputConnectorViewModel.Node = nodeViewModel;
                                    nodeViewModel.Input.Add(inputConnectorViewModel);
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
            var find = connection.FindWithQuery<Diagram>("SELECT * FROM Diagram WHERE Key = ?", diagram.Key);
            if (find != null)
                return false;
            return connection.Insert(diagram) == 1;
        }

        public bool Add(Node node)
        {
            initialisationTask.Wait();
            var find = connection.FindWithQuery<Node>("SELECT * FROM Node WHERE Key = ?", node.Key);
            if (find != null)
                return false;
            return connection.Insert(node) == 1;
        }

        public bool Add(Connector connector)
        {
            initialisationTask.Wait();
            var find = connection.FindWithQuery<Connector>("SELECT * FROM Connector WHERE Key = ? AND NodeId = ?", connector.Key, connector.NodeId);
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