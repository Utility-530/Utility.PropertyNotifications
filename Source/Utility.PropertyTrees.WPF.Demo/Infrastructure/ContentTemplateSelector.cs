using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utility.Observables.Generic;

namespace Utility.PropertyTrees.WPF.Demo
{
    internal class ContentTemplateSelector : DataTemplateSelector, IObservable<SelectTemplateEvent>
    {
        private List<IObserver<SelectTemplateEvent>> observers = new();        

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not ValueProperty { Descriptor: { PropertyType: var propertyType } } propertyNode)
            {
                Broadcast(new(item, UnknownTemplate, container));
                return UnknownTemplate;
            }
            if (propertyType == typeof(string))
            {
                Broadcast(new(item, StringTemplate, container));
                return StringTemplate;
            }
            if (propertyType == typeof(bool))
            {
                Broadcast(new(item, BooleanTemplate, container));
                return BooleanTemplate;
            }
            if (propertyType == typeof(double))
            {
                Broadcast(new(item, DoubleTemplate, container));
                return DoubleTemplate;
            }
            if (propertyType == typeof(int))
            {
                Broadcast(new(item, IntegerTemplate, container));
                return IntegerTemplate;
            }

            return base.SelectTemplate(item, container);
        }

        private void Broadcast(SelectTemplateEvent selectTemplateEvent)
        {
            foreach(var observer in observers)
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
    }

    public record SelectTemplateEvent(object Item, DataTemplate Template, DependencyObject Container);
}
