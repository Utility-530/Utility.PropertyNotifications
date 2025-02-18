using DryIoc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.NonGeneric;

namespace Utility.Trees.Demo.Infrastructure
{
    public class Bootstrapper
    {
        Container container = new();

        public Bootstrapper()
        {

            container.Register<Persist, Persist>();
            container.Register<Persist, ViewModelPersist>();
            container.Register<ViewModel>();
            container.Register<Service, ViewModelService>();

        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public IEnumerable<T> ResolveMany<T>()
        {
            return container.ResolveMany<T>();
        }
    }


    public class ViewModelPersist : Persist
    {
        public async override void Load(Guid guid)
        {
            Guid = guid;
            var x = await Orm.Select<ViewModelPersist>().Where(a => a.Guid == guid).ToListAsync();
            Name = x.Single().Name;
        }

        public override void Save(Guid guid)
        {
            this.Guid = guid;
            Orm.Insert(this).ExecuteAffrows();
        }


        public ViewModelType Type { get; set; }

    }

    public enum ViewModelType
    {
        Text, Numeric,
    }


    public class ViewModel : IName, IGuid
    {
        public ObservableCollection<ViewModel> Children { get; set; }
        public string Name { get; set; }

        public Guid Guid { get; set; }
    }


    public class Service : IGetName, IGetGuid
    {
        public Guid Guid => Guid.Parse("ba941812-7711-49e8-a3ab-cb5284503215");

        public virtual string Name => nameof(Service);
    }

    public class ViewModelService : Service
    {
        public Guid Guid => Guid.Parse("9eab54d7-5b3b-40ac-b34e-a1215b833cd1");

        public IEnumerable<ViewModel> Create()
        {
            yield return new ViewModel { Name = "Creation" };
        }

        public override string Name => nameof(ViewModelService);
    }
}


