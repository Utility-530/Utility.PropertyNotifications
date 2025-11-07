using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Abstract;
using Utility.WPF.Controls.Base;
using Utility.WPF.Controls.Lists.Infrastructure;
using Utility.WPF.Models;
using Utility.WPF.Templates;

namespace Utility.WPF.Controls.Lists
{
    public class CheckBoxesListControl : ListBox<CheckBox>, IIsCheckedPath, IOutput<CheckedRoutedEventArgs>
    {
        private DifferenceHelper differenceHelper;

        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Output), typeof(object), typeof(CheckBoxesListControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(CheckBoxesListControl), new PropertyMetadata(null));
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;
        public static readonly RoutedEvent OutputChangeEvent = EventManager.RegisterRoutedEvent("OutputChange", RoutingStrategy.Bubble, typeof(OutputChangedEventHandler<CheckedRoutedEventArgs>), typeof(CheckBoxesListControl));

        static CheckBoxesListControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxesListControl), new FrameworkPropertyMetadata(typeof(CheckBoxesListControl)));
        }

        public CheckBoxesListControl()
        {
            Loaded += OnChange;
        }

        public override void OnApplyTemplate()
        {
            if (string.IsNullOrEmpty(DisplayMemberPath) && ItemTemplateSelector == null)
                ItemTemplateSelector = CustomDataTemplateSelector.Instance;
            base.OnApplyTemplate();
            //listBox = this.GetTemplateChild("CalendarDaysListBox") as ContentPresenter;
        }

        #region properties

        public event OutputChangedEventHandler<CheckedRoutedEventArgs> OutputChange
        {
            add => AddHandler(OutputChangeEvent, value);
            remove => RemoveHandler(OutputChangeEvent, value);
        }

        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }

        public object Output
        {
            get => GetValue(OutputProperty);
            protected set => SetValue(OutputPropertyKey, value);
        }

        #endregion properties

        protected override void PrepareContainerForItemOverride(CheckBox element, object item)
        {
            if (string.IsNullOrEmpty(DisplayMemberPath) && element.ContentTemplateSelector == null)
                element.ContentTemplateSelector = CustomDataTemplateSelector.Instance;
            CheckBoxesHelper.Bind(element, item, this);
            element.Checked += OnChange;
            element.Unchecked += OnChange;
        }

        protected virtual void OnChange(object sender, RoutedEventArgs eventArgs)
        {
            var dictionary = (differenceHelper ??= new DifferenceHelper(this)).Get;
            Output = dictionary;
            RaiseEvent(new CheckedRoutedEventArgs(OutputChangeEvent, this, dictionary.ToArray()));
        }
    }
}