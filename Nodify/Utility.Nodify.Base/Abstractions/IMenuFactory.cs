using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Nodify.Base.Abstractions
{
    public interface IMenuFactory : IObservable<(PointF, object)>
    {
        INodeViewModel? CreateMenu();
    }
}
