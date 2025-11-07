using System;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using Utility.WPF.Controls.Dragablz;

namespace Utility.WPF.Controls.Hybrid
{
    public class MasterButtonsControl : MasterBindableControl
    {
        public static readonly DependencyProperty CommandPathProperty =
         DependencyProperty.Register("CommandPath", typeof(string), typeof(MasterButtonsControl), new PropertyMetadata(null));

        static MasterButtonsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterButtonsControl), new FrameworkPropertyMetadata(typeof(MasterButtonsControl)));
        }

        public MasterButtonsControl()
        {
            Movement = Enums.XYTraversal.TopToBottom;
        }

        #region properties

        public string CommandPath
        {
            get { return (string)GetValue(CommandPathProperty); }
            set { SetValue(CommandPathProperty, value); }
        }

        #endregion properties

        public override void OnApplyTemplate()
        {
            Content = new ButtonsControl
            {
                CommandPath = CommandPath,
                DisplayMemberPath = DisplayMemberPath,
                ItemsSource = ItemsSource
            };

            this.WhenAnyValue(a => a.ItemsSource)
                .WhereNotNull()
                .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath), this.WhenAnyValue(a => a.CommandPath))
                .Subscribe(tuple =>
                {
                    var (i, d, c) = tuple;
                    Dispatcher.InvokeAsync(() =>
                    {
                        var msn = Content switch
                        {
                            null => new ButtonsControl(),
                            ButtonsControl a => a,
                            _ => throw new ApplicationException("Expected Content to be MasterNotesItemsControl")
                        };
                        msn.CommandPath = c;
                        msn.ItemsSource = i;
                        msn.DisplayMemberPath = d;
                        //DoubleAnimation oLabelAngleAnimation    = new DoubleAnimation();
                        //oLabelAngleAnimation.From = 0;
                        //oLabelAngleAnimation.To = this?.ActualHeight??0;
                        //oLabelAngleAnimation.Duration                    = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                        //oLabelAngleAnimation.RepeatBehavior = new RepeatBehavior(4);
                        //this.BeginAnimation(MasterBindableControl.HeightProperty,                    oLabelAngleAnimation);
                    }, System.Windows.Threading.DispatcherPriority.Background);
                });

            base.OnApplyTemplate();
        }
    }
}