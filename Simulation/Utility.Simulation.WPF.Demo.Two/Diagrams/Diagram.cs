using DryIoc;
using System.Collections.Generic;
using System.Reflection;
using System;
using Utility.Nodify.Core;
using Utility.Nodify.Operations;
using IContainer = DryIoc.IContainer;
using System.Linq;
using Utility.Interfaces.NonGeneric;
using Utility.Helpers;
using Utility.Observables.Generic;
using Utility.Observables.NonGeneric;
using NetFabric.Hyperlinq;

namespace Utility.Nodify.Demo.Infrastructure
{
    public class Diagram1 : Diagram
    {
        Dictionary<Type, List<ConnectorViewModel>> inputs = new();
        Dictionary<Type, List<ConnectorViewModel>> outputs = new();

        public Diagram1(IContainer container)
        {
            Name = "One";

            var instances = container.Resolve<object[]>();

            List<OperationNodeViewModel> nodes = new();
            List<OperationConnectionViewModel> connections = new();

            foreach (var instance in instances)
            {
                var name = instance.GetType().Name;
                var opNode = new OperationNodeViewModel { Title = name, Location = new System.Windows.Point(600, 300), };

                foreach (var input in ReflectionHelper.Inputs(instance))
                {
                    var connector = new ConnectorViewModel() { Title = input.Name };
                    opNode.Input.Add(connector);
                    inputs.GetValueOrNew(input).Add(connector);
                }
                foreach (var output in ReflectionHelper.Outputs(instance))
                {
                    var connector = new ConnectorViewModel() { Title = output.Name };
                    opNode.Output.Add(connector);
                    outputs.GetValueOrNew(output).Add(connector);
                }
                var methods = instance.GetSingleParameterMethods();

                var operationInfo = new OperationInfo
                {
                    Title = name,
                    Type = OperationType.Normal,
                    Operation = new LambdaOperation(a =>
                    {
                        var types = a.Where(c => c.Value != default).ToDictionary(v => v.Value.GetType().GUID, v => v.Value);
                        foreach (var typei in types)
                        {
                            methods[typei.Key].OnNext(typei.Value);
                        }

                        return Array.Empty<IOValue>();
                    }),
                    MinInput = 1,
                    MaxInput = 1
                };

                container.RegisterInstance(operationInfo);


                ReflectionHelper.TrySubscribe(instance, (a) =>
                        {
                            var name = a.GetType().Name;

                            foreach (var typei in opNode.Output)
                            {
                                if (typei.Title == name)
                                    typei.Value = a;
                            }

                        }, (e) => { }, () => { }, (a, b) => { });

                nodes.Add(opNode);
            }


            foreach (var o in outputs)
            {
                foreach (var i in inputs)
                {
                    if (o.Key.Equals(i.Key))
                    {
                        foreach (var input in i.Value)
                            foreach (var output in o.Value)
                            {
                                var opConnection = new OperationConnectionViewModel { Output = output, Input = input };
                                connections.Add(opConnection);
                            }
                    }
                }
            }
            Nodes = nodes.ToArray();
            Connections = connections.ToArray();
        }

        public override OperationConnectionViewModel[] Connections { get; }

        public override NodeViewModel[] Nodes { get; }
    }




    static class ReflectionHelper
    {
        public static IEnumerable<Type> Inputs(object instance)
        {

            foreach (Type intType in instance.GetType().GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition()
                    == typeof(IObserver<>))
                {
                    yield return intType.GetGenericArguments()[0];
                }
            }
        }

        public static IEnumerable<Type> Outputs(object instance)
        {

            foreach (Type intType in instance.GetType().GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition()
                    == typeof(IObservable<>))
                {
                    yield return intType.GetGenericArguments()[0];
                }
            }
        }

        public static Dictionary<Guid, SingleParameterMethod> GetSingleParameterMethods(this object instance)
        {
            return instance.GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => m.Name == nameof(IObserver.OnNext) && m.GetParameters().Length == 1)
                        .ToDictionary(m =>
                        m.GetParameters().Single().ParameterType.GUID,
                        m => new SingleParameterMethod(instance, m));
        }

        public static bool TrySubscribe(object instance, Action<object> action, Action<Exception> onError, Action onCompleted, Action<int, int> onProgress)
        {
            var methods = instance
                            .GetType()
                            .GetMethods(/*BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly*/);

            var single = methods
                            .SingleOrDefault(m => m.Name == nameof(IObservable.Subscribe));

            // if return type is not an observable
            if (single == default)
            {

                if (instance.TryGetPrivateFieldValue("_source", out var source))
                    return TrySubscribe(source, action, onError, onCompleted, onProgress);
                else if (instance.TryGetPrivateFieldValue("_value", out var value))
                {
                    action.Invoke(value);
                    return true;
                }
                return false;
            }

            var arg = Outputs(instance).SingleOrDefault();
            if (arg != null)
            {
                var observer = Activator.CreateInstance(
                            typeof(Observer<>).MakeGenericType(arg), action, onError, onCompleted, onProgress);

                single.Invoke(instance, new[] { observer });
            }
            else
            {
                single.Invoke(instance, new[] { new Observer(action, onError, onCompleted, onProgress) });
            }

            return true;
        }
    }


    public class SingleParameterMethod
    {
        private readonly MethodInfo methodInfo;
        private readonly object instance;

        public SingleParameterMethod(object instance, MethodInfo methodInfo)
        {
            InType = methodInfo.GetParameters().Single().ParameterType;
            OutType = methodInfo.ReturnType;
            this.instance = instance;
            this.methodInfo = methodInfo;
        }

        //public Guid Key => Combine(instance.Key.Guid, InType.GUID);

        public Type InType { get; }

        public Type OutType { get; }

        public void OnNext(object parameter)
        {
            object? output;
            try
            {
                methodInfo.Invoke(instance, new object[] { parameter });
            }
            catch (Exception ex)
            {
                throw;
                //instance.OnNext(new GuidValue(GuidBase.OnError(GetGuid(), ex), Guid, parameter));
                //return;
            }
        }



        //public void OnProgress(int arg1, int arg2)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnCompleted()
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnError(Exception error)
        //{
        //    throw new NotImplementedException();
        //}

        public bool Equals(SingleParameterMethod? other)
        {
            return other?.InType.Equals(this.InType) ==
                other?.OutType.Equals(this.OutType) == true;
        }

        public override string ToString()
        {
            return InType.Name.ToString() + " SingleParameter";
        }


    }

}
