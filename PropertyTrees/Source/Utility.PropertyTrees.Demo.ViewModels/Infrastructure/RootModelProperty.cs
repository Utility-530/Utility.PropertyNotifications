using System;

namespace Utility.PropertyTrees.Demo.ViewModels
{
    public class RootModelProperty : RootProperty
    {
        static readonly Guid guid = Guid.Parse("febe5f0b-6024-4913-8017-74475096fc52");

        public RootModelProperty() : base(guid)
        {
            //Data = new Leader();
            Data = new RootModel();
        }
    }
}
