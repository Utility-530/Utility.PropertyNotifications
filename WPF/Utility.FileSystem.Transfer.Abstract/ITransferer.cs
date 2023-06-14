
using System;
using Utility.FileSystem.Transfer.Common;

namespace Utility.FileSystem.Transfer.Abstract
{
    public interface ITransferer
    {
        IObservable<Progress> Transfer(params string[] args);
    }
}