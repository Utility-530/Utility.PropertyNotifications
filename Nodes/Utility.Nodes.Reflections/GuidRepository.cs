using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Guid = System.Guid;

namespace Utility.Nodes.Reflections
{
    public class GuidRepository
    {
        public record Table
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            public Guid Guid { get; set; }

            public Guid? Parent { get; set; }

            public string Name { get; set; }
        }

        private readonly SQLiteAsyncConnection connection;
        private readonly Task initialisationTask;

        public GuidRepository(string? dbDirectory = default)
        {
            if (dbDirectory != default)
                Directory.CreateDirectory(dbDirectory);
            connection = new SQLiteAsyncConnection(Path.Combine(dbDirectory ?? string.Empty, "data" + "." + "sqlite"));
            initialisationTask = Task.WhenAll(
                new[]{
                    connection.CreateTableAsync<Table>(),
                });
        }

        public async Task<IReadOnlyCollection<Guid>> Find(Guid parentGuid)
        {
            await initialisationTask;
            var tables = await connection.QueryAsync<Table>($"Select * from 'Table' where Parent = '{parentGuid}'");
            List<Guid> childKeys = new();
            foreach (var table in tables)
            {
                childKeys.Add(table.Guid);
            }
            return childKeys;
        }

        public async Task<Guid> Find(Guid parentGuid, string? localName)
        {
            await initialisationTask;
            var tables = await connection.QueryAsync<Table>($"Select * from 'Table' where Parent = '{parentGuid}' AND Name = '{localName}'");
            if (tables.Count == 0)
            {
                var guid = Guid.NewGuid();
                await connection.RunInTransactionAsync(c =>
                {
                    var tables = c.Query<Table>($"Select * from 'Table' where Parent = '{parentGuid}' AND Name = '{localName}'");
                    if (tables.Count != 0)
                        return;

                    var i = c.Insert(new Table { Guid = guid, Name = localName, Parent = parentGuid });
                });
                return guid;
            }
            if (tables.Count == 1)
            {
                var table = tables.Single();
                return table.Guid;
            }
            else
            {
                throw new Exception("3e909re 4323");
            }
        }

        public static GuidRepository Instance { get; } = new();
    }
}
