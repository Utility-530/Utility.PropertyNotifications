using System.Collections;
using UtilityWpf.Demo.Data.Factory;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Master.ViewModels
{
    public class ItemsWrapViewModel
    {
        public ItemsWrapViewModel()
        {
        }

        public IEnumerable Collection { get; } = new FieldsFactory().BuildCollection();
    }
}