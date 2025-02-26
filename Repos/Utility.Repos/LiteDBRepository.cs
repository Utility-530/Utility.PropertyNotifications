using LiteDB;
using System;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using _Key = Utility.Models.Key;

namespace Utility.Repos
{
    public interface ILiteRepository
    {
        IEquatable Key { get; }

        Task<object?> FindValue(object key);

        Task<IEquatable[]> FindKeys(object key);

        Task Update(object value);

        Task Remove(object value);

        Task<object[]> All();
    }

    public partial class LiteDBRepository : ILiteRepository
    {
        private BsonMapper _mapper = new() { SerializeNullValues = true };
        private readonly DatabaseSettings settings;

        public IEquatable Key => new Key<LiteDBRepository>(Guids.LiteDB);


        public LiteDBRepository(DatabaseSettings settings)
        {
            Directory.CreateDirectory(settings.Path);
            this.settings = settings;

            _mapper.RegisterType<Type>
                (
                serialize: (uri) => $"{uri.Namespace}.{uri.Name}, {uri.Assembly}",
                deserialize: (bson) => Type.GetType(bson.AsString)
                );
        }

        public Task<IEquatable[]> FindKeys(object key)
        {
            throw new NotImplementedException();
        }

        public Task<object[]> All()
        {
            List<object> objects = new();
            using (GetCollection(out var collection))
            {
                foreach (var item in collection.FindAll())
                {
                    var deserialised = _mapper.Deserialize(settings.Type, item);
                    objects.Add(deserialised);
                }
                return Task.FromResult(objects.ToArray());
            }
        }
        
        public Task Remove(object item)
        {     
            using (GetCollection(out var collection))
            {
                var x = _mapper.Serialize(settings.Type, item); 
                var result = collection.Delete(x["_id"]);                  
                return Task.CompletedTask;
            }
        }


        public Task<object?> FindValue(object? equatable)
        {

            if (equatable == null)
            {
                List<object> objects = new();
                using (GetCollection(out var collection))
                {
                    foreach (var item in collection.FindAll())
                    {
                        var deserialised = _mapper.Deserialize(settings.Type, item);
                        objects.Add(deserialised);
                    }
                    return Task.FromResult((object)objects);
                }
            }

            if (equatable is not _Key key)
            {
                throw new Exception("vsd s33322 vd");
            }

            var array = Objects().Select(item => _mapper.Deserialize(settings.Type, item)).ToArray();
            return Task.FromResult((object?)array);

            IEnumerable<BsonDocument> Objects()
            {
                using (GetCollection(out var collection))
                {
                    foreach (var findByParentId in collection.Find(a => a["ParentGuid"].AsGuid == key.Guid))
                        yield return findByParentId;

                    foreach (var findByName in collection.Find(a => a["Name"] == key.Name))
                        yield return findByName;

                    var type = _mapper.Serialize(key.Type);

                    foreach (var findByType in collection.Find(a => a["Type"] == type))
                        yield return findByType;
                }
            }
        }

        public Task Update(object value)
        {
            BsonDocument document = _mapper.ToDocument(settings.Type, value);

            using (GetCollection(out var collection))
            {
                var update = (object)collection.Upsert(document);
                return Task.CompletedTask;
            }
        }

        private IDisposable GetCollection(out ILiteCollection<BsonDocument> collection)
        {
            var db = new LiteDatabase(Path.Combine(settings.Path, "data.litedb"));
            collection = db.GetCollection<BsonDocument>(settings.Type.Name);
            return db;
        }
    }
}
