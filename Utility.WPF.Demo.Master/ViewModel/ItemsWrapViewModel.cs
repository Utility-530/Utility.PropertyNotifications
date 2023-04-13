using System.Collections;
using Utility.WPF.Demo.Data.Factory;
using Utility.WPF.Demo.Data.Model;

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