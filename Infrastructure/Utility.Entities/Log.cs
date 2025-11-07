using System.Runtime.CompilerServices;

namespace Utility.Entities
{
    public class Log
    {
        public Log(
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = -1)
        {
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
        }

        public string Key { get; init; }

        public string Source { get; init; }

        public DateTime Date { get; init; }

        public Utility.Enums.Diagnostic Level { get; init; } = Utility.Enums.Diagnostic.Information;

        public string Message { get; init; }

        public string CallerFilePath { get; }

        public int CallerLineNumber { get; }

        public override string ToString()
        {
            return $"{Key}  {Source}  {Date}  {Level}  {Message}";
        }
    }
}