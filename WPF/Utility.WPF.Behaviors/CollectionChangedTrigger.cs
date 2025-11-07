using System.Collections.Specialized;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Behaviors
{
    public class CollectionChangedTrigger : TriggerBase<FrameworkElement>
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                nameof(Source),
                typeof(INotifyCollectionChanged),
                typeof(CollectionChangedTrigger),
                new PropertyMetadata(OnSourceChanged));

        public INotifyCollectionChanged Source
        {
            get => (INotifyCollectionChanged)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CollectionChangedTrigger trigger)
            {
                if (e.OldValue is INotifyCollectionChanged oldColl)
                    oldColl.CollectionChanged -= trigger.OnCollectionChanged;

                if (e.NewValue is INotifyCollectionChanged newColl)
                    newColl.CollectionChanged += trigger.OnCollectionChanged;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == NotifyCollectionChangedAction.Add)
            InvokeActions(e);
        }
    }
}