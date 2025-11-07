namespace Utility.ViewModels.Base
{
    //public abstract class HeaderedReactiveObject : ReactiveObject
    //{
    //    public HeaderedReactiveObject(string header)
    //    {
    //        Header = header;
    //    }

    //    public string Header { get; }
    //}

    //public abstract class PropertyChangeRaiser : INotifyPropertyChanged, IReactiveObject
    //{
    //    #region INotifyPropertyChanged Implementation

    //    /// <summary>
    //    /// Occurs when any properties are changed on this object.
    //    /// </summary>
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    public event PropertyChangingEventHandler PropertyChanging;

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
    //    public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    /// <summary>
    //    ///  raises the PropertyChanged event for a single property
    //    ///  'propertyname' can be left null (e.g OnPropertyChanged()), if called from body of property
    //    /// </summary>
    //    public void RaisePropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
    //    {
    //        property = value;
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    public void RaisePropertyChanging(PropertyChangingEventArgs args)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    #endregion INotifyPropertyChanged Implementation
    //}
}