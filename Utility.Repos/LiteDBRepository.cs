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

        public LiteDBRepository(DatabaseSettings settings)
        {
            Directory.CreateDirectory(settings.Path);
            this.settings = settings;
        }


        public Task<IEquatable> FindKeyByParent(IEquatable key)
        {
            throw new NotImplementedException();
        }

        public Task<object?> FindValue(IEquatable? equatable)
        {
            if (equatable is not Key key)
            {
                throw new Exception("vsd s33322 vd");
            }

            if (key.Name == null)
            {
                List<object> objects = new();
                using (GetCollection(out var collection))
                {
                    foreach(var item in collection.FindAll())
                    {
                        var deserialised = _mapper.Deserialize(settings.Type, item);
                        objects.Add(deserialised);
                    }
                    return Task.FromResult((object)objects);
                }
            }
            using (GetCollection(out var collection))
            {
                var find = collection.FindById(new BsonValue(key.Guid));
                if (find != null)
                    return Task.FromResult(_mapper.Deserialize(settings.Type, find));

                var activated = Activator.CreateInstance(settings.Type);
                BsonDocument document = _mapper.ToDocument(settings.Type, activated);
                document["_id"] = key.Guid;
                document["Name"] = key.Name;
                var insert = (object)collection.Insert(document);
                return Task.FromResult(activated);
            }
        }

        public Task UpdateValue(IEquatable equatable, object value)
        {
            if (equatable is not Key key)
            {
                throw new Exception("vsd s33322 vd");
            }

            BsonDocument document = _mapper.ToDocument(settings.Type, value);
            document["_id"] = key.Guid;
            document["Name"] = key.Name;

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
