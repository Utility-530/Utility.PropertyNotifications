using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.Collections;
using Utility.Enums;
using Utility.Helpers;
using Utility.WPF.Helpers;
using static Evan.Wpf.DependencyHelper;
using Visibility = System.Windows.Visibility;

namespace Utility.WPF.Controls.Buttons
{
    public partial class DirectionButtons : EnumButtons
    {
        private Map<Enum, Button>? map;

        protected override Map<Enum, Button> Map => map;

        static DirectionButtons()
        {
            EnabledProperty.OverrideMetadata(forType: typeof(DirectionButtons), typeMetadata: new(Direction.Left | Direction.Right | Direction.Up | Direction.Down));
            VisibleProperty.OverrideMetadata(forType: typeof(DirectionButtons), typeMetadata: new(Direction.Left | Direction.Right | Direction.Up | Direction.Down));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectionButtons), new FrameworkPropertyMetadata(typeof(DirectionButtons)));
        }

        public override void OnApplyTemplate()
        {
            var LeftButton = (Button)this.GetTemplateChild("Left");
            var RightButton = (Button)this.GetTemplateChild("Right");
            var DownButton = (Button)this.GetTemplateChild("Down");
            var UpButton = (Button)this.GetTemplateChild("Up");

            LeftButton.Click += Value_Click;
            RightButton.Click += Value_Click;
            DownButton.Click += Value_Click;
            UpButton.Click += Value_Click;

            map = new() { { Direction.Left, LeftButton }, { Direction.Right, RightButton }, { Direction.Down, DownButton }, { Direction.Up, UpButton } };

            this.WhenAnyValue(a => a.Enabled)
                .Select(a => EnumHelper.SeparateFlags((Direction)a))
                .Subscribe(a =>
                {
                    foreach (var x in map)
                    {
                        x.Value.IsEnabled = a.Contains((Direction)x.Key);
                    }
                });

            this.WhenAnyValue(a => a.Visible)
             .Select(a => EnumHelper.SeparateFlags((Direction)a))
                .Subscribe(a =>
                {
                    foreach (var x in map)
                    {
                        x.Value.Visibility = a.Contains((Direction)x.Key) ? Visibility.Visible : Visibility.Collapsed;
                    }
                });
        }

        private void Value_Click(object sender, RoutedEventArgs e)
        {
            Command?.Execute(Map[(Button)sender]);
            e.Handled = true;
        }
    }

    //public class DirectionButtons : EnumButtons<Direction>
    //{
    //    static DirectionButtons()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectionButtons), new FrameworkPropertyMetadata(typeof(DirectionButtons)));
    //    }
    //}

    public class EditCollectionButtons : EnumButtons<AddRemove>
    {
        static EditCollectionButtons()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditCollectionButtons), new FrameworkPropertyMetadata(typeof(EditCollectionButtons)));
        }
    }

    public class PersistenceButtons : EnumButtons<Persistence>
    {
        static PersistenceButtons()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PersistenceButtons), new FrameworkPropertyMetadata(typeof(PersistenceButtons)));
        }
    }

    public class StepButtons : EnumButtons<Step>
    {
        static StepButtons()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StepButtons), new FrameworkPropertyMetadata(typeof(StepButtons)));
        }
    }

    public class PlayBackButtons : EnumButtons<Playback>
    {
        private PlayPauseButton toggle;

        static PlayBackButtons()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlayBackButtons), new FrameworkPropertyMetadata(typeof(PlayBackButtons)));
        }

        public override void OnApplyTemplate()
        {
            toggle = this.FindChildren<PlayPauseButton>().Single();
            toggle.Checked += Toggle_Checked;
            toggle.Unchecked += Toggle_Unchecked;
            base.OnApplyTemplate();
        }

        protected override void LastChanged(Enum @enum)
        {
            if(@enum is Playback.Pause)
            {
                toggle.IsChecked = false;
            }
            else if(@enum is Playback.Play)
            {
                toggle.IsChecked = true;
            }
        }

        protected override void EnableChanged(Enum @enum)
        {
            var xd = Utility.Helpers.EnumHelper.SeparateFlags((Playback)@enum).Cast<Playback>();
            if (xd.Contains(Playback.Play) == false)
            {
                toggle.IsEnabled = false;
                toggle.IsChecked = false;
            }
            else if (xd.Contains(Playback.Pause) == false)
            {
                toggle.IsEnabled = false;
                toggle.IsChecked = true;
            }
            else
            {
                toggle.IsEnabled = true;
            }
           base.EnableChanged(@enum);
        }


        private void Toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Value_Click(Playback.Pause);
            foreach (var x in Map)
            {
                x.Value.IsEnabled = true;
            }
        }

        private void Toggle_Checked(object sender, RoutedEventArgs e)
        {
            Value_Click(Playback.Play);
            foreach (var x in Map)
            {
                x.Value.IsEnabled = false;
            }

        }
    }

    public abstract class EnumButtons<TEnum> : EnumButtons where TEnum : struct, Enum
    {
        private Map<Enum, Button>? map;

        protected override Map<Enum, Button> Map => map;

        public EnumButtons()
        {
        }

        public override void OnApplyTemplate()
        {
            map ??= new Map<Enum, Button>(this.FindChildren<Button>().ToDictionary(a => (Enum)Enum.Parse<TEnum>(a.Name), a => a));

            foreach (var m in map)
            {
                m.Value.Click += (s, e) => { Value_Click(Map[(Button)s]); e.Handled = true; };
            }

            this.WhenAnyValue(a => a.Visible)
                .WhereNotNull()
                .Subscribe(LastChanged);

            this.WhenAnyValue(a => a.Enabled)
                .WhereNotNull()
                .Subscribe(EnableChanged);

            this.WhenAnyValue(a => a.Visible)
                .WhereNotNull()
                .Subscribe(VisibleChanged);

            base.OnApplyTemplate();
        }

        protected virtual void LastChanged(Enum @enum)
        {

        }

        protected virtual void EnableChanged(Enum @enum)
        {
            var xd = Utility.Helpers.EnumHelper.SeparateFlags<TEnum>((TEnum)@enum).Cast<Enum>();
            foreach (var x in Map)
            {
                x.Value.IsEnabled = xd.Contains(x.Key) != false;
            }
        }

        protected virtual void VisibleChanged(Enum @enum)
        {
            var xd = Utility.Helpers.EnumHelper.SeparateFlags<TEnum>((TEnum)@enum).Cast<Enum>();
            foreach (var x in Map)
            {
                x.Value.Visibility = xd?.Contains(x.Key) != false ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        protected void Value_Click(Enum @enum)
        {
            Last = @enum;
            Command?.Execute(map);
        }

        public static T GetAllBitsOn<T>() where T : Enum
        {
            // Get the underlying type of the enum
            Type underlyingType = Enum.GetUnderlyingType(typeof(T));

            // Get the maximum value of the underlying type
            object maxValue = Convert.ChangeType(UInt16.MaxValue, underlyingType);

            // Cast the maximum value to the enum type and return it
            return (T)Enum.ToObject(typeof(T), maxValue);
        }
    }

    public abstract class EnumButtons : Control
    {
        public static readonly DependencyProperty
            CommandProperty = Register(),
            EnabledProperty = Register(),
            VisibleProperty = Register(),
            LastProperty = Register(),
            MarginsProperty = Register(new PropertyMetadata(new Thickness(8)));

        protected abstract Map<Enum, Button> Map { get; }

        public EnumButtons()
        {
            //   addRemove = new() { { Direction.Left, LeftButton }, { Direction.Right, RightButton }, { Direction.Down, DownButton }, { Direction.Up, UpButton } };
        }

        //public static IEnumerable<T> GetIndividualFlags<T>(T flags) where T : Enum
        //{
        //    // Get the underlying type of the enum
        //    Type underlyingType = Enum.GetUnderlyingType(typeof(T));

        //    // Get the values of all the individual flags in the enum
        //    var individualFlags = Enum.GetValues(typeof(T))
        //                              .Cast<T>()
        //                              .Where(f => flags.HasFlag(f));

        //    return individualFlags;
        //}

        #region dependencyproperties

        public Enum Enabled
        {
            get { return (Enum)GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }
        public Enum Last
        {
            get { return (Enum)GetValue(LastProperty); }
            set { SetValue(LastProperty, value); }
        }

        public Enum Visible
        {
            get { return (Enum)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public Thickness Margins
        {
            get { return (Thickness)GetValue(MarginsProperty); }
            set { SetValue(MarginsProperty, value); }
        }

        #endregion dependencyproperties
    }
}