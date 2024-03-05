using Utility.Nodify.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Utility.Nodify.Base;
using YetAnotherStringMatcher;

namespace Utility.Nodify.Core
{
    public interface ICore
    {

    }

    public class NodeViewModel : BaseNodeViewModel
    {
        private RangeObservableCollection<ConnectorViewModel> input = new();
        private RangeObservableCollection<ConnectorViewModel> output = new();

        public event Action<NodeViewModel> InputChanged;

        //private RangeObservableCollection<ConnectorViewModel> input = new(), output = new();


        public NodeViewModel()
        {
            NewMethod();
        }


        public virtual ICore Core { get; set; }

        private void NewMethod()
        {
            _ = Input.WhenAdded(x =>
            {
                sd(x);
            })
            .WhenRemoved(x =>
            {
                x.PropertyChanged -= OnInputValueChanged;
            });
        }

        void sd(ConnectorViewModel x)
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

        public Guid Id { get; } = Guid.NewGuid();
        public Key Key => new(Id, Title);



        public RangeObservableCollection<ConnectorViewModel> Input
        {
            get =>
                input;
            set
            {
                input = value;
                foreach (var inp in value)
                {
                    sd(inp);
                }
                NewMethod();
            }
        }
        public RangeObservableCollection<ConnectorViewModel> Output
        {
            get => output;
            set
            {
                output = value;
            }
        }
        public virtual void OnInputValueChanged(ConnectorViewModel connectorViewModel)
        {
        }


    }
}
