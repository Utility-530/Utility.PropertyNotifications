using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Nodify.Base.Abstractions
{
    public interface IConnectionFactory
    {
        public object CreateConnection(object output, object input);
    }
}
