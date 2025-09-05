using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.Helpers.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodes;
using Utility.Nodify.Core;
using Utility.Nodify.Entities;
using Utility.Nodify.Models;
using Utility.Nodify.Operations.Infrastructure;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;
using Node = Utility.Nodify.Entities.Node;

namespace Utility.Nodify.Transitions.Demo.Infrastructure
{


    public class NodeSource : INodeSource
    {
        record Ref(PropertyInfo Info, IConnectionViewModel ConnectionViewModel);
        record Ref2(Type Type, List<MethodInfo> MethodInfos);
        record Ref3(Type Type, MethodInfo MethodInfo) : IType;

        public IEnumerable<MenuItem> Filter(object viewModel)
        {

            if (viewModel is IConnectionViewModel pending)
            {
                if (pending.Input.Type is { } type)
                {
                    var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                    var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == type))).ToArray();
                    foreach (var x in set)
                        yield return new MenuItem(x.Key, Guid.NewGuid()) { Reference = new Ref2(type, x.Value) };
                }
                else if (pending.Input.Node.Data is MethodNode data)
                {
                    var properties = typeof(ViewModelTree).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

                    var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                    var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == data.OutValue.Type))).ToArray();
                    foreach (var x in set)
                        yield return new MenuItem(x.Key, Guid.NewGuid()) { Reference = new Ref2(data.OutValue.Type, x.Value) };

                }
                else if (pending.Input.Node.Data is { })
                {
                    var properties = typeof(ViewModelTree).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

                    foreach (var item in properties)
                    {
                        yield return new MenuItem(item.Name, Guid.NewGuid()) { Reference = new Ref(item, pending) };
                    }
                    //return new List<MenuItem>
                    //{
                    //    new MenuItem("Math", Guid.NewGuid()),
                    //    new MenuItem("Logic", Guid.NewGuid()),
                    //    new MenuItem("Flow", Guid.NewGuid()),
                    //};
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
                else if (menuItem.Key == "Math")
                {

                    //yield return new MenuItem("Add", Guid.NewGuid());
                    //yield return new MenuItem("Subtract", Guid.NewGuid());
                    //yield return new MenuItem("Multiply", Guid.NewGuid());
                    //yield return new MenuItem("Divide", Guid.NewGuid());

                }
                yield break;
            }
            throw new NotImplementedException();
        }

        public INodeViewModel Find(object guid)
        {

            if (guid is IGetReference { Reference: Ref3 { Type: { } _type, MethodInfo: { } methodInfo } })
            {
                var m = new MethodNode(new Method(methodInfo, null));

                NodeViewModel nodeViewModel = new NodeViewModel { Data = m };
                foreach (var item in methodInfo.GetParameters().Select(a => a))
                {
                    var input = new ConnectorViewModel() { Title = item.Name, Data = m.InValues.Single(a => a.Key == item.Name).Value, Flow = Enums.ConnectorFlow.Input, Node = nodeViewModel };
                    nodeViewModel.Input.Add(input);
                }
                if (m.OutValue != null)
                {
                    //var x = new ConnectorViewModel() { Flow = Enums.ConnectorFlow.Input, Node = nodeViewModel };
                    var output = new ConnectorViewModel() { Flow = Enums.ConnectorFlow.Output, Node = nodeViewModel };
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
                            var obs = changed.WithChangesTo<INotifyPropertyChanged, string>(info);
                            observable = new Observable<string>([obs]);

                        }
                        else if (info.PropertyType == typeof(bool))
                        {
                            var obs = changed.WithChangesTo<INotifyPropertyChanged, bool>(info);
                            observable = new Observable<bool>([obs]);


                        }
                        else if (info.PropertyType == typeof(int))
                        {
                            var obs = changed.WithChangesTo<INotifyPropertyChanged, int>(info);
                            observable = new Observable<int>([obs]);

                        }

                        nodeViewModel = new NodeViewModel { Data = observable };

                        var input = new ConnectorViewModel { Type = info.PropertyType };
                        var output = new ConnectorViewModel { Type = info.PropertyType };
                        nodeViewModel.Input.Add(input);
                        nodeViewModel.Output.Add(output);
                        input.Node = nodeViewModel;
                        output.Node = nodeViewModel;
                        return nodeViewModel;
                    }

                }

                throw new NotImplementedException();
            }
            return new NodeViewModel() { Data = guid };
        }
    }
}
