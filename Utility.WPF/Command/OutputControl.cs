using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Command
{
    public class OutputControl : ContentControl, ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public string? PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(OutputControl), new PropertyMetadata(null));

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Dispatcher.Invoke(() =>
            Content = PropertyName == null ?
            parameter :
            parameter?.GetType().GetProperty(PropertyName)?.GetValue(parameter)
            );
        }
    }

    //public class OutputCommand2 : Control, ICommand
    //{
    //    public IEnumerable Output
    //    {
    //        get { return (IEnumerable)GetValue(OutputProperty); }
    //        set { SetValue(OutputProperty, value); }
    //    }

    //    public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(IEnumerable), typeof(OutputCommand2), new PropertyMetadata(null));

    //    public event EventHandler CanExecuteChanged;

    //    public bool CanExecute(object parameter)
    //    {
    //        return true;
    //    }

    //    public void Execute(object parameter)
    //    {
    //        this.Dispatcher.InvokeAsync(() => Output = parameter as IEnumerable, System.Windows.Threading.DispatcherPriority.Background);
    //    }
    //}
}