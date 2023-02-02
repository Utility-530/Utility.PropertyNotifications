using System.Collections.Generic;

namespace Utility.WPF.Controls.Lists.Infrastructure
{
    internal interface IDifferenceHelper
    {
        IEnumerable<CheckedRoutedEventArgs.ChangedItem> Get { get; }
    }
}