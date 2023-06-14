using System;
using System.Windows.Input;

namespace Utility.FileSystem.Transfer.WPF.Controls.Abstract
{
    public interface IProgressView
    {
        IObservable<TimeSpan> CompleteEvents { get; }

        ICommand RunCommand { get; }
    }
}