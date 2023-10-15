using DryIoc;
using MintPlayer.ObservableCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Trees.Demo.Two
{
    public class BootStrapper
    {

        public BootStrapper(Container container)
        {
            container.Register<ViewModel>();
            container.Register<ViewModelService>();
        }
    }

    public class ViewModel: IName
    {
        public ObservableCollection<ViewModel> Children { get; set; }
        public string Name { get; set; }
    }


    public interface IName
    {
        string Name { get; set; }
    }

    public class Service : IName
    {
        public string Name { get; set; }


 
    }

    public class ViewModelService : Service
    {
        public IEnumerable<ViewModel> Create()
        {
            yield return new ViewModel { Name = "Creation" };
        }
    }

    //public class TreeViewModel : IName
    //{
    //    public string Name { get; set; }
    //}
}
