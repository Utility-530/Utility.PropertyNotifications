using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Utility.Nodify.Base;
using System.Collections.Generic;

namespace Utility.Nodify.Core
{

    public class NodeViewModel : BaseNodeViewModel, INodeViewModel
    {
        private ICollection<IConnectorViewModel> input = new RangeObservableCollection<IConnectorViewModel>();
        private ICollection<IConnectorViewModel> output = new RangeObservableCollection<IConnectorViewModel>();

        public event Action<NodeViewModel> InputChanged;

        public NodeViewModel()
        {
            NewMethod();
        }

        public Guid Id { get; } = Guid.NewGuid();
        public Key Key => new(Id, Title);

        public virtual ICore Core { get; set; }

        private void NewMethod()
        {
            if (input is RangeObservableCollection<IConnectorViewModel> range)
                _ = range.WhenAdded(x =>
                {
                    Add(x);
                })
                .WhenRemoved(x =>
                {
                    x.PropertyChanged -= OnInputValueChanged;
                });
        }

        void Add(IConnectorViewModel x)
        {
            x.Node = this;
            x.IsInput = true;
            x.PropertyChanged += OnInputValueChanged;
            if (x.Value != default)
                OnInputValueChanged(x);
        }

        private void OnInputValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectorViewModel.Value))
            {
                OnInputValueChanged(sender as ConnectorViewModel);
            }
        }

        public NodeState State { get; set; } = NodeState.None;


        public ICollection<IConnectorViewModel> Input
        {
            get =>
                input;
            set
            {
                input = value;
                foreach (var inp in value)
                {
                    Add(inp);
                }
                NewMethod();
            }
        }
        public ICollection<IConnectorViewModel> Output
        {
            get => output;
            set
            {
                output = value;
            }
        }
        public virtual void OnInputValueChanged(IConnectorViewModel connectorViewModel)
        {
            State = NodeState.InputValueChanged;
        }
    }
}
