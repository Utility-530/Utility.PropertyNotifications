using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Utility.Nodify.Base;
using System.Collections.Generic;
using Utility.Nodify.Core;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodify.Models
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

        public string GroupKey { get; set; }

        private void NewMethod()
        {
            if (input is RangeObservableCollection<IConnectorViewModel> range)
                _ = range.WhenAdded(x =>
                {
                    Add(x);
                });
        }

        void Add(IConnectorViewModel x)
        {
            x.Node = this;
            x.IsInput = true;

        }



        public NodeState State { get; set; } = NodeState.None;

        public object Data { get; set; }

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

        public IDiagramViewModel Graph { get; set; }
        public Orientation Orientation { get; set; }
    }
}
