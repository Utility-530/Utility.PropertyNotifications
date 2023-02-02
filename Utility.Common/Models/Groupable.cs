using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utility.Common
{
    public record struct ClassProperty(string Name, string ClassName);

    public interface IGroupable<T>
    {
        T Value { get; }
    }

    public class Groupable : ReactiveObject, IGroupable<object>, IObserver<ClassProperty>
    {
        private readonly PropertyType propertyType;
        private readonly object value;

        public Groupable(object value, ClassProperty? classProperty = null, IObservable<ClassProperty>? observable = null)
            : this(value, new ValuePropertyType(value), classProperty, observable)
        {
        }

        public Groupable(object value, PropertyType propertyType, ClassProperty? classProperty = null, IObservable<ClassProperty>? observable = null) :
            this(value, propertyType, classProperty?.Name ?? propertyType.Names.First(), observable)
        {
            if (classProperty.HasValue && propertyType.Type.GetProperty(classProperty.Value.Name) is null)
                throw new Exception("99df 33");
        }

        private Groupable(object value, PropertyType propertyType, string name, IObservable<ClassProperty>? observable = null)
        {
            this.propertyType = propertyType;
            this.value = value;
            GroupProperty = propertyType.Value(value, name);
            observable?.Subscribe(this.OnNext);
        }

        public string GroupProperty { get; protected set; }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public virtual object Value => value;

        public void OnNext(ClassProperty classProperty)
        {
            //if (classProperty.ClassName != propertyType.Name)
            //    throw new Exception("99df 33");
            if (propertyType.Has(classProperty.Name) == false)
                throw new Exception("j 977  9df 33");
            GroupProperty = propertyType.Value(this.value, classProperty.Name);
            this.RaisePropertyChanged(nameof(GroupProperty));
        }
    }

    public class Groupable<T> : Groupable, IGroupable<T> where T : class
    {
        public Groupable(T value, ClassProperty classProperty, IObservable<ClassProperty>? observable = null) : base(value, classProperty, observable)
        {
            Value = value;
        }

        public Groupable(T value, PropertyType propertyType, ClassProperty classProperty, IObservable<ClassProperty>? observable = null) : base(value, propertyType, classProperty, observable)
        {
            Value = value;
        }

        public override T Value { get; }
    }

    public class PropertyType<T> : PropertyType
    {
        public PropertyType() : base(typeof(T))
        {
        }
    }

    public class ValuePropertyType : PropertyType
    {
        private readonly object value;

        public ValuePropertyType(object value) : base(value.GetType())
        {
            this.value = value;
        }

        public string Value(string property) => Value(value, property);
    }

    public class PropertyType
    {
        protected Dictionary<string, PropertyInfo> properties;

        public string Name { get; private set; }

        public PropertyType(Type type)
        {
            this.Type = type;
            this.properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(a => a.Name, a => a);
            Name = type.Name;
        }

        public IReadOnlyCollection<string> Names => properties.Keys;

        public Type Type { get; }

        public bool Has(string property) => properties.ContainsKey(property);

        public string Value(object value, string property) => properties[property].GetValue(value)?.ToString() ?? string.Empty;
    }
}