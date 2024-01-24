using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace Repository
{
    public class MiniStore 
    {
        public class Options
        {
            internal readonly List<string> PreCommands = new List<string>();

            internal string InternalConnectionString { get; set; } = "";


            public Options ConnectionString(string connectionString)
            {
                InternalConnectionString = connectionString;
                return this;
            }

            public Options FromPath(string path)
            {
                return ConnectionString("Data Source=" + path);
            }

            public Options PreCommand(string command)
            {
                PreCommands.Add(command);
                return this;
            }

            public Options JournalModeWal()
            {
                return PreCommand("PRAGMA journal_mode = WAL");
            }
        }

        private readonly Options _options;

        public static void DeleteStore(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public MiniStore(Options options)
        {
            _options = options;
            EnsureDatabaseExists();
            DoPreCommands();
        }

        public MiniStore(string path)
            : this(new Options().FromPath(path))
        {
        }

        private void DoPreCommands()
        {
            using SqliteConnection sqliteConnection = Connection();
            sqliteConnection.Open();
            using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            using List<string>.Enumerator enumerator = _options.PreCommands.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string text = (sqliteCommand.CommandText = enumerator.Current);
                sqliteCommand.ExecuteNonQuery();
            }
        }

        private SqliteConnection Connection()
        {
            return new SqliteConnection(_options.InternalConnectionString);
        }

        private void EnsureDatabaseExists()
        {
            using SqliteConnection sqliteConnection = Connection();
            sqliteConnection.Open();
            using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "CREATE TABLE IF NOT EXISTS Store (Key TEXT PRIMARY KEY, ParentKey TEXT NOT NULL, Data TEXT)";
            sqliteCommand.ExecuteNonQuery();
        }

        public void BatchPut(IEnumerable<(string key, string parentKey, string value)> items)
        {
            using SqliteConnection sqliteConnection = Connection();
            sqliteConnection.Open();
            using SqliteTransaction sqliteTransaction = sqliteConnection.BeginTransaction();
            using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "INSERT OR REPLACE INTO Store (Key, ParentKey, Data) VALUES (@key, @parentkey, @data)";
            sqliteCommand.Parameters.Add("@key", SqliteType.Text);
            sqliteCommand.Parameters.Add("@data", SqliteType.Text);
            foreach (var (value, value3, value2) in items)
            {
                sqliteCommand.Parameters["@key"].Value = value;
                sqliteCommand.Parameters["@parentkey"].Value = value3;
                sqliteCommand.Parameters["@data"].Value = value2;
                sqliteCommand.ExecuteNonQuery();
            }

            sqliteTransaction.Commit();
        }

        public void Put(string key, string parentKey, string data)
        {
            using SqliteConnection sqliteConnection = Connection();
            sqliteConnection.Open();
            using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "INSERT INTO Store (Key, ParentKey, Data) VALUES (@key, @parentkey, @data) ON CONFLICT(Key) DO UPDATE SET Data = @data";
            sqliteCommand.Parameters.AddWithValue("@key", key);
            sqliteCommand.Parameters.AddWithValue("@parentkey", parentKey);
            sqliteCommand.Parameters.AddWithValue("@data", data);
            sqliteCommand.ExecuteNonQuery();
        }

        public void Put<T>(string key, string parentKey, T data)
        {
            Put(key, parentKey, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
        }

        public T? Get<T>(string key)
        {
            if (Get(key) is string { Length: > 0 } str)
                return JsonSerializer.Deserialize<T>(str);
            return default;
        }

        public string Get(string key)
        {
            using SqliteConnection sqliteConnection = Connection();
            sqliteConnection.Open();
            using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "SELECT Data FROM Store WHERE Key = @key";
            sqliteCommand.Parameters.AddWithValue("@key", key);
            using SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
            return sqliteDataReader.Read() ? sqliteDataReader.GetString(0) : "";
        }


        public IEnumerable<string> GetByParent(string key)
        {
            using SqliteConnection sqliteConnection = Connection();
            sqliteConnection.Open();
            using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "SELECT Data FROM Store WHERE ParentKey = @key";
            sqliteCommand.Parameters.AddWithValue("@key", key);
            using SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
            while (sqliteDataReader.Read())
                yield return sqliteDataReader.GetString(0);
        }

        public IEnumerable<T> GetByParent<T>(string key)
        {
            return GetByParent(key).Select(a => { var x = JsonSerializer.Deserialize<T>(a); return x; });
        }


        public void Delete(string key)
        {
            using SqliteConnection sqliteConnection = Connection();
            sqliteConnection.Open();
            using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "DELETE FROM Store WHERE Key = @key";
            sqliteCommand.Parameters.AddWithValue("@key", key);
            sqliteCommand.ExecuteNonQuery();
        }
        public void DeleteByParent(string key)
        {
            using SqliteConnection sqliteConnection = Connection();
            sqliteConnection.Open();
            using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "DELETE FROM Store WHERE ParentKey = @key";
            sqliteCommand.Parameters.AddWithValue("@key", key);
            sqliteCommand.ExecuteNonQuery();
        }
        public bool Exists(string key)
        {
            using SqliteConnection sqliteConnection = Connection();
            sqliteConnection.Open();
            using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "SELECT COUNT(*) FROM Store WHERE Key = @key";
            sqliteCommand.Parameters.AddWithValue("@key", key);
            using SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
            bool result = sqliteDataReader.Read() && sqliteDataReader.GetString(0) != "0";
            sqliteConnection.Close();
            return result;
        }

        public List<string> Keys(string key)
        {
            using SqliteConnection sqliteConnection = Connection();
            sqliteConnection.Open();
            using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "SELECT Key FROM Store";
            using SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
            List<string> list = new List<string>();
            while (sqliteDataReader.Read())
            {
                list.Add(sqliteDataReader.GetString(0));
            }

            return list;
        }

        public List<string> KeysLike(string expression)
        {
            using SqliteConnection sqliteConnection = Connection();
            sqliteConnection.Open();
            using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "SELECT Key FROM Store WHERE Key LIKE @expression";
            sqliteCommand.Parameters.AddWithValue("@expression", expression);
            using SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
            List<string> list = new List<string>();
            while (sqliteDataReader.Read())
            {
                list.Add(sqliteDataReader.GetString(0));
            }

            return list;
        }
    }
}
