using DryIoc;
using LanguageExt.Pipes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Diagrams;
using Utility.Nodes;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Operations.Infrastructure;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.ServiceLocation;
using Utility.Trees;

namespace Utility.Nodify.Transitions.Demo.Infrastructure
{
    public class NodeSource : INodeSource
    {
        private readonly DryIoc.IContainer container;

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

        public NodeSource(DryIoc.IContainer container)
        {
            this.container = container;
        }

        public IEnumerable<MenuItem> Filter(object viewModel)
        {
            if (viewModel is IConnectionViewModel pending)
            {
                if (pending.Output is IGetData { Data: PropertyInfo { PropertyType: Type _type } propertyInfo })
                {
                    var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                    var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == _type))).ToArray();
                    foreach (var x in set)
                        yield return new MenuItem(x.Key, Guid.NewGuid(), container.Resolve<INodeSource>()) { Reference = new Ref2(_type, x.Value) };
                }
                else if (pending.Output is IGetData { Data: ParameterInfo { ParameterType: Type type } })
                {
                    var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                    var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == type))).ToArray();
                    foreach (var x in set)
                        yield return new MenuItem(x.Key, Guid.NewGuid(), container.Resolve<INodeSource>()) { Reference = new Ref2(type, x.Value) };
                }
                yield break;
            }
            else if (viewModel is MenuItem menuItem)
            {
                if (menuItem.Reference is Ref2 ref2)
                {
                    foreach (var method in ref2.MethodInfos)
                    {
                        yield return new MenuItem(method.Name, Guid.NewGuid(), container.Resolve<INodeSource>()) { Reference = new Ref3(ref2.Type, method) };
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
                            yield return new MenuItem(x.Key, Guid.NewGuid(), container.Resolve<INodeSource>()) { Reference = new Ref2(_type, x.Value) };
                    }

                }
                else if (menuItem.Reference is IPendingConnectionViewModel { } pendingConnectionViewModel)
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
                    var properties = typeof(NodeViewModel).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

                    foreach (var item in properties.Where(a => a.PropertyType == type))
                    {
                        yield return new MenuItem(item.Name, Guid.NewGuid(), container.Resolve<INodeSource>()) { Reference = new Ref(item, _pending) };
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

                var nodeViewModel = container.Resolve<IViewModelFactory>().CreateNode(methodInfo);
                return nodeViewModel;
            }
            throw new Exception("sd dd");

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
