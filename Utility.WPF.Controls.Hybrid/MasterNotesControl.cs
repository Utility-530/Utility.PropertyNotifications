using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Controls.Dragablz;

namespace Utility.WPF.Controls.Hybrid
{
    public class MasterNotesControl : MasterBindableControl
    {
        static MasterNotesControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterNotesControl), new FrameworkPropertyMetadata(typeof(MasterNotesControl)));
        }

        public MasterNotesControl()
        {
            Position = Dock.Bottom;
        }

        public override void OnApplyTemplate()
        {
            Content = new NotesControl
            {
                DisplayMemberPath = DisplayMemberPath,
                ItemsSource = ItemsSource
            };

            this.WhenAnyValue(a => a.ItemsSource)
                .WhereNotNull()
                .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath))
                .Subscribe(a =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        switch (Content)
                        {
                            case null:
                                Content = new NotesControl
                                {
                                    DisplayMemberPath = DisplayMemberPath,
                                    ItemsSource = ItemsSource
                                };
                                break;
                            case NotesControl msn:
                                msn.ItemsSource = ItemsSource;
                                msn.DisplayMemberPath = DisplayMemberPath;
                                break;
                            default:
                                throw new ApplicationException("Expected Content to be MasterNotesItemsControl");
                        }

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