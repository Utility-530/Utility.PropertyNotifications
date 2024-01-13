using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility.Collections;
using Utility.Helpers;
using Utility.Infrastructure;
using Utility.Models;
using VM = Utility.PropertyTrees.Services.ViewModel;

namespace Utility.PropertyTrees.Demo.ViewModels
{
    public class RootModel : BaseViewModel
    {
        DateTime lastRefresh;

        public void Refresh()
        {
            LastRefresh = DateTime.Now;
        }

        public DateTime LastRefresh
        {
            get => lastRefresh;
            set
            {
                Set(ref lastRefresh, value);
            }
        }    

        public ViewModels ViewModels { get; set; } = new();
    }


    public class ViewModels : BaseViewModel
    {
        //private VM @default = new();
        private ViewModelsCollection collection = new();
        private Key key;
        private string name;
        private Guid guid;
        private string type;

        public void AddByName()
        {
            Collection.Add(new VM() { Id = Guid.NewGuid(), Name = Name });
        }

        public void AddByKey()
        {
            Collection.Add(new VM() { Id = Guid.NewGuid(), ParentGuid = Guid });
        }

        public void AddByType()
        {
            Collection.Add(new VM() { Id = Guid.NewGuid(), Type = type.FromString() });
        }

        public void Update()
        {
            Key = new Key(Guid, Name, System.Type.GetType(Type));
        }

        public ViewModelsCollection Collection { get => collection; set => collection = value; }

        public string Name
        {
            get => name; set
            {
                Set(ref name, value);
            }
        }

        public Guid Guid
        {
            get => guid; set
            {
                Set(ref guid, value);
            }
        }

        public string Type
        {
            get => type; set
            {
                Set(ref type, value);
            }
        }

        public Key Key
        {
            get => key;
            private set
            {
                this.Set(ref key, value);
            }
        }

        //public VM Default
        //{
        //    get => @default; set
        //    {
        //        @default = value;
        //    }
        //}
    }

    public class ViewModelsCollection : ThreadSafeObservableCollection<VM>
    {
        public ViewModelsCollection()
        {
            Context = SynchronizationContext.Current;
        }

        public void MoveUp(VM ViewModel)
        {
            var oldIndex = this.IndexOf(ViewModel);
            Move(oldIndex, oldIndex - 1);
        }

        public void MoveDown(VM ViewModel)
        {
            var oldIndex = this.IndexOf(ViewModel);
            Move(oldIndex, oldIndex + 1);
        }
    }


}
