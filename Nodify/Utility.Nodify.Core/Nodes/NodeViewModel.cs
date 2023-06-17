using Utility.Nodify.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Utility.Nodify.Core
{

    public class NodeViewModel : BaseNodeViewModel
    {
        public event Action<NodeViewModel> InputChanged;

        private RangeObservableCollection<ConnectorViewModel> input = new(), output = new();


        public NodeViewModel()
        {
            _ = Input.WhenAdded(x =>
            {
                x.Node = this;
                x.IsInput = true;
                x.PropertyChanged += OnInputValueChanged;

            })
            .WhenRemoved(x =>
            {
                x.PropertyChanged -= OnInputValueChanged;
            });
        }

        private void OnInputValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectorViewModel.Value))
            {
                OnInputValueChanged(sender as ConnectorViewModel);
            }
        }

        public Guid Id { get; } = Guid.NewGuid();
        public Key Key => new(Id, Title);



        public RangeObservableCollection<ConnectorViewModel> Input => input;

        public RangeObservableCollection<ConnectorViewModel> Output => output;

        public virtual void OnInputValueChanged(ConnectorViewModel connectorViewModel)
        {
        }
    }
}
