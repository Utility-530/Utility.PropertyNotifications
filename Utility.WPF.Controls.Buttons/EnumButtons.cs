using NetFabric.Hyperlinq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Utility.Common.Collections;
using Utility.Enums;
using Utility.Helpers;
using Utility.WPF.Helper;
using static Evan.Wpf.DependencyHelper;


namespace Utility.WPF.Controls.Buttons
{


    public partial class DirectionButtons : EnumButtons
    {

        Map<Enum, Button>? map;


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
                .Select(a => EnumHelper.SeparateFlag((Direction)a))
                .Subscribe(a =>
                {
                    foreach (var x in map)
                    {
                        x.Value.IsEnabled = a.Contains((Direction)x.Key);
                    }
                });

            this.WhenAnyValue(a => a.Visible)
             .Select(a => EnumHelper.SeparateFlag((Direction)a))
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

    public abstract class EnumButtons<TEnum> : EnumButtons where TEnum : struct, Enum
    {
        private Map<Enum, Button>? map;

        protected override Map<Enum, Button> Map => map;

        public EnumButtons()
        {

        }

        public override void OnApplyTemplate()
        {
            map = new Map<Enum, Button>(this.FindChildren<Button>().ToDictionary(a => (Enum)Enum.Parse<TEnum>(a.Name), a => a));
            foreach (var m in map)
            {
                m.Value.Click += Value_Click;
            }

            this.WhenAnyValue(a => a.Enabled)
                .Select(a => a == null ? null : GetIndividualFlags<TEnum>((TEnum)a).Cast<Enum>())
                .Subscribe(a =>
                {
                    foreach (var x in Map)
                    {
                        x.Value.IsEnabled = a?.Contains(x.Key) != false;
                    }
                });

            this.WhenAnyValue(a => a.Visible)
                .Select(a => a == null ? null : GetIndividualFlags<TEnum>((TEnum)a).Cast<Enum>())
                .Subscribe(a =>
                {
                    foreach (var x in Map)
                    {
                        x.Value.Visibility = a?.Contains(x.Key) != false ? Visibility.Visible : Visibility.Collapsed;
                    }
                });


            base.OnApplyTemplate();
        }

        private void Value_Click(object sender, RoutedEventArgs e)
        {
            var map = Map[(Button)sender];
            Command?.Execute(map);
            e.Handled = true;
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
            MarginsProperty = Register(new PropertyMetadata(new Thickness(8)));

        protected abstract Map<Enum, Button> Map { get; }

        public EnumButtons()
        {
            //   addRemove = new() { { Direction.Left, LeftButton }, { Direction.Right, RightButton }, { Direction.Down, DownButton }, { Direction.Up, UpButton } };

        }


        public static IEnumerable<T> GetIndividualFlags<T>(T flags) where T : Enum
        {
            // Get the underlying type of the enum
            Type underlyingType = Enum.GetUnderlyingType(typeof(T));

            // Get the values of all the individual flags in the enum
            var individualFlags = Enum.GetValues(typeof(T))
                                      .Cast<T>()
                                      .Where(f => flags.HasFlag(f));

            return individualFlags;
        }


        #region dependencyproperties

        public Enum Enabled
        {
            get { return (Enum)GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
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
