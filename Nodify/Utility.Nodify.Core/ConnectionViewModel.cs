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

        public ConnectionViewModel()
        {
        }

        public Guid Id { get; } = Guid.NewGuid();


        public string Key {get=> new { Id, string.Empty }.ToString(); set=> throw new NotSupportedException("Key is read-only and generated from Id."); }


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
        public object Data { get; set; }

    }
}
