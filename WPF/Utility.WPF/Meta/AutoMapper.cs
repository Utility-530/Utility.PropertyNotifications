using Utility.EventArguments;

namespace Utility.WPF.Meta
{
    public class Profile : AutoMapper.Profile
    {
        public Profile() : base(nameof(Utility.WPF.Meta))
        {
            CreateMap<Abstract.CollectionItemEventArgs, CollectionItemEventArgs>();
            CreateMap<Abstract.CollectionEventArgs, CollectionEventArgs>();
            CreateMap<Abstract.MovementEventArgs, MovementEventArgs>();
            CreateMap<Abstract.CollectionItemChangedEventArgs, CollectionChangedEventArgs>();
        }
    }
}