using ReactiveUI;
using Utility.PropertyNotifications;

namespace Utility.WPF.Demo.Buttons
{
    public class ToggleViewModel : NotifyPropertyClass
    {
        private bool isChecked = true;

        public bool IsChecked
        {
            get => isChecked;
            set => this.RaisePropertyChanged(ref isChecked, value);
        }
    }
}