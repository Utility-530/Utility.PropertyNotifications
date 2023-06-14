using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Utility.WPF.Controls.Base;
using Utility.WPF.Controls.Lists.Infrastructure;
using Utility.WPF.Helpers;
using Utility.WPF.Abstract;

namespace Utility.WPF.Controls.Lists
{
    public class CheckBoxesComboControl : ComboBox<CheckBox>, IIsCheckedPath, IOutput<CheckedRoutedEventArgs>, IIsSelectedPath
    {
        private DifferenceHelper differenceHelper;
        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Output), typeof(object), typeof(CheckBoxesComboControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(CheckBoxesComboControl), new PropertyMetadata(null));
        //public static readonly DependencyProperty OutputProperty = DependencyProperty.RegisterReadOnly("Output", typeof(object), typeof(CheckBoxesComboControl));
        public static readonly RoutedEvent OutputChangeEvent = EventManager.RegisterRoutedEvent("OutputChange", RoutingStrategy.Bubble, typeof(OutputChangedEventHandler<CheckedRoutedEventArgs>), typeof(CheckBoxesComboControl));
        public static readonly DependencyProperty IsSelectedPathProperty = DependencyProperty.Register("IsSelectedPath", typeof(string), typeof(CheckBoxesComboControl));
        public static readonly DependencyProperty IsDisabledShownProperty = DependencyProperty.Register("IsDisabledShown", typeof(bool), typeof(CheckBoxesComboControl));
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;
        public static readonly DependencyProperty FilterCollectionProperty = DependencyProperty.Register("FilterCollection", typeof(IEnumerable), typeof(CheckBoxesComboControl));

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