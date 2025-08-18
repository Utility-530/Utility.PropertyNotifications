using Microsoft.Xaml.Behaviors;
using Nodify;

namespace Utility.Nodify.Views.Infrastructure
{
    using Nodify;
    using System.Windows;
    using System.Windows.Threading;

    internal class ViewPortBehavior : Behavior<NodifyEditor>
    {
        protected override void OnAttached()
        {
            if (this.AssociatedObject.Arrangement == Arrangement.Standard)
            {

            }
            if (this.AssociatedObject.Arrangement == Arrangement.Tree)
            {


            }
            else if (this.AssociatedObject.Arrangement == Arrangement.UniformRow)
            {
                this.AssociatedObject.DisplayConnectionsOnTop = true;

            }
            base.OnAttached();
        }
    }
}
