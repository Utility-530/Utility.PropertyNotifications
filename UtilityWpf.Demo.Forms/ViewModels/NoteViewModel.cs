using Utility.ViewModels;

namespace UtilityWpf.Demo.Forms.ViewModels
{
    public class NoteViewModel : ViewModel
    {
        public NoteViewModel(string text) : base(text)
        {
            //Model = new StringProperty(text);
        }

        public override Utility.Common.Models.Model Model { get; }

        //public override Model Model { get; }
    }
}