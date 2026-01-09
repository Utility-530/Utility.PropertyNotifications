using System;
using System.Collections.Generic;

namespace NetPrints.Compilation
{

    /// <summary>
    /// Interface for code compilers.
    /// </summary>
    public interface ICodeCompiler
    {
        /// <summary>
        /// Compiles code into a binary.
        /// </summary>
        /// <param name="outputPath">Output path for the compilation.</param>
        /// <param name="assemblyPaths">Paths to assemblies to reference.</param>
        /// <param name="sources">Source code to compile.</param>
        /// <param name="generateExecutable">Whether to generate an executable or a dynamically linked library.</param>
        /// <returns>Results for the compilation.</returns>
        ICodeCompileResults CompileSources(string outputPath, IEnumerable<string> assemblyPaths,
            IEnumerable<string> sources, bool generateExecutable);
    }

    public interface ICodeCompileResults
    {
        string PathToAssembly { get; }
        IEnumerable<string> Errors { get; }
        bool Success { get; }
    }
}
