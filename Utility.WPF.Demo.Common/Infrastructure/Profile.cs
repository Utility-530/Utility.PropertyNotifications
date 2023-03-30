using Utility.WPF.Demo.Common.ViewModels;

namespace Utility.WPF.Demo.Common.Infrastructure
{
    public class Profile : AutoMapper.Profile
    {
        public Profile() : base(nameof(Common))
        {
            CreateMap<ReactiveFields, Fields>();
            CreateMap<Fields, ReactiveFields>();
        }
    }
}