using Jellyfish;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Utility.Commands;
using Utility.WPF.Helpers;

namespace Utility.WPF.Adorners
{
    public class MaskBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(MaskBehavior), new PropertyMetadata());
        public static readonly DependencyProperty MaskCommandProperty = DependencyProperty.Register("MaskCommand", typeof(ICommand), typeof(MaskBehavior), new PropertyMetadata());
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MaskBehavior), new PropertyMetadata());

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        public bool? ShowCancel
        {
            get { return (bool?)GetValue(ShowCancelProperty); }
            set { SetValue(ShowCancelProperty, value); }
        }


        public static readonly DependencyProperty ShowCancelProperty =
            DependencyProperty.Register("ShowCancel", typeof(bool?), typeof(MaskBehavior), new PropertyMetadata(false));


        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }


        public ICommand MaskCommand
        {
            get { return (ICommand)GetValue(MaskCommandProperty); }
            set { SetValue(MaskCommandProperty, value); }
        }


        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject.IsLoaded)
            {
                Refresh();
            }
            else
            {
                AssociatedObject.Loaded += (s, e) => Refresh();
            }
        }

        void Refresh()
        {
            if (this.ShowCancel == false)
                Show_Mask();
            else if (this.ShowCancel == true)
                Show_Cancel();
        }

        protected override void OnDetaching()
        {

            base.OnDetaching();
        }

        public void Show_Mask()
        {
            if (AdornerLayer.GetAdornerLayer(AssociatedObject) is { } adornerLayer)
            {
                adornerLayer.Clear(AssociatedObject);
                AdornerLayer? adLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
                Command relayCommand = new Command(() =>
                {
                    MaskCommand?.Execute(null);
                    ShowCancel=true;
                    Refresh();
                });

                var adorner = new MaskAdorner(AssociatedObject, relayCommand) { Opacity = 0.0 };
                adLayer.Add(adorner);

                var watermarkadorner = new WatermarkAdorner(AssociatedObject, Text) { Opacity = 0 };
                adLayer.Add(watermarkadorner);



                DoubleAnimation doubleAnimation = new(1, new Duration(TimeSpan.FromSeconds(1)));
                //doubleAnimation.Completed += fadeOutAnimation_Completed;
                doubleAnimation.Freeze();
                watermarkadorner.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
            }
        }

        public void Show_Cancel()
        {
            if (AdornerLayer.GetAdornerLayer(AssociatedObject) is { } adornerLayer)
            {
                adornerLayer.Clear(AssociatedObject);
                AdornerLayer? adLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
                RelayCommand relayCommand = new((e) =>
                {
                    CancelCommand?.Execute(null);
                    ShowCancel = false;
                    Refresh();
                });
                var adorner = new CancelAdorner(AssociatedObject, relayCommand) { Opacity = 0.0 };
                adLayer.Add(adorner);

                DoubleAnimation doubleAnimation = new(1, new Duration(TimeSpan.FromSeconds(1)));
                //doubleAnimation.Completed += fadeOutAnimation_Completed;
                doubleAnimation.Freeze();
                adorner.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
            }
        }

    }
}
