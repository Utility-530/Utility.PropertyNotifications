using Utility.Nodify.Core;
using System;
using Utility.Collections;
using Utility.Models;
using Utility.ViewModels.Base;

namespace Utility.Nodify.Core
{
    public class ConnectionViewModel : BaseViewModel
    {
        private ConnectorViewModel _output = default!, _input = default!;
        private string _title = string.Empty;


        public Guid Id { get; } = Guid.NewGuid();


        public Key Key => new(Id, string.Empty);


        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public ConnectorViewModel Input
        {
            get => _input;
            set => Set(ref _input, value)
                .Then(() =>
                {
                    this.Input.PropertyChanged += Input_PropertyChanged;
                });
        }

        public ConnectorViewModel Output
        {
            get => _output;
            set => Set(ref _output, value)
                .Then(() =>
                {
                    this.Output.PropertyChanged += Output_PropertyChanged;
                });
        }

        protected virtual void Output_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }
        protected virtual void Input_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }
    }
}
