using BFF.DataVirtualizingCollection.DataVirtualizingCollection;
using DynamicData;
using Endless;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using UtilityWpf.Demo.Data.Factory;
using UtilityWpf.Demo.Data.Model;
using VirtualisationHelper = Utility.Helpers.Ex.VirtualisationHelper;

namespace UtilityWpf.Demo.Controls
{
    public class ProfileCollectionVirtualise1 : ReactiveObject
    {
        private int val = 1;
        private readonly ObservableAsPropertyHelper<IList<Profile>> profiles;

        public ProfileCollectionVirtualise1()
        {
            profiles = this.WhenAnyValue(a => a.Value).Select(GetProfiles).ToProperty(this, a => a.Profiles);

            IList<Profile> GetProfiles(int i)
            {
                var ProfilePool = ProfileFactory.BuildPool();
                return DataVirtualizingCollectionBuilder
                    .Build<Profile>(i, RxApp.MainThreadScheduler)
                 .NonPreloading()
                 .Hoarding()
                 .NonTaskBasedFetchers(
                     (offset, pageSize) =>
                     {
                         Console.WriteLine($"{nameof(Profiles)}: Loading page with offset {offset}");
                         var range = Enumerable.Range(offset, pageSize).Select(i => ProfilePool[i % ProfilePool.Count]).ToArray();
                         return range;
                     },
                     () =>
                     {
                         Console.WriteLine($"{nameof(Profiles)}: Loading count");
                         return 420420;
                     })
                 .AsyncIndexAccess((_, __) => new Profile());
            }
        }

        public IList<Profile> Profiles => profiles.Value;

        public int Value { get => val; set => this.RaiseAndSetIfChanged(ref val, value); }
    }

    public class ProfileCollectionVirtualiseLimited
    {
        private readonly ReadOnlyObservableCollection<Profile> profiles;

        /// <summary>
        /// Only adds to the pool of data when asked to
        /// </summary>
        /// <param name="virtualRequests"></param>
        public ProfileCollectionVirtualiseLimited(IObservable<IVirtualRequest> virtualRequests)
        {
            var pool = ProfileFactory.BuildPool();

            var cached = 0.Iterate(a => a + 1)
                 .Select(i => (i, pool.Random()))
                 .Cached();

            _ =
              virtualRequests
                .SelectMany(a => cached.Skip(a.StartIndex).Take(a.Size + 30))
               .ToObservableChangeSet(a => a.i)
               .Transform(a => a.Item2)
                        .Bind(out profiles)
                        .Subscribe();
        }

        public ReadOnlyObservableCollection<Profile> Profiles => profiles;
    }

    public class ProfileCollectionVirtualise
    {
        private readonly ReadOnlyObservableCollection<Profile> profiles;

        /// <summary>
        /// Creates an initial set of blank data then fills when requested
        /// </summary>
        /// <param name="virtualRequests"></param>
        /// <param name="initialSize"></param>
        public ProfileCollectionVirtualise(IObservable<IVirtualRequest> virtualRequests, int initialSize)
        {
            var pool = ProfileFactory.BuildPool();
            var items = new Func<Profile>(pool.Random).Repeat();

            _ = VirtualisationHelper.CreateChangeSet(items, virtualRequests, initialSize)
                .Bind(out profiles)
                .Subscribe();
        }

        public ReadOnlyObservableCollection<Profile> Profiles => profiles;
    }
}