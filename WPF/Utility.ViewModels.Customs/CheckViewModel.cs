using ReactiveUI;
using Utility.Models.Filters;

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

    public class CheckViewModel : ViewModel<Filter>
    {
        private bool? isChecked;

        public CheckViewModel(string header, bool isChecked) : base(header, default)
        {
            Header = header;
            IsChecked = isChecked;
        }

        public override bool? IsChecked { get => isChecked; set => this.RaiseAndSetIfChanged(ref isChecked, value); }

        public override bool IsSelected { get; set; }

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

        public CheckContentViewModel() : base(default, default)
        {
        }

        public override object Content { get; }
    }
}