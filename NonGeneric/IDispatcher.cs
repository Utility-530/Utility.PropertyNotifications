using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UtilityInterface.NonGeneric
{
    public interface IDispatcher
    {
        void Invoke(Action action);

        Task InvokeAsync(Action action);

        bool CheckAccess();
    }
}
