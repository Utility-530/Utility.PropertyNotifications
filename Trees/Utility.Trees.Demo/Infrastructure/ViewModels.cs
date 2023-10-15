using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.Trees.Demo.Two;

namespace Utility.Trees.Demo.Infrastructure
{
    public class ConnectionsViewModel : Jellyfish.ViewModel
    {
        private bool isLine;
        private Point? point0;
        private Point? point1;
        private LineViewModel last;

        public bool IsLine { get => isLine; set => this.Set(ref isLine, value); }
        public ObservableCollection<LineViewModel> Lines { get; } = new();
        public ObservableCollection<ConnectionModel> Connections { get; } = new();
        public ITree ViewModel { get; set; }

        public List<Service> ServiceModel { get; set; }

        public Point? Point0 { get => point0; set => Set(ref point0, value); }
        public Point? Point1 { get => point1; set => Set(ref point1, value); }

        public LineViewModel Last { get => last; set => Set(ref last, value); }

    }

    public class LineViewModelInfo
    {
        public bool IsPersisted { get; set; }
    }

    public class LineViewModel : Jellyfish.ViewModel
    {
        private Point _startPoint;

        public Guid Guid { get; set; }

        public Point StartPoint
        {
            get { return _startPoint; }
            set
            {
                _startPoint = value;
                this.OnPropertyChanged();
            }
        }

        private Point _endPoint;
        public Point EndPoint
        {
            get { return _endPoint; }
            set
            {
                _endPoint = value;
                this.OnPropertyChanged();
            }
        }
    }


    public class ConnectionModel : IGuid
    {
        public Guid Guid { get; set; }

        public Guid ViewModelGuid { get; set; }

        public Guid ServiceGuid { get; set; }
    }


    public class Property
    {
        public string Name { get; set; }
        public ENativeType Type { get; set; }
        public object Value { get; set; }
    }


    public class ButtonViewModel : PersistViewModel
    {
        public bool IsPressed { get; set; }
    }


    /// <inheritdoc />
    /// <summary>
    ///     The observable base class for every object that needs to update subscribers about certain changes using
    ///     <see cref="T:System.ComponentModel.INotifyPropertyChanged" />
    ///     <para />
    ///     (For ViewModels use <see cref="ViewModel" />)
    /// </summary>
    public abstract class PersistViewModel : Persist, INotifyPropertyChanged, IInitialise
    {
        /// <inheritdoc />
        /// <summary>
        ///     The event on property changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Initialise(object o)
        {
            Guid = Guid.NewGuid();
        }

        /// <summary>
        ///     Raise the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The caller member name of the property (auto-set)</param>
        //[NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Set a property and raise the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <typeparam name="T">The type of the Property</typeparam>
        /// <param name="field">A reference to the backing field from the property</param>
        /// <param name="value">The new value being set</param>
        /// <param name="callerName">The caller member name of the property (auto-set)</param>
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string? callerName = default)
        {
            if (field?.Equals(value) != true)
            {
                field = value;
                OnPropertyChanged(callerName);
                return true;
            }
            return false;
        }
    }

}
