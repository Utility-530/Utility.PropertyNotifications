using LiteDB;
using Utility.Models;
using _Key = Utility.Models.Key;

namespace Utility.Repos
{
    public partial class ViewModelRepository : IRepository, IDisposable
    {
        public record Type
        {
            public int Id { get; init; }
            public string? Assembly { get; init; }
            public string? Namespace { get; init; }
            public string Name { get; init; }
        }

        public class MetaData
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public Guid ValueId { get; set; }

            public Guid? ParentId { get; set; }

            public Type? Type { get; set; }

            //public bool Inherit { get; set; }

            //public string Mode { get; set; }

            //public Dictionary<string, string> ValuePairs { get; set; }
        }

        private readonly BsonMapper _mapper = new() { SerializeNullValues = true };
        private readonly DatabaseSettings settings;

        public IEquatable Key => new Key<LiteDBRepository>(Guids.LiteDB);
        private Lazy<LiteDatabase> liteDatabase;
        private PropertyInfo? propertyInfo;

        private ILiteCollection<BsonDocument> collection => liteDatabase.Value.GetCollection<BsonDocument>(settings.Type.Name);
        //private ILiteCollection<Type> typeCollection => liteDatabase.Value.GetCollection<Type>();
        private ILiteCollection<MetaData> metaDataCollection => liteDatabase.Value.GetCollection<MetaData>();

        public ViewModelRepository(DatabaseSettings settings)
        {
            Directory.CreateDirectory(settings.Path);
            this.settings = settings;
            liteDatabase = new(() => new LiteDatabase(Path.Combine(settings.Path, "data.litedb")));
            _mapper = new() { SerializeNullValues = true, };
        }


        public Task<IEquatable[]> FindKeys(IEquatable key)
        {
            throw new NotImplementedException();
        }

        public Task<object?> FindValue(IEquatable? equatable)
        {
            if (equatable is not _Key { } key)
            {
                throw new Exception("vsd s33322 vd");
            }
            List<MetaData> results = new();

            metaDataCollection.EnsureIndex(a => a.ParentId);


            var idMatchItems = metaDataCollection.Query()
                .Where(a => a.ParentId == key.Guid)
                //.Include(a=>a.ValuePairs)
                .ToArray();
            results.AddRange(idMatchItems);

            if (key.Type is System.Type type)
            {
                var dbType = new Type { Assembly = type.Assembly.FullName, Namespace = type.Namespace, Name = type.Name };
                metaDataCollection.EnsureIndex(a => a.Type);
                var items = metaDataCollection.Query()
                    .Where(a => a.Type == dbType)
                    //.Include(a => a.ValuePairs)
                    .ToArray();
                results.AddRange(items);
            }

            while (key.Child != null)
            {
                results.AddRange(metaDataCollection
                    .Query()
                    .Where(a => a.ParentId == key.Child.Guid)
                    //.Include(a => a.ValuePairs)
                    .ToArray());
                key = key.Child;
            }
            results.Reverse();

            return Task.FromResult((object?)results);
        }

        public Task Update(IEquatable equatable, object value)
        {
            if (equatable is not _Key key)
            {
                throw new Exception("vsd s33322 vd");
            }

            BsonDocument metaDocument = _mapper.ToDocument(typeof(MetaData), equatable);

            var metaData = _mapper.ToObject<MetaData>(metaDocument);

            //metaData.Inherit = document["Inherit"];
            //metaData.Mode = document["Mode"].ToString();

            //metaData.ValuePairs = value as Dictionary<string,string>;
            //if (settings.IdPropertyName != default)
            //{
            //    var id = (propertyInfo ?? value.GetType().GetProperty(settings.IdPropertyName))?.GetValue(value) ?? throw new Exception("sd ee222");
            //    var guid = (Guid)id;
            //    document["_id"] = guid;
            //}

            var dbType = key.Type != null ? new Type { Assembly = key.Type.Assembly.FullName, Namespace = key.Type.Namespace, Name = key.Type.Name } : default;

            //var metaData = new MetaData
            //{
            //    //Id = key.Guid,
            //    //ValueId = document["ValueId"].AsGuid,
            //    //Inherit = document["Inherit"],
            //    //Mode = document["Mode"].ToString(),
            //    //Name = key.Name,
            //    ParentId = key.Guid,
            //    Type = dbType
            //    ValuePairs = 
            //};

            metaDataCollection.Upsert(metaData);


            BsonDocument document = _mapper.ToDocument(settings.Type, value);
            //document["_id"] = key.Guid;
            //document["Name"] = key.Name;

            var update = (object)collection.Upsert(document);


            //int? _typeId = default;
            //if (key.Type is System.Type type)
            //{
            //    typeCollection.EnsureIndex(a => a.Assembly);
            //    typeCollection.EnsureIndex(a => a.Namespace);
            //    typeCollection.EnsureIndex(a => a.Name);
            //    _typeId = typeCollection.FindOne(a => a.Assembly == type.Assembly.FullName && a.Namespace == type.Namespace && a.Name == type.Name)?.Id;

            //    if (_typeId.HasValue == false)
            //    {
            //        _typeId = typeCollection.Insert(dbType).AsInt32;
            //    }
            //}

            //document["Type"] = _typeId ?? throw new Exception("dfv dfsdsssss");


            //var update = collection.Upsert(document);

            //while (key.Parent is Key parent)
            //{
            //    BsonDocument keyDocument = new(new Dictionary<string, BsonValue>() { { "Key", new BsonValue(key.Guid) }, { "Parent", new BsonValue(parent.Guid) } });
            //    key = parent;
            //    collection.Upsert(keyDocument);
            //}

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            liteDatabase.Value.Dispose();
        }
    }
}
