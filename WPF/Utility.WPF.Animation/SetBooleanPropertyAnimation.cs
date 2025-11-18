using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Utility.WPF.Animations
{
    /// <summary>
    /// Useful for setting a boolean property (instantly) via a Storyboard
    /// </summary>
    public class SetBooleanPropertyAnimation : BooleanAnimationUsingKeyFrames
    {
        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register(
                nameof(Property),
                typeof(PropertyPath),
                typeof(SetBooleanPropertyAnimation),
                new PropertyMetadata(null, OnPropertyChanged));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(bool),
                typeof(SetBooleanPropertyAnimation),
                new PropertyMetadata(true, OnValueChanged));

        public PropertyPath? Property
        {
            get => (PropertyPath?)GetValue(PropertyProperty);
            set => SetValue(PropertyProperty, value);
        }

        public bool Value
        {
            get => (bool)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var anim = (SetBooleanPropertyAnimation)d;
            anim.UpdateKeyFrames();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var anim = (SetBooleanPropertyAnimation)d;
            anim.UpdateKeyFrames();
        }

        private void UpdateKeyFrames()
        {
            // Clear any old frames
            KeyFrames.Clear();

            // Add a single discrete frame
            KeyFrames.Add(new DiscreteBooleanKeyFrame(Value, KeyTime.FromTimeSpan(TimeSpan.Zero)));

            // Force TargetProperty to be updated automatically when used in a Storyboard
            if (Property != null)
                Storyboard.SetTargetProperty(this, Property);

            Duration = TimeSpan.Zero; // instantaneous
        }
    }

    /*
      example usage in XAML:
     
       <Style.Triggers>
            <EventTrigger RoutedEvent="Loaded">
                <EventTrigger.Actions>
                    <BeginStoryboard>
                        <Storyboard>
                            <local:SetBooleanPropertyAnimation Property="(local:TreeTabHelper.IsLoaded)" />
                        </Storyboard>
                    </BeginStoryboard>
                </EventTrigger.Actions>
            </EventTrigger>
        </Style.Triggers>
    */

}
