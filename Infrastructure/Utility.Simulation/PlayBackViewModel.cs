using Utility.PropertyNotifications;

namespace Utility.Simulation
{
    public class PlayBackViewModel : NotifyPropertyClass
    {
        private Enum last, enabled;

        public PlayBackViewModel()
        {
        }

        public int GridRow => 0;

        public Enum Last
        {
            get => last;
            set
            {
                last = value;
                RaisePropertyChanged();
            }
        }

        public Enum Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                RaisePropertyChanged();
            }
        }




    }
}
