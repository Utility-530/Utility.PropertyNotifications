//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.CompilerServices;

//namespace Utility.ViewModels.Base
//{
//    public record PropertyAction(object Instance, PropertyInfo PropertyInfo);
//    public record PropertySet(object Instance, PropertyInfo PropertyInfo, object Value) : PropertyAction(Instance, PropertyInfo);
//    public record PropertyGet(object Instance, PropertyInfo PropertyInfo) : PropertyAction(Instance, PropertyInfo);
//    public record PropertyChange(object Instance, PropertyInfo PropertyInfo, object Value) : PropertyAction(Instance, PropertyInfo);

//    public class BaseViewModel : NotifyPropertyChanged, IObservable<PropertyAction>, IObserver<PropertyChange>
//    {
//        //private bool isSelected;
//        //private bool isChecked;
//        //private bool isVisible;
//        //private bool isReadOnly;
//        //private string name;
//        //private int row;
//        //private int column;
//        static readonly Dictionary<string, PropertyInfo> dictionary = new();

//        static BaseViewModel() => dictionary = typeof(BaseViewModel).GetProperties().ToDictionary(a => a.Name, a => a);

//        public ObservableCollection<PropertyChange> Changes { get; } = new();

//        public ObservableCollection<IObserver<PropertyAction>> Observers { get; } = new();

//        public bool IsSelected
//        {
//            get => Get<bool>(); set => Set(value);
//        }

//        public bool IsChecked
//        {
//            get => Get<bool>(); set => Set(value);
//        }

//        public bool IsVisible
//        {
//            get => Get<bool>(); set => Set(value);
//        }

//        public bool IsReadOnly
//        {
//            get => Get<bool>(); set => Set(value);
//        }

//        public string Name
//        {
//            get => Get<string>(); set => Set(value);
//        }

//        public int Row
//        {
//            get => Get<int>(); set => Set(value);
//        }

//        public int Column
//        {
//            get => Get<int>(); set => Set(value);
//        }

//        protected T Get<T>([CallerMemberName] string? calledMemberName = null)
//        {
//            if(Changes.SingleOrDefault(a => a.PropertyInfo == dictionary[calledMemberName]) is T t)
//            {
//                return t;
//            }
//            foreach (var observer in Observers)
//                observer.OnNext(new PropertyGet(this, dictionary[calledMemberName]));

//            return default;
//        }

//        protected void Set(object value, [CallerMemberName] string? calledMemberName = null)
//        {
//            foreach (var observer in Observers)
//                observer.OnNext(new PropertySet(this, dictionary[calledMemberName], value));
//        }

//        public IDisposable Subscribe(IObserver<PropertyAction> observer)
//        {
//            return new Utility.Observables.Generic.Disposer<PropertyAction>(Observers, observer);
//        }

//        public void OnCompleted()
//        {
//            throw new NotImplementedException();
//        }

//        public void OnError(Exception error)
//        {
//            throw new NotImplementedException();
//        }

//        public void OnNext(PropertyChange value)
//        {
//            Changes.Add(value);
//            this.RaisePropertyChanged(value.PropertyInfo.Name);
//        }
//    }

//    public class NotifyPropertyChanged : INotifyPropertyChanged
//    {
//        public event PropertyChangedEventHandler PropertyChanged;

//        /// <summary>
//        ///  raises the PropertyChanged event for a single property
//        ///  'propertyname' can be left null (e.g OnPropertyChanged()), if called from body of property
//        /// </summary>
//        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }

//        /// <summary>
//        ///  raises the PropertyChanged event for a single property
//        ///  'propertyname' can be left null (e.g OnPropertyChanged()), if called from body of property
//        /// </summary>
//        public void RaisePropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
//        {
//            property = value;
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}