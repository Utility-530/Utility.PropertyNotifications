using NetPrints.Core;
using System.Collections.Generic;

namespace NetPrints.Interfaces
{
    public interface IProject
    {
        IObservableCollection<ICompilationReference> References { get; }

        IEnumerable<string> GenerateClassSources();
    }
}
