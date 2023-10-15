using System;
using Utility.PropertyTrees.Demo.Model;

namespace Utility.PropertyTrees.WPF.Demo.Basic
{
    public class RootModelProperty : RootProperty
    {
        static readonly Guid guid = Guid.Parse("febe5f0b-6024-4913-8017-74475096fc52");

        public RootModelProperty() : base(guid)
        {
            Data = new Leader();
        }
    }
}
