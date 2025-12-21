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
        }

        public IConnectorViewModel Input
        {
            get => Inputs.SingleOrDefault();
            set { Inputs.Add(value); if (Inputs.Count == 1) RaisePropertyChanged(); }
        }

        public IConnectorViewModel Output
        {
            get => Outputs.SingleOrDefault();
            set { Outputs.Add(value); if (Outputs.Count == 1) RaisePropertyChanged(); }
        }


        public bool IsDirectionForward { get; set; }
    }
}