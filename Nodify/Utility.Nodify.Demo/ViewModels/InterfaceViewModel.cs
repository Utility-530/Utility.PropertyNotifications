using System.Collections.Generic;
using DryIoc;
using Utility.Models;
//using Autofac;

namespace Utility.Nodify.Demo
{
    public class InterfaceViewModel
    {
        private readonly IContainer container;

        public InterfaceViewModel(IContainer container)
        {
            this.container = container;
        }

        public ICollection<BaseViewModel> ViewModels => container.Resolve<ICollection<BaseViewModel>>();
    }
}
