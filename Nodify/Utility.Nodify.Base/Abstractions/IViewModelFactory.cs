using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Nodify.Core;

namespace Utility.Nodify.Base.Abstractions
{
    public interface IViewModelFactory
    {
        INodeViewModel CreateNode(object dataContext);
        IConnectorViewModel CreateConnector(object dataContext);
        IConnectorViewModel CreatePendingConnector(object dataContext);
        IConnectionViewModel CreateConnection(IConnectorViewModel source, IConnectorViewModel? target);
        bool CanCreateConnection(IConnectorViewModel source, IConnectorViewModel? target);
    }
}
