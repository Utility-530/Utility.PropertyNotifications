//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Utility.Models
//{
//    public class Property<T> : Property, INotifyPropertyChanged
//    {
//        private readonly IEqualityComparer<T>? equalityComparer;
//        //private readonly ReplaySubject<T> subject = new ReplaySubject<T>(1);
//        private T value = default!;

//        public event PropertyChangedEventHandler? PropertyChanged;

//        public Property()
//        {
//        }

//        public Property(T value)
//        {
//            Value = value;
//        }

//        public virtual T Value
//        {
//            get => value;
//            set => throw new Exception("Unable to set readonly property");
//        }

//        public void RaisePropertyChanged([CallerMemberName] string? callerMemberName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
//        }

//        public IDisposable Subscribe(IObserver<T> observer)
//        {
//            throw new NotImplementedException();
//            //return subject.Subscribe(observer);
//        }
//    }

//    public class Property
//    {
//    }

//    public class StringProperty : Property
//    {
//        public StringProperty(string value) => Value = value;
//        public string Value { get; set; }

//        public static implicit operator string(StringProperty stringProperty)
//        {
//            return stringProperty.Value;
//        }

//        public static implicit operator StringProperty(string stringValue)
//        {
//            return new StringProperty(stringValue);
//        }
//    }

//}