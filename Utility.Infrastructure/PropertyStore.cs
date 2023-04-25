using System.Diagnostics.CodeAnalysis;
using Utility.Infrastructure.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Enums;
using Utility.Infrastructure;
using Utility.Models;

namespace Utility.PropertyTrees.Infrastructure
{
    public class PropertyStore : BaseObject, IPropertyStore
    {
        private readonly IRepository repository;
        public override Key Key => new(Guid, nameof(PropertyStore), typeof(PropertyStore));

        public PropertyStore(IRepository repository)
        {
            this.repository = repository;
        }

        public Guid Guid => Guid.Parse("f04c2e55-bf33-480c-a4e7-b4b7804d1735");

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public async void OnNext(object value)
        {
            if (value is not Order order)
            {
                throw new Exception("g 3434 3");
            }

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

        private void Update(object newValue, Order order)
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
}