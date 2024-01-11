using System.ComponentModel;
using System.Reactive.Linq;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.Generic;
using Utility.Observables.NonGeneric;
using System.Runtime.CompilerServices;
using Utility.Objects;
using Utility.Models;

namespace Utility.Nodes
{

    public class PropertyNode : Node
    {
        private PropertyData data;
        bool flag;

        public PropertyNode(PropertyData propertyData)
        {
            if (propertyData == null)
            {

            }
            this.data = propertyData;
        }

        public override object Data => data;// Activator.CreateInstance(data);

        public override IEquatable Key => null;// new CombinationKey(new[] { Parent.Key, new StringKey(data.Descriptor.Name) });

        public override async Task<object?> GetChildren()
        {
            flag = true;
            var children = await ChildPropertyExplorer.Convert(data.Instance, data.Descriptor);
            return children.Select(a => new PropertyData(a.Descriptor.GetValue(data.Instance), a.Descriptor)).ToArray();
        }

        public override string ToString()
        {
            //return data.Name;
            return "";
        }

        public override Node ToNode(object value)
        {
            return new PropertyNode(value as PropertyData);
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }

    public record PropertyData(object Instance, PropertyDescriptor Descriptor) : IValue
    {
        public object? Value => Instance;
    }


    public static class OnNextAwaiterHelper
    {
        public static NotifyOnCompletion<T> GetAwaiter<T>(this IObservable<T> lazy) => new NotifyOnCompletion<T>(lazy);
    }


    public class NotifyOnCompletion<T> : INotifyCompletion
    {
        private IDisposable disposable;
        private bool isCompleted;
        private List<T> values = new();

        public NotifyOnCompletion(IObservable<T> observable)
        {
            disposable =
                observable.Subscribe(a =>
                {
                    values.Add(a);
                }, () => isCompleted = true);
        }

        public void OnCompleted(Action continuation)
        {
            disposable.Dispose();
            continuation();
        }

        public List<T> GetResult() => values;

        public bool IsCompleted => isCompleted;
    }
}


