using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Nodify.Core;

namespace Utility.Nodify.Base.Abstractions
{
    public interface IConnectorFactory
    {
        IConnectorViewModel Create(object obj);
    }
}
