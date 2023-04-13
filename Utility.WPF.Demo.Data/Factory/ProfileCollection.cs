using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using DynamicData;
using Utility.WPF.Demo.Data.Model;

namespace Utility.WPF.Demo.Data.Factory
{
    public class ProfileCollectionTimed
    {
        //private const int _speed = 3;

        private readonly ReadOnlyObservableCollection<Profile> profiles;

        public ProfileCollectionTimed(int speed)
        {
            new ProfileCollectionObservable(30, speed)
                 .ToObservableChangeSet()
                 .Sort(new comparer())
                 .Bind(out profiles).Subscribe();
        }

        public ProfileCollectionTimed()
        {
            new ProfileCollectionObservable()
                 .ToObservableChangeSet()
                 .Sort(new comparer())
                 .Bind(out profiles).Subscribe();
        }

        public ReadOnlyObservableCollection<Profile> Profiles => profiles;

        private class comparer : IComparer<Profile>
        {
            public int Compare([AllowNull] Profile x, [AllowNull] Profile y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
    }

    public class ProfileCollectionSlow : ProfileCollectionTimed
    {
        public ProfileCollectionSlow() : base(6)
        {
        }
    }
}