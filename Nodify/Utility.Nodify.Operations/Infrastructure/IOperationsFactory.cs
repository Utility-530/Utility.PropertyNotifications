using Utility.Nodify.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Nodify.Operations.Infrastructure
{
    public interface IOperationsFactory
    {
        IEnumerable<OperationInfo> GetOperations();
    }
}
