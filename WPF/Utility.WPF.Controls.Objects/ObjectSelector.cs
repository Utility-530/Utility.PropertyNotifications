using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;

namespace Utility.WPF.Controls.Objects
{
    public class ObjectSelector : ObjectItemsControl
    {
        private ObjectItemsControl? itemsControl;

        static ObjectSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ObjectSelector), new FrameworkPropertyMetadata(typeof(ObjectSelector)));
        }

        public ObjectSelector()
        {
        }

        public override void OnApplyTemplate()
        {
            itemsControl = GetTemplateChild("ObjectItemsControl") as ObjectItemsControl;

            itemsControl.WhenAnyValue(a => a.Value)
                .WhereNotNull()
                .Subscribe(a =>
                {
                    Value = a;
                });
            this.WhenAnyValue(a => a.Object)
                .WhereNotNull()
                .Subscribe(a =>
                {
                    itemsControl.Object = a;
                });
            base.OnApplyTemplate();
        }
    }
}