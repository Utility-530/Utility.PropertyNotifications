namespace Utility.Repos
{

    public partial class SqliteRepository
    {
        public record DatabaseDirectory(string Path);
    }

    public partial class LiteDBRepository
    {
        public record DatabaseSettings(string Path, Type Type);
    }
}