using Utility.Helpers.Generic;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Nodify.Base;

namespace Utility.Nodes
{
    public class ConnectionViewModel : ViewModelTree, IConnectionViewModel
    {
        private IConnectorViewModel _output = default!, _input = default!;

        public ConnectionViewModel()
        {
        }

        public IConnectorViewModel Input
        {
            get => _input;
            set => RaisePropertyChanged(ref _input, value)
                .Then(() =>
                {
                });
        }

        public NodeState State { get; set; } = NodeState.None;

        public IConnectorViewModel Output
        {
            get => _output;
            set => RaisePropertyChanged(ref _output, value)
                .Then(() =>
                {
                });
        }


        public bool IsDirectionForward { get; set; }
    }
}