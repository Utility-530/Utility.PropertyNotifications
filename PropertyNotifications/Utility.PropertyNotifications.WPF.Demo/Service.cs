using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;
using System.Reactive.Linq;

namespace Utility.PropertyNotifications.WPF.Demo
{
    public class Service
    {
        public Service(MainViewModel mainViewModel, IDataStore dataStore)
        {
            mainViewModel
                .WithChangesTo(a => a.OtherProperty, includeNulls: false, includeInitialValue: true, includeDefaultValues: true)
                .CombineLatest(mainViewModel.WithChangesTo(a => a.SomeProperty))
                .Subscribe(o =>
                {
                    Console.WriteLine(o.First + o.Second);
                });

            mainViewModel
                .WhenReceivedFrom(a => a.SomeProperty, includeNulls: true, includeInitialValue: false, includeDefaultValues: true)
                .Subscribe(value =>
                {
                    if (value == "name")
                    {
                        mainViewModel.RaisePropertyChanged(nameof(MainViewModel.SomeProperty));
                    }
                });

            mainViewModel
                .WhenCalledFrom(a => a.SomeProperty)
                .Subscribe(async o =>
                {
                    var value = await dataStore.Value<string>(nameof(MainViewModel.SomeProperty));
                    mainViewModel.SomeProperty = value;
                });
        }
    }

    public interface IDataStore
    {
        Task<T> Value<T>(string name);
    }
}
