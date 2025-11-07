using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using ReactiveUI;
using Utility.WPF.Demo.Common.ViewModels;

namespace Utility.WPF.Demo.Buttons
{
    public class Number
    {
        public Number(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }

    public class ButtonsViewModel
    {
        public ButtonsViewModel()
        {
            Data = new ObservableCollection<ButtonViewModel>
            {
                new("1", new ActionCommand(()=>{ Numbers.Add(new Number(1)); })),
                new("2", ReactiveCommand.Create(()=>{ Numbers.Add(new Number(2)); })),
                new("3", ReactiveCommand.Create(()=>{ Numbers.Add(new Number(3)); })),
            };

            Command = new ActionCommand(() => { MessageBox.Show("Event To Command"); });
        }

        public ObservableCollection<ButtonViewModel> Data { get; }

        public ObservableCollection<Number> Numbers { get; } = new();

        public ICommand Command { get; }
    }
}