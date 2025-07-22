using System;
using System.Collections.Generic;
using System.Windows.Input;
using Utility.Commands;
using Utility.PropertyNotifications;

namespace Utility.Nodify.ViewModels
{
    public class TabViewModel : NotifyPropertyClass
    {
        public event Action<TabViewModel, object>? OnOpenInnerCalculator;
        public event Action<TabViewModel>? Close;

        public TabViewModel? Parent { get; set; }

        public TabViewModel()
        {
     
            OpenCalculatorCommand = new Command<object>(calculator =>
            {
                OnOpenInnerCalculator?.Invoke(this, calculator);
            });   

            CloseCommand = new Command<object>(a =>
            {
                Close?.Invoke(this);
            });
        }

        public ICommand OpenCalculatorCommand { get; }
        public ICommand CloseCommand { get; }

        public Guid Id { get; } = Guid.NewGuid();

        private object _calculator = default!;
        public object Content 
        {
            get => _calculator;
            set => RaisePropertyChanged(ref _calculator, value);
        }

        private string? _name;
        public string? Name
        {
            get => _name;
            set => RaisePropertyChanged(ref _name, value);
        }
    }
}
