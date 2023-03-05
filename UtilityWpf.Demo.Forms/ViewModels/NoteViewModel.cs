using Utility.Models;
using Utility.ViewModels;

namespace UtilityWpf.Demo.Forms.ViewModels
{
    public class NoteViewModel : ViewModel
    {
        public NoteViewModel(string text) : base(text)
        {
            //Model = new StringProperty(text);
        }

        public override Property Model { get; }

        //public override Model Model { get; }
    }
}