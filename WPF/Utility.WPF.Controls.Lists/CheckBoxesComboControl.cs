using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Evan.Wpf;
using Utility.WPF.Abstract;
using Utility.WPF.Controls.Base;
using Utility.WPF.Controls.Lists.Infrastructure;
using Utility.WPF.Models;

namespace Utility.WPF.Controls.Lists
{
    public class CheckBoxesComboControl : ComboBox<CheckBox>, IIsCheckedPath, IOutput<CheckedRoutedEventArgs>, IIsSelectedPath
    {
        private DifferenceHelper differenceHelper;
        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyHelper.RegisterReadOnly();
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyHelper.Register();

        //public static readonly DependencyProperty OutputProperty = DependencyProperty.RegisterReadOnly("Output", typeof(object), typeof(CheckBoxesComboControl));
        public static readonly RoutedEvent OutputChangeEvent = EventManager.RegisterRoutedEvent("OutputChange", RoutingStrategy.Bubble, typeof(OutputChangedEventHandler<CheckedRoutedEventArgs>), typeof(CheckBoxesComboControl));

        public static readonly DependencyProperty IsSelectedPathProperty = DependencyHelper.Register();
        public static readonly DependencyProperty KeyPathProperty = DependencyHelper.Register();
        public static readonly DependencyProperty IsDisabledShownProperty = DependencyHelper.Register();
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;
        public static readonly DependencyProperty FilterCollectionProperty = DependencyHelper.Register();

        static CheckBoxesComboControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxesComboControl), new FrameworkPropertyMetadata(typeof(CheckBoxesComboControl)));
        }

        public CheckBoxesComboControl()
        {
            Loaded += OnChange;
            AddHandler(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, new RoutedEventHandler(CloseButtonClick), true);
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            OnChange(sender, e);
        }

        #region properties

        public IEnumerable FilterCollection
        {
            get { return (IEnumerable)GetValue(FilterCollectionProperty); }
            set { SetValue(FilterCollectionProperty, value); }
        }

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

        public string KeyPath
        {
            get => (string)GetValue(KeyPathProperty);
            set => SetValue(KeyPathProperty, value);
        }

        public object Output
        {
            get => GetValue(OutputProperty);
            protected set => SetValue(OutputPropertyKey, value);
        }

        public string IsSelectedPath
        {
            get { return (string)GetValue(IsSelectedPathProperty); }
            set { SetValue(IsSelectedPathProperty, value); }
        }

        public bool IsDisabledShown
        {
            get { return (bool)GetValue(IsDisabledShownProperty); }
            set { SetValue(IsDisabledShownProperty, value); }
        }

        #endregion properties

        protected override void PrepareContainerForItemOverride(CheckBox element, object item)
        {
            CheckBoxesHelper.Bind(element, item, this, KeyPathProperty);
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