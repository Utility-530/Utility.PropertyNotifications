using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace ReactiveAsyncWorker.Wpf
{
    //public class WorkerItem<T> : ReactiveAsyncWorker.WorkerItem<T>,INotifyPropertyChanged
    //{

    //    private T output;
    //    public override T Output { get=>output; protected set { this.output = value;OnPropertyChanged(); } }

    //    public WorkerItem(Action action, string key) : base(action, key)
    //    {
         
    //        Completed.Subscribe(_ => { Output = _; OnPropertyChanged(nameof(Output)); });
    //    }

    //    #region INotifyPropertyChanged Implementation
    //    /// <summary>
    //    /// Occurs when any properties are changed on this object.
    //    /// </summary>
    //    public event PropertyChangedEventHandler PropertyChanged;


    //    /// <summary>
    //    ///  raises the PropertyChanged event for one to many properties.
    //    /// </summary>
    //    /// <param name="propertyNames">The names of the properties that changed.</param>
    //    public virtual void OnPropertiesChanged(params string[] propertyNames)
    //    {
    //        foreach (string name in propertyNames)
    //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    //    }

    //    /// <summary>
    //    ///  raises the PropertyChanged event for a single property
    //    ///  'propertyname' can be left null (e.g OnPropertyChanged()), if called from body of property 
    //    /// </summary>
    //    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    /// <summary>
    //    ///  raises the PropertyChanged event for a single property
    //    ///  'propertyname' can be left null (e.g OnPropertyChanged()), if called from body of property 
    //    /// </summary>
    //    public void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
    //    {
    //        property = value;
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }


    //    #endregion INotifyPropertyChanged Implementation
    //}
}
