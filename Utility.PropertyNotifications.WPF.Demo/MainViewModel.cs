
namespace Utility.PropertyNotifications.WPF.Demo
{
    public class MainViewModel : NotifyPropertyClass
    {

        private string _someProperty;
        private string _otherProperty;

        public string SomeProperty
        {
            get
            {
                RaisePropertyCalled(_someProperty);
                return _someProperty;
            }
            set
            {
                RaisePropertyReceived(ref _someProperty, value);
            }
        }

        public string OtherProperty
        {
            get => _otherProperty;
            set
            {
                RaisePropertyChanged(ref _otherProperty, value);
            }
        }
    }
}
