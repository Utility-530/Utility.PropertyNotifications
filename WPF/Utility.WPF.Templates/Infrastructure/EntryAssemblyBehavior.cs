using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.WPF.Controls.ComboBoxes;
using Utility.WPF.Controls.Trees;

namespace Utility.WPF.Templates.Infrastructure
{
    public class EntryAssemblyBehavior:Behavior<TypeSelector>
    {
        protected override void OnAttached()
        {
            this.AssociatedObject.Assemblies = new[] { Assembly.GetEntryAssembly() };
            base.OnAttached();
        }
    }
}
