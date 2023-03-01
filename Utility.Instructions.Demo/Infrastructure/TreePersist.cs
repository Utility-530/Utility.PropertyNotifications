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
using Utility.Persist;
using Utility.Persist.Infrastructure;
using Utility.Trees;

namespace Utility.Instructions.Demo
{

    public interface IOrm
    {
        IFreeSql Orm { get; set; }
    }

    public class Persist : IOrm, IPersist
    {
        public IFreeSql Orm { get; set; }

        [Column(IsIdentity = true, IsPrimary = true)]
        public Guid Key { get; set; }
        public string Name { get; set; }

        public async void Load(Guid key)
        {
            Key = key;
            var x = await Orm.Select<Persist>().Where(a => a.Key == key).ToListAsync();
            Name = x.Single().Name;
        }

        public void Save(Guid key)
        {
            this.Key = key;
            Orm.Insert(this).ExecuteAffrows();
        }
        public override string ToString()
        {
            return Name;
        }
    }

    public class TreePersist
    {
        private TreePersist()
        {
            FreeSqlFactory.InitialiseSQLite();
        }

        public void Save<T>(ITree<T> tree) where T : IOrm, IPersist
        {
            using (var uow = BaseEntity.Orm.CreateUnitOfWork())
            {

                List<ChildParentPair> pairs = new();
                tree.Visit(t =>
                {
                    var key = t.Parent?.Key;
                    if (key is Guid k)
                    {
                        t.Data.Orm = uow.Orm;
                        t.Data.Save(t.Key);
                        var x = new ChildParentPair { Parent = k, Child = t.Key };
                        pairs.Add(x);
                    }
                    else
                    {
                        t.Data.Orm = uow.Orm;
                        t.Data.Save(t.Key);        
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

                //tree.Visit(t =>
                //{
                //    var key = t.Parent?.Key;
                //    if (key is Guid k)
                //    {
                //        t.Data.Save(t.Key);
                //        var x = new ChildParentPair { Parent = k, Child = t.Key };
                //        pairs.Add(x);
                //    }
                //});

                var pairs = uow.Orm.Select<ChildParentPair>().ToList();
                var keys = pairs.Select(a => a.Parent).ToList();
                
                Tree<T> tree = null;
                var persist = pairs.GetEnumerator();
                while(keys.Count > 0) 
                {
                    if( persist.MoveNext()==false)
                        persist =  pairs.GetEnumerator();

                    var pair = persist.Current;


                    if (tree == null)
                    {
                        var t2 = new T() { Orm = uow.Orm };
                        t2.Load(pair.Parent);
                        tree = new Tree<T>(t2) { Key = pair.Parent};
                    }                
                    if(tree.Match(pair.Parent) is ITree<Persist> branch)
                    {
                        var t = new T() { Orm = uow.Orm };
                        t.Load(pair.Child);
                        branch.Add(new Tree<T>(t) { Key = pair.Child });
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

