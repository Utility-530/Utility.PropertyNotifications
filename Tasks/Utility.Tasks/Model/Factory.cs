using Utility.Interfaces.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using Utility.Interfaces.NonGeneric;

namespace Utility.Tasks.Model
{



    public abstract class Factory : IFactory
    {
        public abstract object Create(object args);
    }


    public enum FactoryStatus
    {
        Set,
        Scheduled,
        Created
    }

    public class FactoryRequest<TArguments> : FactoryRequest, IKey<string>
    {
        public FactoryRequest(string key, DateTime? scheduledTime, TArguments arguments) : base(key, scheduledTime)
        {        
            Arguments = arguments;
        }

        public TArguments Arguments { get; }
    }

    public class FactoryRequest : IKey<string>
    {
        public FactoryRequest(string key, DateTime? scheduledTime)
        {
            Key = key;
            ScheduledTime = scheduledTime;
        }

        public string Key { get; }

        public DateTime? ScheduledTime { get; }

        public bool Equals(IKey<string> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IEquatable other)
        {
            throw new NotImplementedException();
        }
    }

    public class FactoryOrder : IKey<string>
    {
        public FactoryOrder(string key, FactoryStatus factoryStatus = FactoryStatus.Set, DateTime? scheduled = default, DateTime? creation = default)
        {
            Key = key;
            Creation = creation;
            Scheduled = scheduled;
            State = factoryStatus;
        }

        public string Key { get; }

        public DateTime? Creation { get; }

        /// <summary>
        /// The scheduled time for the order's completion
        /// </summary>
        public DateTime? Scheduled { get; }

        public FactoryStatus State { get; }

        public bool Equals(IKey<string> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IEquatable other)
        {
            throw new NotImplementedException();
        }
    }

    public class FactoryWrapper<TItem>
    {
        public FactoryWrapper(FactoryOrder id, TItem item)
        {
            Id = id;
            Item = item;
        }

        public FactoryOrder Id { get; }

        public TItem Item { get; }
    }

}
