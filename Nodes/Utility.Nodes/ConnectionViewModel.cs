using System.Collections.ObjectModel;
using Utility.Helpers.Generic;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Nodify.Base;

namespace Utility.Nodes
{
    public class ConnectionViewModel : NodeViewModel, IConnectionViewModel
    {
        private IConnectorViewModel _output = default!, _input = default!;

        public ConnectionViewModel()
        {
            Inputs = new ObservableCollection<IConnectorViewModel>();
            Outputs = new ObservableCollection<IConnectorViewModel>();
        }

        public IConnectorViewModel Input
        {
            get => Inputs.SingleOrDefault();
            set => Inputs.Add(value);
        }

        public IConnectorViewModel Output
        {
            get => Outputs.SingleOrDefault();
            set => Outputs.Add(value);
        }


        public bool IsDirectionForward { get; set; }
    }
}