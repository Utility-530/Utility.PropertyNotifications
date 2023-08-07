using System.Reactive.Disposables;
using System.Reflection;
using Utility.Helpers;
using Utility.Infrastructure;
using Utility.Models;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Utility.PropertyTrees.Services
{
    public class MethodsExplorer : BaseObject
    {
        readonly Dictionary<Type, MethodInfo[]> cachedMethodInfos = new();
        readonly Dictionary<Type, MethodInfo[]> cachedParentMethodInfos = new();
        readonly Dictionary<MethodInfo, bool> methodInfoIncludes = new();

        static readonly string[] validParentMethods = new[] { "Remove", "MoveUp", "MoveDown" };
        static readonly string[] validChildMethods = new[] { "Add", "Clear", "Send", "Connect", "Foo", "Bar", "AddByName", "AddByKey", "AddByType", "Refresh", "Update" };
        static readonly Dictionary<Type, string[]> inValidMethods = new(new ObjectAncestryComparer()) {
            {typeof(object), new[] { "ToString", "GetType", "Equals", "GetHashCode" } },
                 };

        static readonly Dictionary<string, string[]> inValidMethodsByName = new() {
            {"ViewModelsCollection", new[] { "Add", "Clear" } },
                 };

        public override Key Key => new(Guids.MethodsExplorer, nameof(MethodsExplorer), typeof(MethodsExplorer));

        public Interfaces.Generic.IObservable<MethodsResponse> OnNext(MethodsRequest value)
        {
            var infos = cachedMethodInfos.GetValueOrCreate(value.Descriptor.PropertyType, () => MethodInfos(value.Descriptor).ToArray());
            var parentInfos = cachedParentMethodInfos.GetValueOrCreate(value.Descriptor.PropertyType, () => ParentMethodInfos(value.Descriptor).ToArray());
            var total = infos.Length + parentInfos.Length;
            int i = 0;
            CompositeDisposable composite = new();

            return Create<MethodsResponse>(observer =>
            {
                if (total == 0)
                    observer.OnCompleted();
                foreach (var info in infos)
                {

                    if (methodInfoIncludes.TryGetValue(info, out bool include) == false)
                        Observe<MethodInfoFilterResponse, MethodInfoFilterRequest>(new MethodInfoFilterRequest(info))
                        .Subscribe(a =>
                        {
                            SubscribeToMethodInfo(info, methodInfoIncludes[info] = a.Include, observer);
                        })
                        .DisposeWith(composite);
                    else
                        SubscribeToMethodInfo(info, include, observer);
                }
                foreach (var info in parentInfos)
                {

                    if (methodInfoIncludes.TryGetValue(info, out bool include) == false)
                        Observe<MethodInfoFilterResponse, MethodInfoFilterRequest>(new MethodInfoFilterRequest(info))
                        .Subscribe(a =>
                        {
                            SubscribeToParentMethodInfo(info, methodInfoIncludes[info] = a.Include, observer);
                        })
                        .DisposeWith(composite);
                    else
                        SubscribeToParentMethodInfo(info, include, observer);
                }

                return composite;
            });

            void SubscribeToMethodInfo(MethodInfo methodInfo, bool include, Interfaces.Generic.IObserver<MethodsResponse> obs)
            {
                NodeFromMethodInfo()
                       .Subscribe(node =>
                       {
                           i++;
                           obs.OnNext(new(node, methodInfo));
                           if (i == total)
                               obs.OnCompleted();
                       }).DisposeWith(composite);


                Interfaces.Generic.IObservable<MethodNode?> NodeFromMethodInfo()
                {
                    return Create<MethodNode>(observer =>
                    {
                        return Observe<MethodActivationResponse, MethodActivationRequest>(new(value.Guid, methodInfo, value.Data))
                            .Subscribe(a =>
                            {
                                observer.OnNext(a.Node);
                            });
                    });
                }
            }

            void SubscribeToParentMethodInfo(MethodInfo methodInfo, bool include, Interfaces.Generic.IObserver<MethodsResponse> obs)
            {
                NodeFromMethodInfo()
                       .Subscribe(node =>
                       {
                           i++;
                           obs.OnNext(new(node, methodInfo));
                           if (i == total)
                               obs.OnCompleted();
                       }).DisposeWith(composite);


                Interfaces.Generic.IObservable<MethodNode?> NodeFromMethodInfo()
                {
                    return Create<MethodNode>(observer =>
                    {
                        return Observe<MethodActivationResponse, MethodActivationRequest>(new(value.Guid, methodInfo, value.ParentData))
                            .Subscribe(a =>
                            {
                                observer.OnNext(a.Node);
                            });
                    });
                }
            }
        }

        static IEnumerable<MethodInfo> MethodInfos(PropertyDescriptor propertyDescriptor)
        {
            return
               propertyDescriptor
               .PropertyType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance/* | BindingFlags.DeclaredOnly*/)
                .Where(m => !m.IsSpecialName)
                .Where(a => Filter(propertyDescriptor, a))
                .Where(a => validChildMethods.Contains(a.Name))
                .Cast<MethodInfo>()
                .OrderBy(d => d.Name);
        }

        private static bool Filter(PropertyDescriptor propertyDescriptor, MethodInfo methodInfo)
        {
            var b1 = (inValidMethods.TryGetValue(propertyDescriptor.PropertyType, out var o) && o.Contains(methodInfo.Name));
            var b2 = (inValidMethodsByName.TryGetValue(propertyDescriptor.PropertyType.Name, out var o2) && o2.Contains(methodInfo.Name));
            return b1 == false && b2 == false;
        }

        static IEnumerable<MethodInfo> ParentMethodInfos(PropertyDescriptor propertyDescriptor)
        {

            if (propertyDescriptor.ComponentType != null)
            {

                return propertyDescriptor
                    .ComponentType
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance/* | BindingFlags.DeclaredOnly*/)
                    .Where(m => !m.IsSpecialName)
                    .Where(m => m.GetParameters().Select(a => a.ParameterType).Contains(propertyDescriptor.PropertyType))
                    .Where(m => validParentMethods.Contains(m.Name))
                    .Cast<MethodInfo>()
                    .OrderBy(d => d.Name);
            }
            return Array.Empty<MethodInfo>();
        }

        public class ObjectAncestryComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type? x, Type? y)
            {
                return x.IsAssignableTo(y) || y.IsAssignableTo(x);
            }

            public int GetHashCode([DisallowNull] Type obj)
            {
                return 0;
            }
        }
    }
}