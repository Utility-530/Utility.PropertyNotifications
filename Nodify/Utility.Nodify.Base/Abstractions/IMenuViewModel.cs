using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Nodify.Operations.Infrastructure;

namespace Utility.Nodify.Base.Abstractions
{
    public interface IMenuViewModel : IObservable<(PointF, IMenuItemViewModel)>
    {

        IList<IMenuItemViewModel> Items { get; }

        void OpenAt(PointF mouseLocation);
        void Close();

        event Action Closed;
    }
    public interface IMenuItemViewModel
    {     
        event Action<IMenuItemViewModel> Selected;
        object Content { get; }
  
    }
    public interface IMenuFactory
    {
        IMenuViewModel? CreateMenu();
        IMenuItemViewModel? Create(MenuItem menuItem);
    }
}
