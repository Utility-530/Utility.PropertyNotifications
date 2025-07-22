using System;
using Utility.Helpers.Generic;
using Utility.Nodify.Core;
using Utility.PropertyNotifications;

namespace Utility.Nodify.Models
{
    public class ConnectionViewModel : NotifyPropertyClass, IConnectionViewModel
    {
        private IConnectorViewModel _output = default!, _input = default!;
        private string _title = string.Empty;


        public Guid Id { get; } = Guid.NewGuid();


        public Key Key => new(Id, string.Empty);


        public string Title
        {
            get => _title;
            set => RaisePropertyChanged(ref _title, value);
        }

        public IConnectorViewModel Input
        {
            get => _input;
            set => RaisePropertyChanged(ref _input, value)
                .Then(() =>
                {
                    Input.PropertyChanged += Input_PropertyChanged;
                });
        }
        public NodeState State { get; set; } = NodeState.None;

        public IConnectorViewModel Output
        {
            get => _output;
            set => RaisePropertyChanged(ref _output, value)
                .Then(() =>
                {
                    
                    Output.PropertyChanged += Output_PropertyChanged;
                });
        }

        protected virtual void Output_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectorViewModel.Value))
                State = NodeState.OutputValueChanged;
        }
        protected virtual void Input_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectorViewModel.Value))
                State = NodeState.InputValueChanged;

        }
    }
}
