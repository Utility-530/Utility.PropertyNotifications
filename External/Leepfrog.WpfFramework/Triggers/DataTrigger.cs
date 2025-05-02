using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Leepfrog.WpfFramework.Triggers
{
    // Custom DataTrigger, also evaluates binding on load instead of just on change
    public class DataTrigger : Microsoft.Xaml.Behaviors.Core.DataTrigger
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            var element = AssociatedObject as FrameworkElement;
            if (element != null)
            {
                element.Loaded += OnElementLoaded;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            var element = AssociatedObject as FrameworkElement;
            if (element != null)
            {
                element.Loaded -= OnElementLoaded;
            }
        }

        private void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            EvaluateBindingChange(null);
        }

        private bool _ignoreDuplicates;
        public bool IgnoreDuplicates { get { return _ignoreDuplicates; } set { _ignoreDuplicates = value; } }

        private object _prev = null;

        private string _debug;
        public string Debug { get { return _debug; } set { _debug = value; } }

        protected override void EvaluateBindingChange(object args)
        {
            try
            {
                // added ml 2015-10-06
                // only trigger if binding value has actually changed, not every time propertychanged is raised!
                // also handle nulls - if binding is now null, then check prev value was also null
                if (_ignoreDuplicates)
                {
                    if (Binding?.Equals(_prev) ?? (_prev == null))
                    {
                        return;
                    }
                    _prev = Binding;
                }
                base.EvaluateBindingChange(args);
            }
            catch (Exception ex)
            {
                this.AddLog(ex);
                this.AddLog("TRIGGER", $"FAILED: {Debug} {ex.Message}");
                //throw ;
            }
        }
        
    }
}

