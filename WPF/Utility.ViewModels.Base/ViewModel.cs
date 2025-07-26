using System.ComponentModel;
using System.Runtime.CompilerServices;
using Utility.Interfaces.NonGeneric;

namespace Utility.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    ///     The observable base class for every object that needs to update subscribers about certain changes using
    ///     <see cref="T:System.ComponentModel.INotifyPropertyChanged" />
    ///     <para />
    ///     (For ViewModels use <see cref="ViewModel" />)
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged
    {
        #region propertyChanged
        /// <inheritdoc />
        /// <summary>
        ///     The event on property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Raise the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The caller member name of the property (auto-set)</param>
        //[NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion propertyChanged


        /// <summary>
        ///     Set a property and raise the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <typeparam name="T">The type of the Property</typeparam>
        /// <param name="field">A reference to the backing field from the property</param>
        /// <param name="value">The new value being set</param>
        /// <param name="callerName">The caller member name of the property (auto-set)</param>
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string callerName = default)
        {
            if (field?.Equals(value) != true)
            {
                field = value;
                OnPropertyChanged(callerName);
                return true;
            }
            return false;
        }

        private object value;

        public object Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }

        public bool IsReadOnly { get; set; }
        public int GridRow { get; set; }
    }



    //public class ValueViewModel : ViewModel, IValue
    //{

    //}

}