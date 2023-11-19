using Castle.Core.Resource;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Helpers.NonGeneric;
using Utility.Nodes;
using Utility.Observables.Generic;
using Utility.PropertyTrees.Services;
using Utility.Trees;
using Utility.WPF.Helpers;

namespace Utility.PropertyTrees.WPF
{
    public class PropertyTemplateSelector : DataTemplateSelector, IObservable<SelectTemplateEvent>
    {
        private List<IObserver<SelectTemplateEvent>> observers = new();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var template = FindTemplate(item, container);
            this.broadcast(new(item, template, container));
            return template;
        }

        public DataTemplate FindTemplate(object item, DependencyObject container)
        {
            DataTemplate template = default;

            if (item is ViewModel viewmodel)
            {

            }

            if (item is MethodNode methodNode)
            {
                return MethodTemplate;
            }
            if (item is PropertyBase { DataTemplateKey: string key })
            {
                template = (DataTemplate)Application.Current.TryFindResource(key);
                if (template == null)
                {
                    template = UnknownTemplate;
                }
                return template;
            }

            //if (propertyType?.FullName?.Equals("System.RuntimeType") == true)
            //{
            //    this.broadcast(new(item, TypeTemplate, container));
            //    return TypeTemplate;
            //}
            if (item is ReferenceProperty { } propertyBase)
            {
                if ((container as FrameworkElement).TryFindResource(new DataTemplateKey(propertyBase.PropertyType)) is DataTemplate dataTemplate)
                {
                    return dataTemplate;
                }
                var collectionTemplate = HeaderTemplate(propertyBase.Ancestors().Count());
                return collectionTemplate;
            }

            if (item is RootProperty)
            {
                return RootTemplate;
            }
            if (item is not ValueProperty { Descriptor: { PropertyType: var propertyType } } propertyNode)
            {
                return UnknownTemplate;
            }
            if (propertyType == typeof(Guid))
            {
                return StringTemplate;
            }
            if (propertyType == typeof(DateTime))
            {
                return StringTemplate;
            }
            if (propertyType == typeof(string))
            {
                return StringTemplate;
            }
            if (propertyType == typeof(bool))
            {
                return BooleanTemplate;
            }
            if (propertyType == typeof(double))
            {
                return DoubleTemplate;
            }
            if (propertyType == typeof(int))
            {
                return IntegerTemplate;
            }
            if (propertyType.IsEnum)
            {

                return EnumTemplate;
            }
            //if (propertyType?.FullName?.Equals("System.RuntimeType") == true)
            //{
            //    this.broadcast(new(item, TypeTemplate, container));
            //    return TypeTemplate;
            //}


            return UnknownTemplate;
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

        public DataTemplate StringTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }
        public DataTemplate DoubleTemplate { get; set; }
        public DataTemplate IntegerTemplate { get; set; }
        public DataTemplate UnknownTemplate { get; set; }
        public DataTemplate EnumTemplate { get; set; }
        public DataTemplate ReferenceTemplate { get; set; }
        public DataTemplate RootTemplate { get; set; }
        public DataTemplate MethodTemplate { get; set; }
        //public DataTemplate TypeTemplate { get; set; }


        public static DataTemplate HeaderTemplate(int count) => TemplateGenerator.CreateDataTemplate(() =>
        {
            Binding binding = new(nameof(ValueNode.Name))
            {
                Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter
            };
            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, binding);
            textBlock.FontWeight = FontWeight.FromOpenTypeWeight(500 - count * 10);
            textBlock.FontSize = 32 - count * 1;
            return textBlock;
        });
    }

    public record SelectTemplateEvent(object Item, DataTemplate Template, DependencyObject Container);



}
