using Utility.Models;

namespace Utility.Infrastructure
{
    public class Outputs
    {
        private Connection[] keys;

        public Outputs(Connection key, Connection[] keys)
        {
            Connection = key;
            this.keys = keys;
        }

        public Connection Connection { get; }

        public Connection[] Connections => keys;

        //public bool Match(Key key)
        //{
        //    return Connection.Type.IsAssignableFrom(key.Type);
        //}
    }

    public abstract class Connection
    {
        public abstract string Name { get; }

        public abstract bool Match(Key key);
    }

    public class Connection<T> : Connection
    {
        public override string Name => typeof(T).Name;

        public override bool Match(Key key)
        {
            return typeof(T).IsAssignableFrom(key.Type);
        }
    }

    public class DynamicConnection : Connection
    {
        public override string Name => "Dynamic";

        public override bool Match(Key key)
        {
            return typeof(BaseObject).IsAssignableFrom(key.Type) == false;
        }
    }

    //public class Connection
    //{
    //    private bool isPriority = true;
    //    private bool skipContext = true;
    //    public bool IsPriority { get => isPriority; set => Set(ref isPriority, value); }

    //    public bool SkipContext { get => skipContext; set => Set(ref skipContext, value); }
    //}

    //public class Connection<TObserver> : BaseViewModel, IConnection
    //    where TObserver : IObserver, IKey<Key>
    //{
    //    private IContainer container;
    //    private bool isPriority = true;
    //    private bool skipContext = true;

    //    public Connection(IContainer container)
    //    {
    //        this.container = container;
    //    }

    //    public bool IsPriority { get => isPriority; set => Set(ref isPriority, value); }

    //    public bool SkipContext { get => skipContext; set => Set(ref skipContext, value); }

    //    public IEnumerable<TObserver> Observers => container.ResolveMany<TObserver>();

    //    IEnumerable<Key> IConnection.Observers => container.ResolveMany<Key>();

    //    public override string ToString()
    //    {
    //        return typeof(TObserver).Name;
    //    }
    //}

    //public interface IConnection
    //{
    //    public bool IsPriority { get; set; }

    //    public bool SkipContext { get; set; }

    //    public IEnumerable<Key> Outputs { get; }
    //}
}