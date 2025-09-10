using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Core;
using Utility.PropertyNotifications;

namespace Utility.Nodify.Engine
{
    public class ConnectionFactory : IConnectionFactory
    {
        public object CreateConnection(object output, object input)
        {
            if (output is IConnectorViewModel { Data: PropertyInfo propertyInfo, Node: IData { Data: INotifyPropertyChanged data } })
            {
                //data.WithChangesTo(propertyInfo).Subscribe(a => )
            }
            return null;
        }
    }
}
