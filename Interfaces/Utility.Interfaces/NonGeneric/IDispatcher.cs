using System;
using System.Threading.Tasks;

namespace Utility.Interfaces.NonGeneric
{
    public interface IDispatcher
    {
        void Invoke(Action action);

        Task InvokeAsync(Action action);

        bool CheckAccess();
    }
}