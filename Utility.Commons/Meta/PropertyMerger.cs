using AutoMapper;
using Fasterflect;
using KellermanSoftware.CompareNetObjects;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilityWpf.Service
{
    public class PropertyMerger<TChange> : IEnableLogger
    {
        private class AutoMapperFactory
        {
            public static IMapper Create()
            {
                return new MapperConfiguration(cfg =>
                {
                    //  cfg.AddProfile(new MergeProfile());
                }).CreateMapper();
            }
        }

        private readonly Dictionary<string, Dictionary<string, (Type type, MemberSetter setter)>> properties = new();
        private readonly ObjectsComparer.Comparer<TChange> comparer = new();
        private readonly Lazy<IMapper> mapper = new(() => AutoMapperFactory.Create());
        //private readonly Lazy<IMapper> mapper = new(() => AutoMapperMapperFactory.Create());

        private PropertyMerger()
        {
        }

        public void Set<T>(T currentValue, TChange newValue)
        {
            var type = typeof(T);
            if (!properties.ContainsKey(type.Name))
            {
                properties[type.Name] = GetProperties(type);

                static Dictionary<string, (Type type, MemberSetter setter)> GetProperties(Type type)
                {
                    return type
                        .GetProperties()
                        .Where(a => a.CanWrite)
                        .ToDictionary(a => a.Name, a => (a.PropertyType, type.DelegateForSetPropertyValue(a.Name)));
                }
            }

            var props = properties[type.Name];

            if (props.Count == 0)
            {
                this.Log().Warn("Count equals 0: Possible issue");
            }

            if (mapper.Value == null)
            {
                throw new Exception($"Check the order of object creation. The IMapper needs to be created first");
            }

            TChange? currentValueAsChange = default;

            try
            {
                // N.B if a property is not mapped check that is included in IVIewModel
                currentValueAsChange = mapper.Value.Map<TChange>(currentValue);
            }
            catch (InvalidCastException ex)
            {
                throw new Exception($"Checked the the type {nameof(TChange)} has been included as a type to map when building the mapper profile", ex);
            }

            if (!comparer.Compare(newValue, currentValueAsChange, out var differences))
                if (differences.Where(a => !string.IsNullOrEmpty(a.Value1)).ToArray() is { } dd)
                    foreach (var difference in dd)
                    {
                        this.Log().Debug(difference);
                        var replace = difference.MemberPath.Replace(".Value", "");
                        var single = props.GetValueOrDefault(replace);
                        if (single == default || single.type == default)
                        {
                            continue;
                        }
                        try
                        {
                            var conversion = TypeConvert.Convert(difference.Value1, single.type);
                            single.setter(currentValue, conversion);
                        }
                        catch (Exception ex)
                        {
                            this.Log().Error("Can't convert" + ex.Message);
                        }
                        //(currentValue as INotifyPropertyChanged)?.PropertyChanged(replace);
                    }
        }

        public static PropertyMerger<TChange> Instance { get; } = new PropertyMerger<TChange>();
    }

    public class PropertyMerger : IEnableLogger
    {
        private class AutoMapperFactory
        {
            public static IMapper Create()
            {
                return new MapperConfiguration(cfg =>
                {
                    //  cfg.AddProfile(new MergeProfile());
                }).CreateMapper();
            }
        }

        private readonly Dictionary<string, Dictionary<string, (Type type, MemberSetter setter)>> properties = new();
        private readonly CompareLogic comparer = new();
        private readonly Lazy<IMapper> mapper = new(() => AutoMapperFactory.Create());
        //private readonly Lazy<IMapper> mapper = new(() => AutoMapperMapperFactory.Create());

        private PropertyMerger()
        {
        }

        public void Set(object currentValue, object newValue)
        {
            var type = currentValue.GetType();
            var newType = newValue.GetType();

            if (!properties.ContainsKey(type.Name))
            {
                properties[type.Name] = GetProperties(type);

                static Dictionary<string, (Type type, MemberSetter setter)> GetProperties(Type type)
                {
                    return type
                        .GetProperties()
                        .Where(a => a.CanWrite)
                        .ToDictionary(a => a.Name, a => (a.PropertyType, type.DelegateForSetPropertyValue(a.Name)));
                }
            }

            var props = properties[type.Name];

            if (props.Count == 0)
            {
                this.Log().Warn("Count equals 0: Possible issue");
            }

            if (mapper.Value == null)
            {
                throw new Exception($"Check the order of object creation. The IMapper needs to be created first");
            }

            object? currentValueAsChange = default;

            try
            {
                // N.B if a property is not mapped check that is included in IVIewModel
                currentValueAsChange = mapper.Value.Map(currentValue, type, newType);
            }
            catch (InvalidCastException ex)
            {
                throw new Exception($"Checked the the type {newType.Name} has been included as a type to map when building the mapper profile", ex);
            }

            //This is the comparison class
            CompareLogic compareLogic = new CompareLogic();

            if (comparer.Compare(newValue, currentValueAsChange) is { } result)

                foreach (var difference in result.Differences)
                {
                    this.Log().Debug(difference);
                    var single = props.GetValueOrDefault(difference.PropertyName);
                    if (single == default)
                    {
                        return;
                    }
                    try
                    {
                        var conversion = TypeConvert.Convert(difference.Object1, single.type);
                        single.setter(currentValue, conversion);
                    }
                    catch (Exception ex)
                    {
                        this.Log().Error("Can't convert" + ex.Message);
                    }
                    //(currentValue as ReactiveObject)?.RaisePropertyChanged(difference.PropertyName);
                }
        }

        public static PropertyMerger Instance { get; } = new PropertyMerger();
    }
}