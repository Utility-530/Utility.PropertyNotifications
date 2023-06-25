using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utility.Observables.Generic;

namespace Utility.PropertyTrees.WPF
{
    public class ContentTemplateSelector : DataTemplateSelector, IObservable<SelectTemplateEvent>
    {
        private List<IObserver<SelectTemplateEvent>> observers = new();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate template = default;

            if(item is MethodNode methodNode)
            {
                this.broadcast(new(item, MethodTemplate, container));
                return MethodTemplate;
            }
            if (item is PropertyBase { DataTemplateKey: string key })
            {
                template = (DataTemplate)Application.Current.TryFindResource(key);
                if (template == null)
                {
                    template = UnknownTemplate;
                }

                this.broadcast(new(item, template, container));
                return template;
            }

            if (item is PropertyBase { IsObservableCollection: true })
            {

                this.broadcast(new(item, CollectionTemplate, container));
                return CollectionTemplate;
            }
            if (item is PropertyBase { IsCollection: true })
            {

                this.broadcast(new(item, CollectionTemplate, container));
                return CollectionTemplate;
            }
            if (item is RootProperty)
            {
                this.broadcast(new(item, ReferenceTemplate, container));
                return RootTemplate;
            }
            if (item is ReferenceProperty)
            {
                this.broadcast(new(item, ReferenceTemplate, container));
                return ReferenceTemplate;
            }
            if (item is not ValueProperty { Descriptor: { PropertyType: var propertyType } } propertyNode)
            {
                this.broadcast(new(item, UnknownTemplate, container));
                return UnknownTemplate;
            }
            if (propertyType == typeof(Guid))
            {
                this.broadcast(new(item, StringTemplate, container));
                return StringTemplate;
            }
            if (propertyType == typeof(string))
            {
                this.broadcast(new(item, StringTemplate, container));
                return StringTemplate;
            }
            if (propertyType == typeof(bool))
            {
                this.broadcast(new(item, BooleanTemplate, container));
                return BooleanTemplate;
            }
            if (propertyType == typeof(double))
            {
                this.broadcast(new(item, DoubleTemplate, container));
                return DoubleTemplate;
            }
            if (propertyType == typeof(int))
            {
                this.broadcast(new(item, IntegerTemplate, container));
                return IntegerTemplate;
            }
            if (propertyType.IsEnum)
            {
                this.broadcast(new(item, EnumTemplate, container));
                return EnumTemplate;
            }


            template = UnknownTemplate;
            this.broadcast(new(item, template, container));
            return template;
            //return base.SelectTemplate(item, container);
        }

        private void broadcast(SelectTemplateEvent selectTemplateEvent)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(selectTemplateEvent);
            }
        }

        public IDisposable Subscribe(IObserver<SelectTemplateEvent> observer)
        {
            return new Disposer<SelectTemplateEvent>(observers, observer);
        }

        //public static ContentTemplateSelector Instance { get; } = new ContentTemplateSelector();

        public DataTemplate StringTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }
        public DataTemplate DoubleTemplate { get; set; }
        public DataTemplate IntegerTemplate { get; set; }
        public DataTemplate UnknownTemplate { get; set; }
        public DataTemplate CollectionTemplate { get; set; }
        public DataTemplate EnumTemplate { get; set; }
        public DataTemplate ReferenceTemplate { get; set; }
        public DataTemplate RootTemplate { get; set; }
        public DataTemplate MethodTemplate { get; set; }
    }

    public record SelectTemplateEvent(object Item, DataTemplate Template, DependencyObject Container);
}
