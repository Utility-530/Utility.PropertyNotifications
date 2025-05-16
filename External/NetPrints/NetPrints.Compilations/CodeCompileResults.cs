namespace NetPrints.Compilation
{
    /// <summary>
    /// Contains results of a compilation.
    /// </summary>
    [Serializable]
    public class CodeCompileResults : ICodeCompileResults
    {
        /// <summary>
        /// Whether the compilation was successful.
        /// </summary>
        public bool Success
        {
            get;
        }

        /// <summary>
        /// Errors of the compilation.
        /// </summary>
        public IEnumerable<string> Errors
        {
            get;
        }

        /// <summary>
        /// Path to the generated assembly.
        /// </summary>
        public string PathToAssembly
        {
            get;
        }

        public CodeCompileResults(bool success, IEnumerable<string> errors, string pathToAssembly)
        {
            Success = success;
            Errors = errors;
            PathToAssembly = pathToAssembly;
        }
    }
}
