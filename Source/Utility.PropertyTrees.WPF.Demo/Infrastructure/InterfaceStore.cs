using System;
using System.Collections.Generic;
using Utility.Infrastructure;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.PropertyTrees.Abstractions;

namespace Utility.PropertyTrees.WPF.Demo.Infrastructure
{
    internal class InterfaceStore : BaseObject, IObserver
    {
        Dictionary<Type, Type> dictionary = new() { { typeof(IViewModel), typeof(ViewModel) } };

        public InterfaceStore()
        {
        }

        Guid Guid = Guid.Parse("8eeb7df3-9ae7-49d6-aabc-a492c6254718");

        public override Key Key => new(Guid, nameof(InterfaceStore), typeof(InterfaceStore));

        public override void OnNext(object value)
        {
            if (value is GuidValue { Value: Type type } keyType)
            {
                Broadcast(new GuidValue(keyType.Guid, dictionary[type], 0));
            }
        }
    }
}
