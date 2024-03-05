using Nodify;
using Utility.Nodify.Core;
using System;
using Utility.ViewModels.Base;

namespace Utility.Nodify.Operations
{
    public class OperationConnectionViewModel : ConnectionViewModel
    {

        public static IObserver<BaseViewModel> Observer;


        protected override void Output_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectorViewModel.Value))
            {
                Observer.OnNext(this);
            }
        }
    }
}
