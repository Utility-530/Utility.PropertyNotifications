using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Attributes;
using Utility.Entities;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using Utility.Models;

namespace Utility.Nodes.Demo.Editor.Infrastructure
{
    public class ModelFactory : IFactory<IId<Guid>>
    {
        public IId<Guid> Create(object config)
        {
            if (config is Type type)
            {
                //if (type.GetConstructors().SingleOrDefault(a => a.TryGetAttribute<FactoryAttribute>(out var x)) is { } x)
                //{
                //    return (IId<Guid>)x.Invoke(new[] { default(object) });
                //}
                if (type == typeof(Model<string>))
                {
                    return (IId<Guid>)new Model<string>() { Guid = Guid.NewGuid(), Value = "New" };
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
