using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Utility.Nodify.Core;

namespace Utility.Nodify.Operations.Infrastructure
{
    public interface INodeSource
    {
        IEnumerable<MenuItem> Filter(bool? isInput, Type? type);

        Node Find(MenuItem guid);
    }

    public record MenuItem(string Key, Guid Guid);
}
