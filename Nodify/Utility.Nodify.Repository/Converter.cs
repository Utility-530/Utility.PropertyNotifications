using DryIoc;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodify.Base;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Core;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees;
using Utility.Trees.Abstractions;
using static Utility.Nodify.Repository.DiagramRepository;

namespace Utility.Nodify.Repository
{
    public class Converter
    {
        private readonly DryIoc.IContainer container;
        private Collection<IReadOnlyTree> collection;

        public Converter(DryIoc.IContainer container, Collection<IReadOnlyTree> collection)
        {
            this.container = container;
            this.collection = collection;
        }

        // Key format: "TypeName|ActualKey" for type safety and reversibility
        public string KeyExtractor(object instance)
        {
            return instance switch
            {
                string namedTree => $"String|{namedTree}",
                IConnectorViewModel connectorViewModel => KeyExtractor(connectorViewModel.Data),
                Tree tree => KeyExtractor(tree.Data),
                MethodConnector methodConnector => $"Parameter|{JsonConvert.SerializeObject(methodConnector.Parameter)}",
                Utility.Interfaces.Exs.IMethod methodNode => $"Method|{JsonConvert.SerializeObject(methodNode.MethodInfo)}",
                IGetName descriptor => $"Descriptor|{descriptor.Name}",
                IGetReference getReference when getReference is IPropertyInfo propertyDescriptor =>
                    $"PropertyReference|{KeyExtractor(getReference.Reference)}#{propertyDescriptor.PropertyInfo.Name}",
                PropertyInfo propertyInfo => $"PropertyInfo|{JsonConvert.SerializeObject(propertyInfo)}",
                _ => throw new ArgumentException($"Unsupported instance type: {instance?.GetType()?.Name ?? "null"}")
            };
        }

        public IObservable<InstanceKey> CreateInstanceFromKey(string compositeKey)
        {
            var (typePrefix, key) = ParseKey(compositeKey);

            return Observable.Create<InstanceKey>(observer =>
            {
                if (typePrefix == "String")
                    observer.OnNext(CreateNamedInstance(key));
                else if (typePrefix == "Method")
                    observer.OnNext(CreateMethodNode(key));
                else if (typePrefix == "Parameter")
                    observer.OnNext(new InstanceKey(key, new MethodConnector() { Parameter = JsonConvert.DeserializeObject<ParameterInfo>(key) }));
                else if (typePrefix == "ConnectorViewModel")
                    observer.OnNext(CreateConnectorViewModel(key));
                else if (typePrefix == "Descriptor")
                    observer.OnNext(CreateNamedInstance(key));
                else if (typePrefix == "PropertyReference")
                    CreatePropertyReference(key).Subscribe(observer);
                else if (typePrefix == "PropertyInfo")
                    observer.OnNext(new InstanceKey("Prop", JsonConvert.DeserializeObject<PropertyInfo>(key)));
                else
                    throw new ArgumentException($"Unsupported type prefix: {typePrefix}");
                return Disposable.Empty;
            });

            InstanceKey CreateMethodNode(string methodString)
            {
                // This would need to parse the method string back to MethodInfo
                // This is complex and might require a method registry or reflection

                var method = JsonConvert.DeserializeObject<MethodInfo>(methodString);
                return new InstanceKey(methodString, new MethodNode(method, null));
            }

            InstanceKey CreateConnectorViewModel(string key)
            {
                //return new InstanceKey(key, new ConnectorViewModel { Key = key, Data = null });
                return new InstanceKey(key, this.container.Resolve<IViewModelFactory>().CreateConnector(key));
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