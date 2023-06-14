using System.Collections;
using Utility.WPF.Events;

namespace Utility.WPF.Abstract
{
    public interface ICheckedSelector
    {
        IEnumerable CheckedItems { get; }
        IEnumerable UnCheckedItems { get; }

        event CheckedChangedEventHandler CheckedChanged;
    }
}