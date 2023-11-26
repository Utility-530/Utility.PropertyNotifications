using Dapper.FastCrud;
using FreeSql;
using FreeSql.DataAnnotations;
using Jellyfish.DependencyInjection;
using NetFabric.Hyperlinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Persists;
using Utility.Trees.Abstractions;

namespace Utility.Trees.Demo.Infrastructure
{

    public interface IOrm
    {
        IFreeSql Orm { get; set; }
    }

    public class Persist : IOrm, IPersist, IClone, IGuid, IName
    {

        public Persist()
        {
        }

        public IFreeSql Orm { get; set; }

        [Column(IsIdentity = true, IsPrimary = true)]
        public Guid Guid { get; set; }
        public string Name { get; set; }

        public object Clone()
        {
            return new Persist { Guid = Guid.NewGuid(), Name = this.Name + " 1" };
        }

        public async virtual void Load(Guid key)
        {
            Guid = key;
            var x = await Orm.Select<Persist>().Where(a => a.Guid == key).ToListAsync();
            var single = x.SingleOrDefault();
            Name = single?.Name;
        }

        public virtual void Save(Guid key)
        {
            this.Guid = key;
            Orm.Insert(this).ExecuteAffrows();
        }

        public override string ToString()
        {
            return this.GetType().Name + " Name: " + Name;
        }
    }

    public class TreePersist
    {
        private TreePersist()
        {
            FreeSqlFactory.InitialiseSQLite();
        }

        public void Save(ITree tree)
        {
            using (var uow = BaseEntity.Orm.CreateUnitOfWork())
            {

                List<ChildParentPair> pairs = new();
                tree.Visit(t =>
                {
                    //if (t.IsRoot())
                    //    return;
                    if (t.Data is not IPersist persist)
                    {
                        throw new Exception("dsf");
                    }
                    if (t.Data is not IGuid key)
                    {
                        throw new Exception("ds2 w");
                    }
                    if (t.Parent is not null && t.Parent.Data is not IGuid)
                    {
                        throw new Exception("ds2 w");
                    }
                    var keyParent = t.Parent as IGuid;
                    //var key = t.Parent?.Key;

                        if (t.Data is IOrm orm)
                            orm.Orm = uow.Orm;
                        persist.Save(key.Guid);
                    if (t.IsRoot() == false)
                    {
                        var x = new ChildParentPair { Parent = keyParent?.Guid ?? Guid.Empty, Child = key.Guid };
                        pairs.Add(x);
                    }
                });

                uow.Orm.Insert(pairs).ExecuteAffrows();
                uow.Commit();
            }
        }

        public ITree<T> Load<T>() where T : IPersist, IOrm, new()
        {
            //List<ChildParentPair> pairs = new();
            using (var uow = BaseEntity.Orm.CreateUnitOfWork())
            {

                var pairs = uow.Orm.Select<ChildParentPair>().ToList();
                var keys = pairs.Select(a => a.Parent).ToList();

                Tree<T> tree = null;
                var persist = pairs.GetEnumerator();
                while (keys.Count > 0)
                {
                    if (persist.MoveNext() == false)
                    {
                        persist = pairs.GetEnumerator();
                    }
                    var pair = persist.Current;
                    if (pair == null)
                        return tree;

                    if (tree == null)
                    {
                        var t2 = new T() { Orm = uow.Orm };
                        t2.Load(pair.Parent);
                        tree = new Tree<T>(t2) { Key = new Key<Tree<T>>(pair.Parent) };
                    }
                    if (tree.Match(pair.Parent) is ITree<Persist> branch)
                    {
                        var t = new T() { Orm = uow.Orm };
                        t.Load(pair.Child);
                        branch.Add(new Tree<T>(t) { Key = new Key<Tree<T>>(pair.Child) });
                        keys.Remove(pair.Parent);
                    }
                    else
                    {

                    }
                }

                return tree;
            }
        }

        public static TreePersist Instance { get; } = new();

    }

    public class ChildParentPair : BaseEntity
    {

        [Column(IsIdentity = true, IsPrimary = true)]
        public Guid Parent { get; set; }
        public Guid Child { get; set; }
    }
}

