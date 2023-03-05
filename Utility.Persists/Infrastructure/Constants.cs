namespace Utility.Persists.Infrastructure
{
    public static class Constants
    {
        public static class FileExtension
        {
            public const string LiteDB = "litedb";
            public const string Sqlite = "sqlite";
            public const string Default = "db";
        }

        public static class Default
        {
            public const string Directory = "../../../Data";
            public const string Name = "Data";
            public const string DateFormat = "yyyy-MM-dd";
        }

    }
}
