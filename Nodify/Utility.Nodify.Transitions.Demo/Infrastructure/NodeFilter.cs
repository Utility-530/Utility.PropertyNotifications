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
using Utility.Interfaces;
using System.Collections;
using Utility.Interfaces.Generic;

namespace Utility.Nodify.Transitions.Demo.Infrastructure
{
    record Ref(PropertyInfo Info, IConnectionViewModel ConnectionViewModel);
    record Ref2(Type Type, string Key, List<MethodInfo> MethodInfos);
    record Ref3(Type Type, MethodInfo MethodInfo) : IType;
    record Ref4(Type Type, IConnectionViewModel ConnectionViewModel) : IType;
    record Ref5(Utility.PropertyDescriptors.MemberDescriptor MemberDescriptor, PropertyInfo Info)
    {
        public override string ToString()
        {
            return MemberDescriptor.Name + "<-" + Info.Name;
        }
    }

    public class NodeFactory : IFactory<INodeViewModel>
    {
        public INodeViewModel Create(object guid)
        {
            if (guid is Ref3 { Type: { } _type, MethodInfo: { } methodInfo } )
            {
                return Globals.Resolver.Resolve<IViewModelFactory>().CreateNode(methodInfo);
            }
            throw new Exception("sd dd");
        }
    }

    public class NodeFilter : IEnumerableFactory
    {

        public NodeFilter()
        {
        }

        public IEnumerable Create(object viewModel)
        {
            if (viewModel is IConnectionViewModel pending)
            {
                if (pending.Output is IGetData { Data: PropertyInfo { PropertyType: Type _type } propertyInfo })
                {
                    var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                    var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == _type))).ToArray();
                    foreach (var x in set)
                        yield return new Ref2(_type, x.Key, x.Value);
                }
                else if (pending.Output is IGetData { Data: ParameterInfo { ParameterType: Type type } })
                {
                    var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                    var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == type))).ToArray();
                    foreach (var x in set)
                        yield return new Ref2(type, x.Key, x.Value);
                }
                yield break;
            }
            else if (viewModel is INodeViewModel menuItem)
            {
                if (menuItem.Data() is Ref2 ref2)
                {
                    foreach (var method in ref2.MethodInfos)
                    {
                        yield return new Ref3(ref2.Type, method);
                    }
                }
                else if (menuItem.Data() is Ref3 { MethodInfo: { } methodInfo })
                {
                    yield break;
                }
                else if (menuItem.Data() is Type { } _type)
                {
                    if (menuItem.Name() == "Method")
                    {
                        var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                        var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == _type))).ToArray();
                        foreach (var x in set)
                            yield return new Ref2(_type, x.Key, x.Value);
                    }

                }
                else if (menuItem.Data() is IPendingConnectionViewModel { } pendingConnectionViewModel)
                {
                    //if (menuItem.Key == "Method")
                    //{
                    //    var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                    //    var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == _type))).ToArray();
                    //    foreach (var x in set)
                    //        yield return new MenuItem(x.Key, Guid.NewGuid()) { Reference = new Ref2(_type, x.Value) };
                    //}
                }
                else if (menuItem.Name() == "Set" && menuItem.Data() is Ref4 { Type: { } type, ConnectionViewModel: { } _pending })
                {
                    var properties = typeof(NodeViewModel).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

                    foreach (var item in properties.Where(a => a.PropertyType == type))
                    {
                        //yield return new MenuItem(item.Name, Guid.NewGuid(), this) { Reference = new Ref(item, _pending) };
                        yield return new Ref(item, _pending);
                    }
                }
                yield break;
            }
            else if (viewModel is Ref2 ref2)
            {
                foreach (var method in ref2.MethodInfos)
                {
                    yield return new Ref3(ref2.Type, method);
                }
            }
            else if (viewModel is Ref3 { MethodInfo: { } methodInfo })
            {
                yield break;
            }
            else if (viewModel is Type { } _type)
            {
                throw new NotImplementedException("dsd 54");
                //if (menuItem.Name() == "Method")
                //{
                //    var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                //    var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == _type))).ToArray();
                //    foreach (var x in set)
                //        yield return new Ref2(_type, x.Key, x.Value);
                //}

            }
            else if (viewModel is IPendingConnectionViewModel { } pendingConnectionViewModel)
            {
                //if (menuItem.Key == "Method")
                //{
                //    var asses = AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().ToArray();
                //    var set = asses.SelectMany(a => a.GetPublicStaticMethodsGroupedByClass(ca => ca.Any(da => da == _type))).ToArray();
                //    foreach (var x in set)
                //        yield return new MenuItem(x.Key, Guid.NewGuid()) { Reference = new Ref2(_type, x.Value) };
                //}
            }
            else if (viewModel is Ref4 { Type: { } type, ConnectionViewModel: { } _pending })
            {
                var properties = typeof(NodeViewModel).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

                foreach (var item in properties.Where(a => a.PropertyType == type))
                {
                    //yield return new MenuItem(item.Name, Guid.NewGuid(), this) { Reference = new Ref(item, _pending) };
                    yield return new Ref(item, _pending);
                }
            }
            yield break;
        }

        //public INodeViewModel Find(object guid)
        //{
        //    if (guid is IGetReference { Reference: Ref3 { Type: { } _type, MethodInfo: { } methodInfo } })
        //    {

        //        var nodeViewModel = Globals.Resolver.Resolve<IViewModelFactory>().CreateNode(methodInfo);
        //        return nodeViewModel;
        //    }
        //    throw new Exception("sd dd");

        //}

        // Method 1: Using MakeGenericType to construct IObservable<T> at runtime
        public static bool IsObservableOfType(object obj, Type genericType)
        {
            if (obj == null) return false;

            var observableType = typeof(IObservable<>).MakeGenericType(genericType);
            return observableType.IsAssignableFrom(obj.GetType());
        }

    }
}
