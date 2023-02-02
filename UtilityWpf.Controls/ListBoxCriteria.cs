#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Controls.Base;

namespace UtilityWpf.Controls
{
    public class ListBoxCriteria : ListBox<CriteriaItem>
    {
        public static readonly DependencyProperty IsCriteriaMetProperty = DependencyProperty.Register("IsCriteriaMet", typeof(bool), typeof(ListBoxCriteria), new PropertyMetadata(false, MetChanged));
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(ListBoxCriteria), new PropertyMetadata(null, PropertyNameChanged));
        public static readonly RoutedEvent CriteriaMetEvent = EventManager.RegisterRoutedEvent("CriteriaMet", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CriteriaItem));

        public ListBoxCriteria() : base()
        {
        }

        #region properties

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        private static void PropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public bool IsCriteriaMet
        {
            get { return (bool)GetValue(IsCriteriaMetProperty); }
            set { SetValue(IsCriteriaMetProperty, value); }
        }

        private static void MetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public event RoutedEventHandler CriteriaMet
        {
            add { AddHandler(CriteriaMetEvent, value); }
            remove { RemoveHandler(CriteriaMetEvent, value); }
        }

        #endregion properties

        protected override CriteriaItem InitialiseItem(CriteriaItem criteriaItem, object viewmodel)
        {
            Binding myBinding = new (PropertyName)
            {
                Source = viewmodel
            };
            criteriaItem.SetBinding(CriteriaItem.MeetsCriteriaProperty, myBinding);
            criteriaItem.CriteriaChanged += CriteriaItem_CriteriaChanged;
            CriteriaItem_CriteriaChanged(this, default);
            return criteriaItem;
        }

        private void CriteriaItem_CriteriaChanged(object sender, RoutedEventArgs? e)
        {
            List<int> indices = new();
            List<object> metItems = new();
            List<object> missedItems = new();

            for (int i = this.Items.Count - 1; i >= 0; i--)
            {
                if (Get(this, i) is CriteriaItem criteriaItem)
                {
                    if (criteriaItem.MeetsCriteria)
                    {
                        indices.Add(i);
                        metItems.Add(criteriaItem.Content);
                    }
                    else
                    {
                        missedItems.Add(criteriaItem.Content);
                    }
                }
            }
            if (indices.Count > 0)
            {
                IsCriteriaMet = true;
            }

            RaiseEvent(new CriteriaMetEventArgs(CriteriaMetEvent, metItems, missedItems, indices.ToArray()));

            static CriteriaItem? Get(ItemsControl itemCollection, int index)
            {
                if (itemCollection.Items[index] is CriteriaItem criteriaItem)
                    return criteriaItem;
                return itemCollection.ItemContainerGenerator.ContainerFromIndex(index) as CriteriaItem;
            }
        }

        public class CriteriaMetEventArgs : RoutedEventArgs
        {
            public int[] Indices { get; }

            public IEnumerable Met { get; }
            public IEnumerable Missed { get; }

            public CriteriaMetEventArgs(RoutedEvent @event, IEnumerable met, IEnumerable missed, int[] indices) : base(@event)
            {
                this.Met = met;
                this.Missed = missed;
                Indices = indices;
            }
        }
    }

    public class CriteriaItem : ListBoxItem
    {
        public static readonly DependencyProperty MeetsCriteriaProperty = DependencyProperty.Register("MeetsCriteria", typeof(bool), typeof(CriteriaItem), new FrameworkPropertyMetadata(false, MeetsCriteriaChanged));
        public static readonly RoutedEvent CriteriaChangedEvent = EventManager.RegisterRoutedEvent("CriteriaChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CriteriaItem));

        static CriteriaItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CriteriaItem), new FrameworkPropertyMetadata(typeof(CriteriaItem)));
        }

        public CriteriaItem()
        {
            //this.Loaded += CriteriaItem_Loaded;
        }

        //private void CriteriaItem_Loaded(object sender, RoutedEventArgs e)
        //{
        //}

        #region properties

        public bool MeetsCriteria
        {
            get { return (bool)GetValue(MeetsCriteriaProperty); }
            set { SetValue(MeetsCriteriaProperty, value); }
        }

        public event RoutedEventHandler CriteriaChanged
        {
            add { AddHandler(CriteriaChangedEvent, value); }
            remove { RemoveHandler(CriteriaChangedEvent, value); }
        }

        #endregion properties

        private static void MeetsCriteriaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CriteriaItem)?.RaiseEvent(new CriteriaChangedEventArgs(CriteriaItem.CriteriaChangedEvent) { CriteriaIsMet = (bool)e.NewValue });
        }

        public class CriteriaChangedEventArgs : RoutedEventArgs
        {
            public bool CriteriaIsMet;

            public CriteriaChangedEventArgs(RoutedEvent @event) : base(@event)
            { }
        }
    }
}