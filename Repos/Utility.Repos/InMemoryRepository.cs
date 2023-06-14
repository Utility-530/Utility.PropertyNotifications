using Utility.Helpers;
using Utility.Infrastructure.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
namespace Utility.Repos
{

    public class InMemoryRepository : IRepository
    {
        public class Table
        {
            public int Id { get; set; }

            public Guid Guid { get; set; }

            public Guid? Parent { get; set; }

            public string Name { get; set; }

            public Type Type { get; set; }
        }

        public class Property
        {
            public Guid Guid { get; set; }
            public DateTime Added { get; set; }
            public DateTime? Removed { get; set; }
            public object Value { get; set; }
        }


        List<Table> Tables = new();
        Dictionary<int, List<Property>> Properties = new();


        public IEquatable Key => new Key<InMemoryRepository>(Guids.InMemory);


        public Task UpdateValue(IEquatable key, object value)
        {
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            var tables = Tables.Where(v => v.Guid.Equals(guid)).ToList();

            if (Tables.Count == 1)
            {
                if (Properties.ContainsKey(tables.Single().Id) == false)
                {
                    Properties[tables.Single().Id] = new();
                }
            }
            else if (tables.Count == 0)
            {
                throw new Exception("22v ere 4323");
            }
            else
            {
                throw new Exception("676 ere 4323");
            }
            return Task.CompletedTask;
        }


        public Task<IEquatable[]> FindKeys(IEquatable key)
        {
            if (key is not Key { Guid: var parent } _key)
            {
                throw new Exception("reg 43cs ");
            }

            if (key is not Key { Name: string name, Type: Type type })
            {
                var tables = Tables.Where(t => t.Parent == parent);
                List<IEquatable> childKeys = new();
                foreach (var table in tables)
                {
                    var childKey = new Key(table.Guid, table.Name, table.Type);
                    childKeys.Add(childKey);
                }
                return Task.FromResult(childKeys.ToArray());
            }
            else
            {
                var tables = Tables.Where(t => t.Parent == parent && t.Name == name).ToArray();
                if (tables.Length == 0)
                {
                    var guid = Guid.NewGuid();
                    Tables.Add(new Table { Guid = guid, Name = name, Parent = parent, Type = type });
                    return Task.FromResult(new IEquatable[] { new Key(guid, name, type) });
                }
                if (tables.Length == 1)
                {
                    var table = tables.Single();
                    return Task.FromResult(new IEquatable[] { new Key(table.Guid, name, type) });
                }
                else
                {
                    throw new Exception("3e909re 4323");
                }
            }
        }

        public Task<object?> FindValue(IEquatable key)
        {
            if (key is not Key { Guid: var guid } _key)
            {
                throw new Exception("reg 43cs ");
            }

            var tables = Tables.Where(v => v.Guid.Equals(guid)).ToArray();

            if (tables.Length == 0)
            {
                throw new Exception("!43 ere 4323");
            }
            else if (tables.Length == 1)
            {
                var tableId = tables.Single().Id;

                var properties = Properties[tableId];
                {
                    List<object> list = new();
                    foreach (var property in properties)
                    {
                            list.Add(property.Value);
                    }
                    return Task.FromResult(list.LastOrDefault() ?? null);
                }
            }
            else
            {
                throw new Exception("ere 4323");
            }
        }
    }
}