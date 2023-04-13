using System.Collections;

namespace Utility.WPF.Abstract
{
    public interface IItemsSource
    {
        IEnumerable ItemsSource { get; set; }
    }
}