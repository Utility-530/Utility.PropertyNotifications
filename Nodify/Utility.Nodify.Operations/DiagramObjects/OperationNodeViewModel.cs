using Utility.Nodify.Core;
using System;
using System.Windows.Input;
using Utility.ViewModels.Base;

namespace Utility.Nodify.Operations
{
    public class OperationNodeViewModel : NodeViewModel
    {
        public static IObserver<BaseViewModel> Observer;

        public OperationNodeViewModel()
        {

        }


        public override void OnInputValueChanged(ConnectorViewModel connectorViewModel)
        {

            Observer.OnNext(this);
        }
    }
}
