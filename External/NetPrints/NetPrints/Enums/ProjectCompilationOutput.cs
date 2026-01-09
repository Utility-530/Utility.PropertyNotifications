using System;

namespace NetPrints.Enums
{

    [Flags]
    public enum ProjectCompilationOutput
    {
        Nothing = 0,
        SourceCode = 1,
        Binaries = 2,
        Errors = 4,
        All = SourceCode | Binaries | Errors,
    }
}
