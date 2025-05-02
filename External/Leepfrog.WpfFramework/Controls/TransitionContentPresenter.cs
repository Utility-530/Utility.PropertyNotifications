using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Leepfrog.WpfFramework.Controls
{
    [TemplatePartAttribute(Name = "PART_ContentHost", Type = typeof(ContentPresenter))]
    [TemplatePartAttribute(Name = "PART_StaleContentHost", Type = typeof(ContentPresenter))]
    public class TransitionContentPresenter : ContentControl
    {

        public static readonly DependencyProperty TransitionProperty;
        public static readonly DependencyProperty StaleContentProperty;
        public static readonly RoutedEvent ContentChangedEvent;

        public event RoutedEventHandler ContentChanged
        {
            add { AddHandler(ContentChangedEvent, value); }
            remove { RemoveHandler(ContentChangedEvent, value); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Object StaleContent
        {
            get { return base.GetValue(StaleContentProperty); }
            set { base.SetValue(StaleContentProperty, value); }
        }

        public Storyboard Transition
        {
            get { return (Storyboard)base.GetValue(TransitionProperty); }
            set { base.SetValue(TransitionProperty, value); }
        }


        static TransitionContentPresenter()
        {
            // TODO: MAYBE CHANGE THIS SO THAT THE VIEWS REMAIN LOADED... SOMEHOW!
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitionContentPresenter), new FrameworkPropertyMetadata(typeof(TransitionContentPresenter)));
            StaleContentProperty = DependencyProperty.Register(
                        "StaleContent",
                        typeof(Object),
                        typeof(TransitionContentPresenter),
                        new FrameworkPropertyMetadata(null)
                        );

            TransitionProperty = DependencyProperty.Register(
                        "Transition",
                        typeof(Storyboard),
                        typeof(TransitionContentPresenter),
                        new FrameworkPropertyMetadata(new Storyboard())
                        );

            ContentControl.ContentProperty.OverrideMetadata(
                typeof(TransitionContentPresenter),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentChanged)));

            ContentChangedEvent = EventManager.RegisterRoutedEvent("ContentChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TransitionContentPresenter));

        }

        private static void OnContentChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            TransitionContentPresenter tc = element as TransitionContentPresenter;
            tc.StaleContent = e.OldValue;
            //if (e.OldValue != null && e.NewValue != null)
            {
                if (!tc.IsLoaded)
                {
                    tc.Loaded += onLoaded;
                }
                else
                {
                    tc.raiseContentChanged();
                }
            }
        }

        private void clearStaleContent()
        {
            StaleContent = null;
        }

        static void onLoaded(object sender, RoutedEventArgs e)
        {
            TransitionContentPresenter tc = e.Source as TransitionContentPresenter;
            tc.Loaded -= onLoaded;
            tc.raiseContentChanged();
        }

        private Storyboard _sb;

        private void raiseContentChanged()
        {
            if (_sb != null)
            {
                _sb.Completed -= sb_Completed;
                _sb.Stop();
                _sb = null;
            }
            //this.AddLog("TCC", "a");
            RaiseEvent(new RoutedEventArgs(TransitionContentPresenter.ContentChangedEvent));
            //this.AddLog("TCC", "b");
            _sb = ((Storyboard)this.GetValue(TransitionProperty)).Clone();
            //this.AddLog("TCC", "c");
            _sb.Completed += sb_Completed;
            //this.AddLog("TCC", "d");
            _sb.Begin((Grid)this.GetTemplateChild("PART_root"));
            //this.AddLog("TCC", "e");
        }

        void sb_Completed(object sender, EventArgs e)
        {
            _sb.Completed -= sb_Completed;
            _sb = null;
            clearStaleContent();
        }

    }
}
