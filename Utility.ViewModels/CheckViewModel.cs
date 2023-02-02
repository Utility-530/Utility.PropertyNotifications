using ReactiveUI;

namespace Utility.ViewModels
{
    //public class CheckViewModel : ViewModel
    //{
    //    public CheckViewModel(string key, bool value) : base(key)
    //    {
    //        Model = new ReactiveProperty<Filter>(value);
    //    }

    //    public override ReactiveProperty<bool> Model { get; }
    //}

    public class CheckViewModel : ReactiveObject
    {
        private bool isChecked;

        public CheckViewModel(string header, bool isChecked)
        {
            Header = header;
            IsChecked = isChecked;
        }

        public CheckViewModel()
        {
        }

        public bool IsChecked { get => isChecked; set => this.RaiseAndSetIfChanged(ref isChecked, value); }

        public bool IsSelected { get; set; }

        public string Header { get; init; }

        public virtual object Content => Header;

        public override string ToString()
        {
            return $"{Header} {IsChecked} {IsSelected}";
        }
    }

    public class CheckContentViewModel : CheckViewModel
    {
        public CheckContentViewModel(object content, string header, bool isChecked) : base(header, isChecked)
        {
            Content = content;
        }

        public CheckContentViewModel()
        {
        }

        public override object Content { get; }
    }
}