using LiteDB;
using System;
using Utility.Infrastructure.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Repos
{
    public partial class LiteDBRepository : IRepository
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
                serialize: (uri) => $"{uri.Namespace}, {uri.Assembly}.{uri.Name}",
                deserialize: (bson) => Type.GetType(bson.AsString)
                );
        }


        public Task<IEquatable[]> FindKeys(IEquatable key)
        {
            throw new NotImplementedException();
        }

        public Task<object?> FindValue(IEquatable? equatable)
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

            if (equatable is not Key key)
            {
                throw new Exception("vsd s33322 vd");
            }

            return Task.FromResult((object?)Objects());

            IEnumerable<object> Objects()
            {
                using (GetCollection(out var collection))
                {
                    var findByParentId = collection.Find(a => a["ParentGuid"].AsGuid == key.Guid);
                    var findByName = collection.Find(a => a["Name"] == key.Name);
                    var findByType = collection.Find(a => a["Type"] == _mapper.Serialize(key.Type));

                    if (findByParentId != null)
                        yield return findByParentId;
                    if (findByName != null)
                        yield return findByName;
                    if (findByType != null)
                        yield return findByType;

                    //var activated = Activator.CreateInstance(settings.Type);
                    //BsonDocument document = _mapper.ToDocument(settings.Type, activated);
                    //document["_id"] = key.Guid;
                    //document["Name"] = key.Name;
                    //var insert = (object)collection.Insert(document);
                    //return Task.FromResult(activated);
                }
            }
        }

        public Task UpdateValue(IEquatable equatable, object value)
        {
            if (equatable is not Key key)
            {
                throw new Exception("vsd s33322 vd");
            }

            BsonDocument document = _mapper.ToDocument(settings.Type, value);
            //document["_id"] = key.Guid;
            //document["Name"] = key.Name;
            //document["Type"] = key.Type.;

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
