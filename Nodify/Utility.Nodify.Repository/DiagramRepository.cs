using DryIoc;
using Newtonsoft.Json;
using Splat;
using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Helpers.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Diagrams;
using Utility.Nodify.Core;
using Utility.Nodify.Models;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.ServiceLocation;
using Utility.Trees;
using Utility.Trees.Abstractions;
using static Utility.Models.TreeResolver;
using static Utility.Nodify.Repository.DiagramRepository;

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

        [SQLite.Unique]
        public string Key { get; set; }

        public Guid InputId { get; set; }

        public Guid OutputId { get; set; }
    }

    public class DiagramRepository
    {
        private readonly SQLiteConnection connection;
        private readonly Task initialisationTask;
        private Converter converter;

        public DiagramRepository(string? dbDirectory = default)
        {
            var collection = new Collection<IReadOnlyTree>();

            converter = new Converter(collection);
            Globals.Resolver.Resolve<IObservable<IReadOnlyTree>>().Subscribe(a =>
            {
                collection.Add(a);
            });

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

        public void Track(ViewModels.DiagramViewModel diagramViewModel)
        {
            var diagramId = Guid.NewGuid();
            if (diagramViewModel.Guid != default)
            {
                Add(new Diagram { Guid = diagramId, Key = diagramViewModel.Key });
            }
            else
                diagramId = diagramViewModel.Guid;

            diagramViewModel
                .Connections
                .AndAdditions<IConnectionViewModel>()
                .Subscribe(a =>
                {
                    if ((a as IGetGuid).Guid != default)
                        return;
                    Guid inputId = Guid.NewGuid();
                    Guid outputId = Guid.NewGuid();
                    Add(new Connector() { Guid = Guid.NewGuid(), Key = converter.KeyExtractor(a.Input), NodeId = inputId });
                    Add(new Connector() { Guid = Guid.NewGuid(), Key = converter.KeyExtractor(a.Output), NodeId = outputId });
                    Add(new Connection() { Guid = Guid.NewGuid(), InputId = inputId, OutputId = outputId, Key = (a as IGetKey).Key });
                });

            diagramViewModel
                .Nodes
                .AndAdditions<INodeViewModel>()
                .Subscribe(a =>
                {
                    if ((a as IGetGuid).Guid != default)
                        return;
                    if (filter(a.Data) == false)
                        return;
                    var key = converter.KeyExtractor(a.Data);
                    var nodeId = Guid.NewGuid();
                    Add(new Node() { DiagramId = diagramId, Guid = Guid.NewGuid(), Key = key });
                    foreach (var input in a.Input)
                        Add(new Connector() { NodeId = nodeId, Guid = Guid.NewGuid(), Key = converter.KeyExtractor(input) });
                    foreach (var output in a.Output)
                        Add(new Connector() { NodeId = nodeId, Guid = Guid.NewGuid(), Key = converter.KeyExtractor(output) });
                });
        }

        public ViewModels.DiagramViewModel? Convert(string key)
        {
            var find = connection.FindWithQuery<Diagram>("SELECT * FROM Diagram WHERE Key = ?", key);
            if (find == null)
                //throw new Exception($"Diagram with key '{key}' not found");
                return null;

            var diagramViewModel = new ViewModels.DiagramViewModel(Locator.Current.GetService<DryIoc.IContainer>()) { Key = key, Guid = find.Guid };
            var nodes = connection.Query<Node>("SELECT * FROM Node WHERE DiagramKey = ?", key);

            foreach (var node in nodes)
            {
                converter.CreateInstanceFromKey(node.Key).Subscribe(data =>
                {
                    var nodeViewModel = new NodeViewModel() { Key = node.Key, Data = data };
                    var connectors = connection.Query<Connector>("SELECT * FROM Connector WHERE NodeId = ?", node.Guid);
                    foreach (var connector in connectors)
                    {
                        var inputConnections = connection.Query<Connection>("SELECT * FROM Connection WHERE InputId = ?", connector.Guid);
                        foreach (var connectionRecord in inputConnections)
                        {
                            var inputConnectorViewModel = new ConnectorViewModel() { Guid = connector.Guid, IsInput = true, Key = connector.Key, Node = nodeViewModel, Data = null };

                            // Find the output connector
                            var outputConnector = connection.Query<Connector>("SELECT * FROM Connector WHERE Guid = ?", connectionRecord.OutputId).FirstOrDefault();
                            ConnectorViewModel? outputConnectorViewModel = null;
                            if (outputConnector != null)
                            {
                                // Find the output node
                                var outputNode = nodes.FirstOrDefault(n => n.Guid == outputConnector.NodeId);
                                if (outputNode != null)
                                {
                                    var outputNodeViewModel = new NodeViewModel() { Key = outputNode.Key, Guid = outputNode.Guid };
                                    outputConnectorViewModel = new ConnectorViewModel() { Guid = outputConnector.Guid, IsInput = false, Key = outputConnector.Key, Node = outputNodeViewModel, Data = null };
                                }
                            }

                            if (outputConnectorViewModel != null)
                            {
                                diagramViewModel.Connections.Add(new ConnectionViewModel()
                                {
                                    Guid = connectionRecord.Guid,
                                    Input = inputConnectorViewModel,
                                    Output = outputConnectorViewModel
                                });
                            }
                        }

                        var outputConnections = connection.Query<Connection>("SELECT * FROM Connection WHERE OutputId = ?", connector.Guid);
                        foreach (var connectionRecord in outputConnections)
                        {
                            var outputConnectorViewModel = new ConnectorViewModel() { Guid = connectionRecord.Guid, IsInput = false, Key = connector.Key, Node = nodeViewModel, Data = null };

                            // Find the input connector
                            var inputConnector = connection.Query<Connector>("SELECT * FROM Connector WHERE Guid = ?", connectionRecord.InputId).FirstOrDefault();
                            ConnectorViewModel? inputConnectorViewModel = null;
                            if (inputConnector != null)
                            {
                                // Find the input node
                                var inputNode = nodes.FirstOrDefault(n => n.Guid == inputConnector.NodeId);
                                if (inputNode != null)
                                {
                                    var inputNodeViewModel = new NodeViewModel() { Key = inputNode.Key, Guid = inputNode.Guid };
                                    inputConnectorViewModel = new ConnectorViewModel() { Guid = inputConnector.Guid, IsInput = true, Key = inputConnector.Key, Node = inputNodeViewModel, Data = null };
                                }
                            }

                            if (inputConnectorViewModel != null)
                            {
                                diagramViewModel.Connections.Add(new ConnectionViewModel()
                                {
                                    Guid = connectionRecord.Guid,
                                    Input = inputConnectorViewModel,
                                    Output = outputConnectorViewModel
                                });
                            }
                        }
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




        public record InstanceKey(string Key, object Data);

        public void Add(Diagram diagram)
        {
            initialisationTask.Wait();
            var find = connection.FindWithQuery<Diagram>("SELECT * FROM Diagram WHERE Key = ?", diagram.Key);
            if (find != null)
                return;
            connection.Insert(diagram);
        }

        public void Add(Node node)
        {
            initialisationTask.Wait();
            var find = connection.FindWithQuery<Node>("SELECT * FROM Node WHERE Key = ?", node.Key);
            if (find != null)
                return;
            connection.Insert(node);
        }

        public void Add(Connector connector)
        {
            initialisationTask.Wait();
            var find = connection.FindWithQuery<Connector>("SELECT * FROM Connector WHERE Key = ? AND NodeId = ?", connector.Key, connector.NodeId);
            if (find != null)
                return;
            connection.Insert(connector);
        }

        public void Add(Connection connection)
        {
            initialisationTask.Wait();
            var find = this.connection.FindWithQuery<Connection>("SELECT * FROM Connection WHERE InputId = ? AND OutputId = ?", connection.InputId, connection.OutputId);

            if (find != null)
                return;
            this.connection.Insert(connection);
        }
    }

    public class Converter
    {
        private Collection<IReadOnlyTree> collection;

        public Converter(Collection<IReadOnlyTree> collection)
        {
            this.collection = collection;
        }

        // Key format: "TypeName|ActualKey" for type safety and reversibility
        public string KeyExtractor(object instance)
        {
            return instance switch
            {
                Utility.Models.TreeResolver.NamedTree namedTree => $"NamedTree|{namedTree.Name}",
                ConnectorViewModel connectorViewModel => KeyExtractor(connectorViewModel.Data),
                Tree tree => KeyExtractor(tree.Data),
                MethodConnector methodConnector => $"Parameter|{JsonConvert.SerializeObject(methodConnector.Parameter)}",
                Utility.Interfaces.Exs.IMethod methodNode => $"Method|{JsonConvert.SerializeObject(methodNode.MethodInfo)}",
                IGetName descriptor => $"Named|{descriptor.Name}",
                IGetReference getReference when getReference is IPropertyInfo propertyDescriptor =>
                    $"PropertyReference|{KeyExtractor(getReference.Reference)}#{propertyDescriptor.PropertyInfo.Name}",
                _ => throw new ArgumentException($"Unsupported instance type: {instance?.GetType()?.Name ?? "null"}")
            };
        }

        public IObservable<InstanceKey> CreateInstanceFromKey(string compositeKey)
        {
            var (typePrefix, key) = ParseKey(compositeKey);

            return Observable.Create<InstanceKey>(observer =>
            {
                if (typePrefix == "NamedTree")
                    observer.OnNext(CreateNamedTree(key));
                else if (typePrefix == "Method")
                    observer.OnNext(CreateMethodNode(key));
                else if (typePrefix == "ConnectorViewModel")
                    observer.OnNext(CreateConnectorViewModel(key));
                else if (typePrefix == "Named")
                    observer.OnNext(CreateNamedInstance(key));
                else if (typePrefix == "PropertyReference")
                    CreatePropertyReference(key).Subscribe(observer);
                else
                    throw new ArgumentException($"Unsupported type prefix: {typePrefix}");
                return Disposable.Empty;
            });

            InstanceKey CreateNamedTree(string name)
            {
                // This would need to be implemented based on your NamedTree constructor
                // Placeholder implementation - adjust based on actual NamedTree structure
                return new InstanceKey(name, new Utility.Models.TreeResolver.NamedTree { Name = name });
            }

            InstanceKey CreateMethodNode(string methodString)
            {
                // This would need to parse the method string back to MethodInfo
                // This is complex and might require a method registry or reflection

                var method = MethodHelpers.AsMethodInfo(methodString);
                return new InstanceKey(methodString, new MethodNode(method, null));
            }

            InstanceKey CreateConnectorViewModel(string key)
            {
                return new InstanceKey(key, new ConnectorViewModel { Key = key, Data = null });
            }

            InstanceKey CreateNamedInstance(string name)
            {
                // For generic IGetName instances, we might need additional type information
                // This is a simplified implementation
                return new InstanceKey(name, new NamedTree() { Name = name });
            }

            IObservable<InstanceKey> CreatePropertyReference(string key)
            {
                return Observable.Create<InstanceKey>(observer =>
                {
                    var parts = key.Split('#', 2);
                    if (parts.Length != 2)
                        throw new ArgumentException($"Invalid property reference format: {key}");

                    var referenceKey = parts[0];
                    var propertyName = parts[1];
                    return collection
                    .AndAdditions<IReadOnlyTree>()
                    .Subscribe(single =>
                    {
                        if (single.Data.ToString() == referenceKey)
                        {
                            if (single.Data is INotifyPropertyChanged npc)
                            {
                                observer.OnNext(new InstanceKey("", npc.WithChangesTo(npc.GetType().GetProperty(referenceKey))));
                            }
                        }
                        //throw new Exception("Cannot create property reference for non-INotifyPropertyChanged objects");
                    });
                });
            }
        }

        // Two-way conversion helper methods
        (string TypePrefix, string Key) ParseKey(string compositeKey)
        {
            var parts = compositeKey.Split('|', 2);
            if (parts.Length != 2)
                throw new ArgumentException($"Invalid key format: {compositeKey}. Expected 'TypePrefix|Key'");
            return (parts[0], parts[1]);
        }





        //// Enhanced key extractor that includes type information for better reconstruction
        //string ExtractKeyWithTypeInfo(object instance)
        //{
        //    var baseKey = keyExtractor(instance);
        //    var typeInfo = instance.GetType().FullName;
        //    return $"{typeInfo}::{baseKey}";
        //}


    }
}