using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Nodify.Operations.Infrastructure
{
    public class ObjectInfo
    {
        public ObjectInfo(Guid guid, Type type, string name)
        {
            Guid = guid;
            Type = type;
            Name = name;
        }

        public Guid Guid { get; set; }

        public Type Type { get; set; }

        public string Name { get; set; }
    }
}
