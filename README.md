[![NuGet version (Newtonsoft.Json.Bson)](https://img.shields.io/nuget/v/Utility.PropertyNotifications)](https://www.nuget.org/packages/Utility.PropertyNotifications)

# Utility.PropertyNotifications

- Simplifies raising **INotifyPropertyChanged** events with methods [e.g RaisePropertyChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)]
that sit inside the get/set brackets and handles value comparison and updates
- Simplifies subscribing to **INotifyPropertyChanged** events with customisable extension methods [e.g WithChangesTo] that with ReactiveEx can be used for neat subscriptions
- Adds two other interfaces, **INotifyPropertyCalled** and **INotifyPropertyReceived**, with methods comparable to those for INotifyPropertyChanged for greater control over reacting to property getting/setting
- Includes, NotifyPropertyClass - Base class providing methods for raising, calling, and receiving data changes
- Leverages CallerMemberName attribute to avoid string literals
- .NET Standard 2.0

## Example:

    using Utility.PropertyNotifications;


    public class MainViewModel : NotifyPropertyClass
    {

      private string _someProperty;
      private string _otherProperty;

      public string SomeProperty
      {
          get
          {
              RaisePropertyCalled(_someProperty);
              return _someProperty;
          }
          set
          {
              RaisePropertyReceived(ref _someProperty, value);
          }
      }

      public string OtherProperty
      {
          get => _otherProperty;
          set
          {
              RaisePropertyChanged(ref _otherProperty, value);
          }
      }
    }
  
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reactive;
    using System.Reactive.Linq;

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




This project is licensed under the MIT License - see the LICENSE file for details.
Support
