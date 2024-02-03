using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.PropertyNotifications
{
    public interface IRaisePropertyChanged
    {
        public void RaisePropertyChanged(object value, string? propertyName = null);

    }
}
