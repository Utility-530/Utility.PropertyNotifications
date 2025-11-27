using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace Utility.Nodes.Demo.Editor
{


    public class GuidExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
            => Guid.NewGuid();
    }
}
