namespace Utility.Repos
{

    public partial class SqliteRepository
    {
        public record DatabaseDirectory(string Path);
    }

    public partial class LiteDBRepository
    {
        public record DatabaseSettings(string Path, System.Type Type);
    }

    public partial class ViewModelRepository
    {
        public record DatabaseSettings(string Path, System.Type Type);
    }
}