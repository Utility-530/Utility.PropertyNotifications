using System.Diagnostics.CodeAnalysis;
using Utility.Infrastructure.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Enums;
using Utility.Infrastructure;
using Utility.Models;

namespace Utility.PropertyTrees.Infrastructure
{
    public class PropertyStore : BaseObject
    {
        private readonly IRepository repository;
        public override Key Key => new(Guid, nameof(PropertyStore), typeof(PropertyStore));

        public PropertyStore(IRepository repository)
        {
            this.repository = repository;
        }

        public static Guid Guid => Guid.Parse("f04c2e55-bf33-480c-a4e7-b4b7804d1735");

        public override void OnNext(object value)
        {
            if (value is PropertyOrder order)
                Process(order);         
            if (value is GuidValue findOrder)
                Process2(findOrder);
            else
                base.OnNext(value);


            async void Process2(GuidValue order)
            {
                if (order is GuidValue { Guid:var guid,  Value: FindOrder { Key: var key } })
                {
                    var childKey = await repository.FindKeyByParent(key);
                    Broadcast(new GuidValue(guid, new FindResult(childKey), 0));
                }
            }

            async void Process(PropertyOrder order)
            {
                order.Progress = 0;
                switch (order.Access)
                {
                    case Access.Get:
                        {
                            try
                            {
                                var guid = await repository.FindKeyByParent(order.Key);
                                order.Progress = 50;
                                var find = await repository.FindValue(guid);
                                order.Progress = 100;

                                if (find != null)
                                {
                                    Update(find, order);
                                }
                            }
                            catch (Exception ex)
                            {
                                order.Exception = ex;
                            }

                            break;
                        }
                    case Access.Set:
                        {
                            try
                            {
                                var guid = await repository.FindKeyByParent(order.Key);
                                order.Progress = 50;
                                var find = await repository.FindValue(guid);
                                await repository.UpdateValue(guid, order.Value);
                                order.Progress = 100;
                                Update(find, order);
                            }
                            catch (Exception ex)
                            {
                                order.Exception = ex;
                            }

                            break;
                        }
                }
            }
        }
        private void Update(object newValue, PropertyOrder order)
        {
            Broadcast(new PropertyChange(order.Key, newValue, order.Value));
        }




        private class KeyComparer : IEqualityComparer<IEquatable>
        {
            public bool Equals(IEquatable? x, IEquatable? y)
            {
                return x.Equals(y);
            }

            public int GetHashCode([DisallowNull] IEquatable obj)
            {
                return obj.GetHashCode();
            }
        }
    }

    public record FindOrder(Key Key);
    public record FindResult(IEquatable Key);
}