using ReactiveUI;
using _ViewModel = Utility.ViewModels.ViewModel;

namespace UtilityWpf.Demo.Forms.ViewModels
{
    public class TitleViewModel : _ViewModel
    {
        private string title = "The Title";
        private string subtitle = "The SubTitle";

        public TitleViewModel() : base("Title")
        {
        }

        public string Title { get => title; set => this.RaiseAndSetIfChanged(ref title, value); }
        public string SubTitle { get => subtitle; set => this.RaiseAndSetIfChanged(ref subtitle, value); }
        public override Utility.Common.Models.Model Model { get; }
        //public override Model Model { get; }
    }
}