using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;


namespace Leepfrog.WpfFramework.Triggers
{
    // Custom DataTrigger, specifically for selecteditem of tabcontrol
    public class TabSelectedTrigger : DataTrigger
    {

        public TabSelectedTrigger()
        {
            // just setup a dummy binding to point to the tab property's parent's selected item
            {
                var binding = new Binding("Tab.Parent.SelectedItem");
                binding.Source = this;
                BindingOperations.SetBinding(this,BindingProperty,binding);
            }

            // set value to point to Tab
            {
                var binding = new Binding("Tab");
                binding.Source = this;
                BindingOperations.SetBinding(this,ValueProperty,binding);
            }

        }

        public TabItem Tab
        {
            get { return (TabItem)GetValue(TabProperty); }
            set { SetValue(TabProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tab.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabProperty =
            DependencyProperty.Register("Tab", typeof(TabItem), typeof(TabSelectedTrigger), new PropertyMetadata(null));



        protected override void OnAttached()
        {
            base.OnAttached();
            var element = AssociatedObject as FrameworkElement;
            // if we're attached to a tabitem...
            if (element is TabItem tabItem)
            {
                // just take the associated element as our tab!
                var binding = BindingOperations.GetBinding(this, TabProperty);
                if (binding == null)
                {
                    Tab = tabItem;
                }
            }
        }

    }
}

