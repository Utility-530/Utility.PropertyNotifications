using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Factorys
{
    /// <summary>
    /// Class that helps the creation of control and data templates by using delegates.
    /// </summary>
    /// <remarks>
    /// Paulo Zemek
    /// <a href="https://stackoverflow.com/questions/5471405/create-datatemplate-in-code-behind"></a>
    /// </remarks>
    public static class TemplateGenerator
    {
        private sealed class TemplateGeneratorControl : ContentControl
        {
            internal static readonly DependencyProperty FactoryProperty = DependencyProperty.Register("Factory", typeof(Func<object>), typeof(TemplateGeneratorControl), new PropertyMetadata(null, FactoryChanged));

            private static void FactoryChanged(DependencyObject instance, DependencyPropertyChangedEventArgs args)
            {
                var control = (TemplateGeneratorControl)instance;
                var factory = (Func<object>)args.NewValue;
                if (factory != null)
                    control.Content = factory();
            }
        }

        /// <summary>
        /// Creates a data-template that uses the given delegate to create new instances.
        /// </summary>
        public static DataTemplate CreateDataTemplate(Func<FrameworkElement> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var frameworkElementFactory = new FrameworkElementFactory(typeof(TemplateGeneratorControl));
            frameworkElementFactory.SetValue(TemplateGeneratorControl.FactoryProperty, factory);

            DataTemplate dataTemplate = new(typeof(DependencyObject))
            {
                VisualTree = frameworkElementFactory
            };
            return dataTemplate;
        }

        public static HierarchicalDataTemplate CreateHierarcialDataTemplate(Func<FrameworkElement> factory, string childPropertyName) =>
            CreateHierarcialDataTemplate(factory, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(childPropertyName) });

        public static HierarchicalDataTemplate CreateHierarcialDataTemplate(Func<object> factory, BindingBase itemsSourceBinding)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var frameworkElementFactory = new FrameworkElementFactory(typeof(TemplateGeneratorControl));
            frameworkElementFactory.SetValue(TemplateGeneratorControl.FactoryProperty, factory);

            HierarchicalDataTemplate dataTemplate = new(typeof(DependencyObject))
            {
                VisualTree = frameworkElementFactory
            };
            dataTemplate.ItemsSource = itemsSourceBinding;
            return dataTemplate;
        }

        /// <summary>
        /// Creates a items-panel-template
        /// </summary>
        public static ItemsPanelTemplate CreateItemsPanelTemplate<TFrameworkElement>(Action<FrameworkElementFactory>? action = null)
            where TFrameworkElement : FrameworkElement
        {
            return CreateItemsPanelTemplate(typeof(TFrameworkElement), action);
        }

        /// <summary>
        /// Creates a items-panel-template
        /// </summary>
        public static ItemsPanelTemplate CreateItemsPanelTemplate(Type type, Action<FrameworkElementFactory>? action = null)
        {
            ItemsPanelTemplate itemsPanelTemplate = new()
            {
                VisualTree = new FrameworkElementFactory(type)
            };
            action?.Invoke(itemsPanelTemplate.VisualTree);
            return itemsPanelTemplate;
        }

        /// <summary>
        /// Creates a control-template
        /// </summary>
        public static ControlTemplate CreateControlTemplate<TFrameworkElement>(Func<object> factory)
            where TFrameworkElement : FrameworkElement
        {
            return CreateControlTemplate(factory, typeof(TFrameworkElement));
        }

        /// <summary>
        /// Creates a control-template
        /// </summary>
        public static ControlTemplate CreateControlTemplate(Func<object> factory, Type controlType)
        {
            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var frameworkElementFactory = new FrameworkElementFactory(typeof(TemplateGeneratorControl));
            frameworkElementFactory.SetValue(TemplateGeneratorControl.FactoryProperty, factory);

            ControlTemplate controlTemplate = new(controlType)
            {
                VisualTree = frameworkElementFactory
            };
            return controlTemplate;
        }
    }
}