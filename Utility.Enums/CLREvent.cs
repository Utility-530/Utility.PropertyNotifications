using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Utility.Enums
{
    public enum CLREvent
    {
        None = 0,
        [Description("INotifyCollectionChanged")]
        CollectionChanged = 1,
        [Description("INotifyPropertyChanged")]
        PropertyChanged = 2,
        CustomEvent = 4 

    }
}
