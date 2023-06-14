using System.Collections;
using Utility.WPF.Demo.Data.Factory;

namespace Utility.WPF.Demo.Master.ViewModels
{
    public class ItemsWrapViewModel
    {
        public ItemsWrapViewModel()
        {
        }

        public IEnumerable Collection { get; } = new FieldsFactory().BuildCollection();
    }
}