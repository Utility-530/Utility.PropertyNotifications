using Microsoft.Xaml.Behaviors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pather.CSharp;
using Pather.CSharp.PathElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Windows;

namespace Utility.WPF.Controls.Objects
{
    public class ObjectBehavior : Behavior<JsonControl>
    {
        private ResolverFactory.Resolver resolver;
        private ReplaySubject<JsonControl> _playSubject = new(1);
        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register("Object", typeof(object), typeof(ObjectBehavior), new PropertyMetadata(null, changed));
        public static readonly DependencyProperty JsonSerializerProperty = DependencyProperty.Register("JsonSerializer", typeof(JsonSerializer), typeof(ObjectBehavior), new PropertyMetadata());
        public static readonly DependencyProperty RaisePropertyChangedProperty = DependencyProperty.Register("RaisePropertyChanged", typeof(bool), typeof(ObjectBehavior), new PropertyMetadata());
        public static readonly DependencyProperty CollectionsProperty = DependencyProperty.Register("Collections", typeof(IEnumerable), typeof(ObjectBehavior), new PropertyMetadata(changed2));

        private static void changed2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ObjectBehavior @object && e.NewValue is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (item is INotifyCollectionChanged inpc)
                    {
                        inpc.CollectionChanged -= (s, ev) =>
                        {
                            @object._playSubject.OnNext(@object.AssociatedObject);
                        };
                        inpc.CollectionChanged += (s, ev) =>
                        {
                            Application.Current.Dispatcher.Invoke(() => {
                                @object._playSubject.OnNext(@object.AssociatedObject);
                            });

                        };
                    }
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == nameof(Collections))
            {

            }
            base.OnPropertyChanged(e);
        }
        static ObjectBehavior()
        {
            serialiser = JsonSerializer.CreateDefault(new JsonSerializerSettings { Converters = Statics.converters, TypeNameHandling = TypeNameHandling.All });
        }

        private static JsonSerializer serialiser;

        public JsonSerializer JsonSerializer
        {
            get { return (JsonSerializer)GetValue(JsonSerializerProperty); }
            set { SetValue(JsonSerializerProperty, value); }
        }

        public IEnumerable Collections
        {
            get { return (IEnumerable)GetValue(CollectionsProperty); }
            set { SetValue(CollectionsProperty, value); }
        }

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (d is ObjectBehavior b && e.NewValue is { } obj)
            {
                b._playSubject.Subscribe(a =>
                {
                    try
                    {
                        if (b.JsonSerializer != null)
                            a.Object = JToken.FromObject(obj, b.JsonSerializer);
                        else
                            a.Object = JToken.FromObject(obj, serialiser);
                    }
                    catch (Exception ex)
                    {
                        a.Object = JToken.FromObject(ex);
                    }
                });
            }
        }

        protected override void OnAttached()
        {
            AssociatedObject.ValueChanged += AssociatedObject_ValueChanged;
            _playSubject.OnNext(AssociatedObject);
            base.OnAttached();
        }

        public object Object
        {
            get { return (object)GetValue(ObjectProperty); }
            set { SetValue(ObjectProperty, value); }
        }

        public bool RaisePropertyChanged
        {
            get { return (bool)GetValue(RaisePropertyChangedProperty); }
            set { SetValue(RaisePropertyChangedProperty, value); }
        }

        protected void AssociatedObject_ValueChanged(object sender, ValueChangedRoutedEventArgs e)
        {
            resolver ??= ResolverFactory.Create(RaisePropertyChanged);
            var _value = value();
            string pattern = @"\[\'";
            string result = Regex.Replace(e.JPropertyNewValue.JProperty.Path, pattern, "");
            var path = Regex.Match(result, JsonControl.LabelPattern).Groups[0].ToString();
            resolver.Set(Object, path, _value);

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
        public static Resolver Create(bool raisePropertyChange = false)
        {
            return new Resolver(raisePropertyChange);
        }

        public class Resolver : IResolver
        {
            private readonly PathElementSplitter pathSplitter;

            public Resolver(bool raisePropertyChange = false)
            {
                pathSplitter = new PathElementSplitter
                {
                    PathElementFactories = new List<IPathElementFactory>
                    {
                        raisePropertyChange? new PropertyChangeFactory(): new PropertyFactory(),
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

    public class PropertyChangeFactory : PropertyFactory
    {
        public override IPathElement Create(string path, out string newPath)
        {
            string property = Regex.Matches(path, @"^\w+")[0].Value;
            newPath = path.Remove(0, property.Length);
            return new RaiseProperty(property);
        }

        public class RaiseProperty : Property
        {
            public RaiseProperty(string propertyName) : base(propertyName)
            {
            }

            public override void Apply(object target, object value)
            {
                var p = get(target);
                (p ?? throw new ArgumentException($"The property {property} could not be found.")).SetValue(target, value);
                if (target is INotifyPropertyChanged c)
                {
                    Utility.PropertyNotifications.PropertyChangedExtensions.RaisePropertyChanged(c, p.Name);
                }
            }

        }
    }
}
