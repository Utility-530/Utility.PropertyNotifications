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
            //Command = new Command(() =>
            //{
            //    Action();
            //}, () => this.Input.Count == 0 && this.Output.Count == 1);
        }


        //public ICommand Command { get; set; }

        //public Action Action => new Action(() => { Output.Single().Value = MethodInfo.Invoke(null, Input.Select(a => a.Value).ToArray()); });

        //public override IOperation Operation { get; set; }

        public override void OnInputValueChanged(ConnectorViewModel connectorViewModel)
        {

            Observer.OnNext(this);
        }
    }
}
