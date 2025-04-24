using Microsoft.Xaml.Behaviors;
using Newtonsoft.Json.Linq;
using Pather.CSharp;
using Pather.CSharp.PathElements;
using System;
using System.Collections.Generic;
using System.Windows;


namespace Utility.WPF.Controls.Objects
{
    public class ObjectBehavior : Behavior<JsonControl>
    {
        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register("Object", typeof(object), typeof(ObjectBehavior), new PropertyMetadata(null, changed));

        private ResolverFactory.Resolver resolver;

        static ObjectBehavior()
        {
        }

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ObjectBehavior b && e.NewValue is { } obj)
            {
                b.AssociatedObject.Object = JToken.FromObject(obj);
            }
        }

        protected override void OnAttached()
        {
            AssociatedObject.ValueChanged += AssociatedObject_ValueChanged;
            base.OnAttached();
        }

        public object Object
        {
            get { return (object)GetValue(ObjectProperty); }
            set { SetValue(ObjectProperty, value); }
        }

        protected void AssociatedObject_ValueChanged(object sender, ValueChangedRoutedEventArgs e)
        {
            resolver ??= ResolverFactory.Create();
            var _value = value();
            resolver.Set(Object, e.JPropertyNewValue.JProperty.Path, _value);

            object value()
            {
                if (e.JPropertyNewValue is { EnumType: { } type, JProperty: JProperty _jProperty })
                    return Enum.Parse(type, _jProperty.Value.Value<string>());
                else if (e.JPropertyNewValue.JProperty is JValue jValue)
                    return jValue.Value;
                else if (e.JPropertyNewValue.JProperty is JProperty { Value: JValue value })
                    return value.Value;
                else if (e.JPropertyNewValue.JProperty is JToken jToken)
                {
                    return ObjectConverter.ToObject(jToken);
                }

                throw new Exception("2 44");
            }
        }
    }

    public class ObjectConverter
    {
        public static object ToObject(JToken jObject)
        {
            if (jObject["$type"] is JToken token)
            {
                var type = Type.GetType(token.Value<string>());
                return jObject.ToObject(type);
            }
            return null;
        }
    }

    public class ResolverFactory
    {
        public static Resolver Create()
        {
            return new Resolver();
        }

        public class Resolver : IResolver
        {
            private readonly PathElementSplitter pathSplitter;

            public Resolver()
            {
                pathSplitter = new PathElementSplitter
                {
                    PathElementFactories = new List<IPathElementFactory>
                    {
                        new PropertyFactory(),
                        new EnumerableFactory(),
                        new ValuesEnumerableFactory(),
                        new DictionaryFactory(),
                        new SelectionFactory()
                    }
                };
            }

            public object Get(object target, string path)
            {
                return pathSplitter.Resolve(target, path);
            }
            public void Set(object target, string path, object value)
            {
                pathSplitter.Modify(target, path, value);
            }
        }
    }
}
