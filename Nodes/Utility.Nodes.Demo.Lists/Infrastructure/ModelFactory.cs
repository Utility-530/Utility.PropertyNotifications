using System.Reactive.Linq;
using Utility.Attributes;
using Utility.Entities;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using static Utility.API.Services.Coinbase;

namespace Utility.Nodes.Demo.Lists
{
    public class ModelFactory : IFactory<IId<Guid>>
    {
        public IId<Guid> Create(object config)
        {
            if (config is Type type)
            {
                if (type.GetConstructors().SingleOrDefault(a => a.TryGetAttribute<FactoryAttribute>(out var x)) is { } x)
                {
                    return (IId<Guid>)x.Invoke(new[] { default(object) });
                }
                if (type == typeof(UserProfile))
                {
                    return new UserProfile() { Id = Guid.NewGuid(), AddDate = DateTime.Now };
                }
                if (type == typeof(AuctionItem))
                {
                    return new AuctionItem { Id = Guid.NewGuid(), Added = DateTime.Now };
                }
                if (type == typeof(Transaction))
                {
                    return new Transaction() { };
                }
                if (type == typeof(CoinbaseTransaction))
                {
                    return new CoinbaseTransaction() { };
                }
                if (Activator.CreateInstance(type) is IId<Guid> iid)
                {
                    if (iid is IIdSet<Guid> set)
                        set.Id = Guid.NewGuid();
                    return iid;
                }
                else
                {
                    throw new Exception("33889__e");
                }
            }
            else
            {
                throw new Exception("545 fgfgddf");
            }
        }
    }
}