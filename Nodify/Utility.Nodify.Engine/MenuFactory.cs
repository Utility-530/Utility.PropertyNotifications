using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Operations.Infrastructure;
using Utility.Nodify.ViewModels;

namespace Utility.Nodify.Engine
{
    public class MenuFactory : IMenuFactory
    {
        public IMenuItemViewModel? Create(MenuItem menuItem)
        {
            return new MenuItemViewModel() { Content = menuItem, Guid = menuItem.Guid };
        }
        public IMenuViewModel? CreateMenu()
        {
            return new MenuViewModel() { Inputs = [], Outputs = [] };
        }
    }
}
