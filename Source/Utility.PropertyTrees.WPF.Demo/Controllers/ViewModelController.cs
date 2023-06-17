using Utility.Infrastructure;
using Utility.PropertyTrees.WPF.Meta;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class ViewModelController : BaseObject
    {
        public override Key Key => new Key(Guids.ViewModelController, nameof(ViewModelController), typeof(ViewModelController));

        public void OnNext(TreeClickEvent command)
        {

        }
    }
}
