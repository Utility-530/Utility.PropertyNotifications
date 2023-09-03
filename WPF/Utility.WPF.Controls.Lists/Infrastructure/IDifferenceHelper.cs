using System.Collections.Generic;
using Utility.WPF.Models;

namespace Utility.WPF.Controls.Lists.Infrastructure
{
    internal interface IDifferenceHelper
    {
        IEnumerable<CheckedRoutedEventArgs.ChangedItem> Get { get; }
    }
}