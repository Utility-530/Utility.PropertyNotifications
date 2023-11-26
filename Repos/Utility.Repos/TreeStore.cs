using SQLite;
using Utility.Trees;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Trees.Abstractions;

namespace Utility.Repos
{
    public class Repository2 : IRepository
    {
        protected readonly SQLiteAsyncConnection connection;
        private Task initialisationTask;

        private Dictionary<Guid, IEquatable> Dictionary = new();

        public Repository2(string? dbDirectory)
        {
            connection = new SQLiteAsyncConnection(Path.Combine(dbDirectory, "data" + "." + "sqlite"));
            //Initialise();
        }
        public IEquatable Key => new Key<Repository2>(Utility.Guids.Tree);


        private Tree tree = new();

        public async Task Update(IEquatable key, object value)
        {
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }
            tree[key].Add(value);
            Dictionary[Guid.NewGuid()] = key;
        }

        public Task<IEquatable[]> FindKeys(IEquatable key)
        {
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            return Task.Run(() =>
            {
                ITree? parent = tree[_key];

                if (parent == default)
                {
                    tree.Add(_key);
                }


                if (NewMethod(_key) is not IEquatable childKey)
                {
                    childKey = new Key(guid, name, type);
                    (tree[_key] ?? throw new Exception("88df 44gfgdf")).Add(childKey);
                }

                return new[] { childKey };
            });
        }

        private IEquatable? NewMethod(Key _key)
        {
            foreach (var a in (tree[_key] ?? throw new Exception("vdf 44gfgdf")))
            {
                if (a.Data is Key { Guid: var guid, Name: var name, Type: var type })
                {
                    if (name == _key.Name && type == _key.Type)
                    {
                        return new Key(guid, name, type);
                    }
                }
            }
            return null;
        }

        public Task<object> FindValue(IEquatable key)
        {
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            return Task.Run(() => (object)(tree[_key] ?? throw new Exception("82228df 44gfgdf"))?.Items.Cast<object>().LastOrDefault());
        }
    }
}