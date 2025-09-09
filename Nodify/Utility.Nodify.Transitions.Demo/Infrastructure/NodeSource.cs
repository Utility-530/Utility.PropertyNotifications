using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Helpers.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Diagrams;
using Utility.Nodes;
using Utility.Nodify.Core;
using Utility.Nodify.Models;
using Utility.Nodify.Operations.Infrastructure;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.ServiceLocation;
using Utility.Trees;

namespace Utility.Nodify.Transitions.Demo.Infrastructure
{
    public class NodeSource : INodeSource
    {
        record Ref(PropertyInfo Info, IConnectionViewModel ConnectionViewModel);
        record Ref2(Type Type, List<MethodInfo> MethodInfos);
        record Ref3(Type Type, MethodInfo MethodInfo) : IType;
        record Ref4(Type Type, IConnectionViewModel ConnectionViewModel) : IType;
        record Ref5(Utility.PropertyDescriptors.MemberDescriptor MemberDescriptor, PropertyInfo Info)
        {
            public override string ToString()
            {
                return MemberDescriptor.Name + "<-" + Info.Name;
            }
        }

        public IEnumerable<MenuItem> Filter(object viewModel)
        {
            if (viewModel is IConnectionViewModel pending)
            {
                if (pending.Input.Data is Type type)
                {
                    yield return new MenuItem("Method", Guid.NewGuid()) { Reference = type };
                    yield return new MenuItem("Set", Guid.NewGuid()) { Reference = new Ref4(type, pending) };
                }
                else if (pending.Input.Node.Data is MethodNode data)
                {
                    yield return new MenuItem("Method", Guid.NewGuid()) { Reference = pending };
                    yield return new MenuItem("Set", Guid.NewGuid()) { Reference = new Ref4(data.MethodInfo.ReturnType, pending) };

                }
                else if (pending.Input.Node.Data is { })
                {
                    var properties = typeof(ViewModelTree).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

                    foreach (var item in properties)
                    {
                        yield return new MenuItem(item.Name, Guid.NewGuid()) { Reference = new Ref(item, pending) };
                    }
                }

                yield break;
            }
            else if (viewModel is MenuItem menuItem)
            {
                if (menuItem.Reference is Ref2 ref2)
                {
                    foreach (var method in ref2.MethodInfos)
                    {
                        yield return new MenuItem(method.Name, Guid.NewGuid()) { Reference = new Ref3(ref2.Type, method) };
                    }
                }
                else if (menuItem.Reference is Ref3 { MethodInfo: { } methodInfo })
                {
                    yield break;
                }
                else if (menuItem.Reference is Type { } _type)
                {
                    if (menuItem.Key == "Method")
                    {
                        var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                        var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == _type))).ToArray();
                        foreach (var x in set)
                            yield return new MenuItem(x.Key, Guid.NewGuid()) { Reference = new Ref2(_type, x.Value) };
                    }

                }
                else if (menuItem.Reference is PendingConnectionViewModel { } pendingConnectionViewModel)
                {
                    //if (menuItem.Key == "Method")
                    //{
                    //    var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                    //    var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == _type))).ToArray();
                    //    foreach (var x in set)
                    //        yield return new MenuItem(x.Key, Guid.NewGuid()) { Reference = new Ref2(_type, x.Value) };
                    //}
                }
                else if (menuItem.Key == "Set" && menuItem.Reference is Ref4 { Type: { } type, ConnectionViewModel: { } _pending })
                {
                    var properties = typeof(ViewModelTree).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

                    foreach (var item in properties.Where(a => a.PropertyType == type))
                    {
                        yield return new MenuItem(item.Name, Guid.NewGuid()) { Reference = new Ref(item, _pending) };
                    }
                }
                yield break;
            }
            throw new NotImplementedException();
        }

        public INodeViewModel Find(object guid)
        {
            if (guid is IGetReference { Reference: Ref3 { Type: { } _type, MethodInfo: { } methodInfo } })
            {
                var m = new MethodNode(methodInfo, null);

                NodeViewModel nodeViewModel = new NodeViewModel { Data = m };
                foreach (var item in methodInfo.GetParameters().Select(a => a))
                {
                    var input = new ConnectorViewModel() { Key = item.Name, Data = m.InValues.Single(a => a.Key == item.Name).Value, Flow = Enums.ConnectorFlow.Input, Node = nodeViewModel };
                    nodeViewModel.Input.Add(input);
                }
                if (m.OutValue != null)
                {
                    //var x = new ConnectorViewModel() { Flow = Enums.ConnectorFlow.Input, Node = nodeViewModel };
                    var output = new ConnectorViewModel() { Flow = Enums.ConnectorFlow.Output, Node = nodeViewModel, Key = "output", Data = null };
                    //nodeViewModel.Input.Add(x);
                    nodeViewModel.Output.Add(output);
                }
                return nodeViewModel;
            }
            else if (guid is IGetReference { Reference: Ref { Info: { } info, ConnectionViewModel: { } connectionViewModel } })
            {
                var type = info.PropertyType;
                if (connectionViewModel.Input.Node is { Data: { } data })
                {
                    NodeViewModel nodeViewModel = null;
                    object observable = null;
                    if (data is INotifyPropertyChanged changed)
                    {
                        if (info.PropertyType == typeof(string))
                        {
                            observable = changed.WithChangesTo<INotifyPropertyChanged, string>(info);

                        }
                        else if (info.PropertyType == typeof(bool))
                        {
                            observable = changed.WithChangesTo<INotifyPropertyChanged, bool>(info);

                        }
                        else if (info.PropertyType == typeof(int))
                        {
                            observable = changed.WithChangesTo<INotifyPropertyChanged, int>(info);
                        }

                        nodeViewModel = new NodeViewModel { Data = observable };

                        var input = new ConnectorViewModel { Data = info, Key = "input" };
                        var output = new ConnectorViewModel { Data = info, Key = "output" };
                        nodeViewModel.Input.Add(input);
                        nodeViewModel.Output.Add(output);
                        input.Node = nodeViewModel;
                        output.Node = nodeViewModel;
                        return nodeViewModel;
                    }
                    else if (IsObservableOfType(data, info.PropertyType))
                    {

                        // add a placeholder data type to trick panel into arranging the item to the left
                        nodeViewModel = new NodeViewModel { Data = new Observable<string>(Array.Empty<IObservable<string>>()), IsConnectorsReversed = true };
                        var input = new ConnectorViewModel { Data = info, Node = nodeViewModel, Key = "input" };
                        var output = new ConnectorViewModel { Data = info, Node = nodeViewModel, Key = "output" };
                        
                        output
                            .WithChangesTo(a => a.IsConnected)
                            .Where(a => a)
                            .Subscribe(a =>
                            {
                                var connections = Globals.Resolver.Resolve<ViewModels.DiagramViewModel>().Connections.ToArray();
                                var last = connections.Last();
                                var connection = Globals.Resolver.Resolve<ViewModels.DiagramViewModel>().Connections.Where(a => a.Output== output).SingleOrDefault();
                                var x = connection.Input.Node.Data;

                                if (x is Tree { Data: Utility.PropertyDescriptors.MemberDescriptor _data })
                                {
                                    if (data is IObservable<string> str)
                                    {
                                        //str.Subscribe(a =>
                                        //{
                                        //});
                                    }
                                    else if (data is IObservable<bool> bl)
                                    {
                                        //bl.Subscribe(a =>
                                        //{
                                        //});
                                    }
                                    else if (data is IObservable<int> nt)
                                    {
                                        //nt.Subscribe(a =>
                                        //{
                                        //});
                                    }

                                    observable = new Ref5(_data, info);
                                    nodeViewModel.Data = observable;
                                    nodeViewModel.RaisePropertyChanged(nameof(nodeViewModel.Data));
                                }
                            });

                        nodeViewModel.Input.Add(input);
                        nodeViewModel.Output.Add(output);
                        //nodeViewModel.Output.Add(output2);
                        input.Node = nodeViewModel;
                        output.Node = nodeViewModel;
                        return nodeViewModel;
                    }
                }



                throw new NotImplementedException();
            }
            return new NodeViewModel() { Data = guid };
        }

        // Method 1: Using MakeGenericType to construct IObservable<T> at runtime
        public static bool IsObservableOfType(object obj, Type genericType)
        {
            if (obj == null) return false;

            var observableType = typeof(IObservable<>).MakeGenericType(genericType);
            return observableType.IsAssignableFrom(obj.GetType());
        }

    }
}
