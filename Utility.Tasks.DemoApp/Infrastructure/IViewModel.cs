using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Infrastructure;

namespace Utility.Tasks.DemoApp.ViewModel
{
    public interface IViewModel
    {
    }

    public interface IDialogCommandViewModel : IObservable<CloseRequest>
    {
    }
}
