using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using Utility.Helpers.Reflection;

namespace Utility.WPF.Controls.Lists
{
    public class ItemsSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable enumerable)
            {
                if (enumerable.Count() > 0)
                {
                    return create(enumerable.CommonBaseClass());///.InnerType();
                }
                else if (convert(enumerable.GetType()) is { } _object)
                {
                    return _object;
                }
                else if (value is ICollectionView { SourceCollection: { } sourceCollection })
                {
                    return convert(sourceCollection.GetType());
                }
                else
                {
                    throw new NotImplementedException("dv33 322lk2");
                }
            }
            return DependencyProperty.UnsetValue;

            ObjectWrapper convert(Type type)
            {
                if (TypeHelper.GenericTypeArguments(type).SingleOrDefault() is Type _type)
                {
                    return create(_type);
                }
                return null;
            }
        }

        private static ObjectWrapper create(Type _type)
        {
            return new ObjectWrapper { Object = ActivateAnything.Activate.New(_type) };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Unable to bind directly so have to to create a new object with property, Object, to bind to instead
    /// </summary>
    public class ObjectWrapperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is { } enumerable)
            {
                return new ObjectWrapper { Object = value };///.InnerType();
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullTo40Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return new Thickness(0);
            }
            return new Thickness(0, 0, 40, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Necessary for XAML binding which require a property
    /// </summary>
    public class ObjectWrapper
    {
        private object @object;
        static readonly Dictionary<Type, IMapper> mappers = [];

        public object Object
        {
            get => @object; set
            {
                if (@object == null)
                {
                    @object = value;
                    return;
                }

                map(@object, value, @object.GetType());
            }
        }

        static void map(object oldValue, object newValue, Type type)
        {
            mappers.Get(type, (a) =>
            {
                return new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap(type, type);
                }).CreateMapper();
            }).Map(newValue, oldValue);
        }
    }
}

