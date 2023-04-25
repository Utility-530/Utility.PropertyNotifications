using Microsoft.AspNetCore.Mvc;
using Utility.Trees;

namespace Utility.PropertyTrees.API.Controllers
{
    public class KeyValue
    {
        public KeyValue(Guid key, string value)
        {
            Key = key;
            Value = value;
        }

        public Guid Key { get; set; }
        public string Value { get; set; }
    }

    public class Repository
    {
        public Kaos.Collections.RankedDictionary<Guid, Guid> Dictionary { get; } = new();

        public Tree Tree { get; } = new();

        public static Repository Instance { get; } = new Repository();
    }

    [ApiController]
    [Route("[controller]/[action]")]
    public class Controller : ControllerBase
    {
        private Tree tree => Repository.Instance.Tree;
        private Kaos.Collections.RankedDictionary<Guid, Guid> Dictionary => Repository.Instance.Dictionary;

        [HttpPost()]
        public void PostValue(Guid key, string value)
        {
            Console.WriteLine(nameof(PostValue));
            Console.WriteLine("Key: " + key);
            Console.WriteLine("Value: " + value);
            var parent = tree[key];
            parent.Add(new Tree(value) { Key = Guid.NewGuid(), Parent = parent });
            Dictionary[Guid.NewGuid()] = key;
        }

        [HttpGet]
        public KeyValue[] GetChanges(Guid? guid)
        {
            Console.WriteLine(nameof(GetChanges));
            Console.WriteLine("Key: " + guid);
            List<KeyValue> keyValueList = new List<KeyValue>();
            foreach (var keyValue in Dictionary.ElementsBetween(guid ?? Dictionary.MinKey, Dictionary.MaxKey))
            {
                var key = keyValue.Value;
                keyValueList.Add(new KeyValue(key, tree[key].Items.Last().ToString()));
            }
            return keyValueList.ToArray();
        }

        [HttpGet]
        public Guid GetKeyByParent(Guid key, string name)
        {
            Console.WriteLine(nameof(GetKeyByParent));
            Console.WriteLine("Key: " + key);
            Console.WriteLine("Name: " + name);
            lock (tree)
            {
                ITree? parent = tree[key];

                if (parent == default)
                {
                    parent = new Tree() { Key = key, Parent = tree };
                    tree.Add(parent);
                    var guid = Guid.NewGuid();
                    var childTree = new Tree(name) { Key = guid, Parent = parent };
                    parent.Add(childTree);
                    return guid;
                }
                else
                {
                    var childTree = (tree[key] ?? throw new Exception("vdf 44gfgdf"))
                    .Items
                   .SingleOrDefault(a => a.Data.Equals(name));

                    if (childTree == default)
                    {
                        var guid = Guid.NewGuid();
                        parent.Add(new Tree(name) { Key = guid, Parent = parent });
                        return guid;
                    }
                    else
                    {
                        return childTree.Key;
                    }
                }
            }
        }

        [HttpGet]
        public string GetValue(Guid key)
        {
            Console.WriteLine(nameof(GetValue));
            Console.WriteLine("Key: " + key);
            if ((tree[key] ?? throw new Exception("82228df 44gfgdf")).Any())
            {
                object? data = tree[key].Items[^1].Data;
                return (string)data;
            }
            return default;
        }
    }
}