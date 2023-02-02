using Utility.Common.Models;
using Utility.ViewModels;

namespace Utility.WPF.Demo.Date;

public class NotesViewModel : ViewModel
{
    public NotesViewModel(string key) : base(key)
    {
    }

    public override Model Model { get; }
}