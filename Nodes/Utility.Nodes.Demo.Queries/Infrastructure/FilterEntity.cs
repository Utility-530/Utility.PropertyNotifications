using Utility.ViewModels;

namespace Utility.Nodes.Demo.Queries
{
    public class FilterEntity : NotifyPropertyChangedBase
    {
        private string groupKey;
        private string key;
        private string body;

        public FilterEntity()
        {

        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Key
        {
            get => key; set
            {
                if (value == key) return;
                key = value;
                RaisePropertyChanged();
            }
        }

        public string GroupKey
        {
            get => groupKey; set
            {
                if(value == groupKey) return;
                groupKey = value;
                RaisePropertyChanged();
            }
        }

        public string Body
        {
            get => body; set
            {
                if (value == body) return;
                body = value;
                RaisePropertyChanged();
            }
        }
    }

}
