using Microsoft.Xaml.Behaviors;
using Nodify;
using Utility.Models.Diagrams;
using Utility.Nodes;

namespace Utility.Nodify.Views.Infrastructure
{
    internal class TransferBehavior : Behavior<BaseConnection>
    {
        protected override void OnAttached()
        {
            if (this.AssociatedObject.DataContext is ConnectionViewModel { Data: MethodConnection methodConnection } connectionViewModel)
            {
                methodConnection.Transfer += MethodConnection_Transfer;
                methodConnection.TransferComplete += MethodConnection_TransferComplete;
            }
            base.OnAttached();
        }

        private void MethodConnection_TransferComplete()
        {
            AssociatedObject.StopAnimation();

        }

        private void MethodConnection_Transfer()
        {
            AssociatedObject.StartAnimation();
        }
    }
}
