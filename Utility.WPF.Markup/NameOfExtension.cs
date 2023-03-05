using Hardcodet.Wpf.DataBinding;
using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace Utility.WPF.Markup
{
    [ContentProperty(nameof(Path))]
    public class NameOfExtension : BindingDecoratorBase
    {
        public NameOfExtension()
        {
        }


        public static readonly DependencyProperty TypeProperty = DependencyProperty.RegisterAttached("Type", typeof(Type), typeof(NameOfExtension), new PropertyMetadata(null, PropertyChanged));
        
        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //.singleReplaySubject.OnNext(e.NewValue as Type);
        }

        public static Type GetType(DependencyObject d)
        {
            return (Type)d.GetValue(TypeProperty);
        }

        public static void SetType(DependencyObject d, Type value)
        {
            d.SetValue(TypeProperty, value);
        }

        public Type? Type { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Type??= GetType(serviceProvider);

            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (Type == null)
                throw new ArgumentNullException(nameof(Type));
            if (Path == null)
                throw new ArgumentNullException(nameof(Path));

            if (Type == null || string.IsNullOrEmpty(Path.Path) || Path.Path.Contains('.'))
                throw new ArgumentException("Syntax for x:NameOf is Type={x:Type [className]} Member=[propertyName]");

            var propertyInfo = Type.GetRuntimeProperties().FirstOrDefault(pi => pi.Name == Path.Path);
            if (propertyInfo != null)
                return Path.Path;
            var fieldInfo = Type.GetRuntimeFields().FirstOrDefault(fi => fi.Name == Path.Path);
            if (fieldInfo != null)
                return Path.Path;
            var eventInfo = Type.GetRuntimeEvents().FirstOrDefault(ei => ei.Name == Path.Path);
            if (eventInfo == null)
                throw new ArgumentException($"No property or field found for {Path.Path} in {Type}");

            return Path.Path;
        }

        private Type? GetType(IServiceProvider serviceProvider)
        {
            if(TryGetTargetItems(serviceProvider, out var targetObject, out var _))
            {
                return NameOfExtension.GetType(targetObject);
            }
            throw new Exception("gre");
        }
    }
}