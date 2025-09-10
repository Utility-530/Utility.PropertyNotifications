using ActivateAnything;
using DryIoc;
using Splat;
using SQLite;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Utility.Common.Model;
using Utility.Helpers.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Base;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Core;
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

            converter = new Converter(container, collection);
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
               
                    if((a.Output as IGetGuid).Guid==default)
                        (a.Output as ISetGuid).Guid = Guid.NewGuid();

                    Add(new Connection() { Guid = Guid.NewGuid(), InputId = (a.Input as IGetGuid).Guid, OutputId = (a.Output as IGetGuid).Guid });
                });

            diagramViewModel
                .Nodes
                .AndAdditions<INodeViewModel>()
                .Subscribe(a =>
                {
                    var key = converter.KeyExtractor(a.Data);
                    var nodeId = (a as IGetGuid).Guid;
                    Add(new Node() { DiagramId = diagramId, Guid = nodeId, Key = key });

                    a.Input.AndAdditions<IConnectorViewModel>().Subscribe(input =>
                    {
                        if (input is not IPendingConnectorViewModel)
                        {
                            if ((input as IGetGuid).Guid == default)
                                (input as ISetGuid).Guid = Guid.NewGuid();
                            Add(new Connector() { NodeId = (input.Node as IGetGuid).Guid, Guid = (input as IGetGuid).Guid, Key = converter.KeyExtractor(input), IsInput = true });
                        }
                    });

                    a.Output.AndAdditions<IConnectorViewModel>().Subscribe(output =>
                    {
                        if (output is not IPendingConnectorViewModel)
                        {
                            if ((output as IGetGuid).Guid == default)
                                (output as ISetGuid).Guid = Guid.NewGuid();

                            Add(new Connector() { NodeId = (output.Node as IGetGuid).Guid, Guid = (output as IGetGuid).Guid, Key = converter.KeyExtractor(output) });
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
                converter.CreateInstanceFromKey(node.Key).Subscribe(data =>
                {
                    INodeViewModel nodeViewModel;
                    if (diagramViewModel.Nodes.Any(a => (a as IGetGuid).Guid == node.Guid))
                    {
                        nodeViewModel = diagramViewModel.Nodes.SingleOrDefault(a => (a as IGetGuid).Guid.ToString() == node.Guid.ToString());
                    }
                    else
                    {
                        //nodeViewModel = new NodeViewModel() { Key = data.Key, Data = data.Data };
                        nodeViewModel = container.Resolve<IViewModelFactory>().CreateNode(data);
                        diagramViewModel.Nodes.Add(nodeViewModel);                      
                    }

                    var connectors = connection.Query<Connector>("SELECT * FROM Connector WHERE NodeId = ?", node.Guid);
                    foreach (var connector in connectors)
                    {
                        var inputConnections = connection.Query<Connection>("SELECT * FROM Connection WHERE InputId = ?", connector.Guid);
                        foreach (var connectionRecord in inputConnections)
                        {
                            converter.CreateInstanceFromKey(connector.Key).Subscribe(a =>
                            {

                                //var inputConnectorViewModel = new ConnectorViewModel() { Guid = connector.Guid, IsInput = true, Key = a.Key, Node = nodeViewModel, Data = a.Data };
                                var inputConnectorViewModel = container.Resolve<IViewModelFactory>().CreateConnector(new ConnectorParameters(connector.Guid, true, a.Key, nodeViewModel, a.Data));
                                nodeViewModel.Input.Add(inputConnectorViewModel);
                                // Find the output connector
                                var outputConnector = connection.Query<Connector>("SELECT * FROM Connector WHERE Guid = ?", connectionRecord.OutputId).FirstOrDefault();
                                //ConnectorViewModel? outputConnectorViewModel = null;
                                if (outputConnector != null)
                                {
                                    // Find the output node
                                    var outputNode = nodes.FirstOrDefault(n => n.Guid == outputConnector.NodeId);
                                    if (outputNode != null)
                                    {
                                        var outputNodeViewModel = diagramViewModel.Nodes.SingleOrDefault(a => (a as IGetGuid).Guid == outputNode.Guid);
                                        //var outputNodeViewModel = new NodeViewModel() { Key = outputNode.Key, Guid = outputNode.Guid };
                                  

                                        converter.CreateInstanceFromKey(outputConnector.Key)
                                        .Subscribe(al =>
                                        {
                                            //outputConnectorViewModel = new ConnectorViewModel() { Guid = outputConnector.Guid, IsInput = false, Key = al.Key, Node = outputNodeViewModel, Data = al.Data };
                                            var outputConnectorViewModel = container.Resolve<IViewModelFactory>().CreateConnector(new ConnectorParameters(outputConnector.Guid, false, al.Key, outputNodeViewModel, al.Data));
                                            outputNodeViewModel.Output.Add(outputConnectorViewModel);
                                            var connection = container.Resolve<IViewModelFactory>().CreateConnection(inputConnectorViewModel, outputConnectorViewModel);
                                            (connection as ISetGuid).Guid = connectionRecord.Guid;
                                            diagramViewModel.Connections.Add(connection);

                                            //new ConnectionViewModel()
                                            //{
                                            //    Guid = connectionRecord.Guid,
                                            //    Input = inputConnectorViewModel,
                                            //    Output = outputConnectorViewModel
                                            //}

                                        });
                                    }
                                }
                            });
                        }

                        //var outputConnections = connection.Query<Connection>("SELECT * FROM Connection WHERE OutputId = ?", connector.Guid);
                        //foreach (var connectionRecord in outputConnections)
                        //{
                        //    var outputConnectorViewModel = new ConnectorViewModel() { Guid = connectionRecord.Guid, IsInput = false, Key = connector.Key, Node = nodeViewModel, Data = null };

                        //    // Find the input connector
                        //    var inputConnector = connection.Query<Connector>("SELECT * FROM Connector WHERE Guid = ?", connectionRecord.InputId).FirstOrDefault();
                        //    ConnectorViewModel? inputConnectorViewModel = null;
                        //    if (inputConnector != null)
                        //    {
                        //        // Find the input node
                        //        var inputNode = nodes.FirstOrDefault(n => n.Guid == inputConnector.NodeId);
                        //        if (inputNode != null)
                        //        {
                        //            //var inputNodeViewModel = new NodeViewModel() { Key = inputNode.Key, Guid = inputNode.Guid };
                        //            var inputNodeViewModel = diagramViewModel.Nodes.SingleOrDefault(a => (a as IGetGuid).Guid == inputNode.Guid);
                        //            inputConnectorViewModel = new ConnectorViewModel() { Guid = inputConnector.Guid, IsInput = true, Key = inputConnector.Key, Node = inputNodeViewModel, Data = null };
                        //        }
                        //    }

                        //    if (inputConnectorViewModel != null)
                        //    {
                        //        diagramViewModel.Connections.Add(new ConnectionViewModel()
                        //        {
                        //            Guid = connectionRecord.Guid,
                        //            Input = inputConnectorViewModel,
                        //            Output = outputConnectorViewModel
                        //        });
                        //    }
                        //}
                    }
                });
            }
            return diagramViewModel;
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