namespace Utility.WPF.Demo.SandBox
{
    //public class MeasurementsControl : DoubleContentControl
    //{
    //    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(Enum), typeof(MeasurementsControl));

    //    static MeasurementsControl()
    //    {
    //        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MeasurementsControl), new FrameworkPropertyMetadata(typeof(MeasurementsControl)));
    //    }
    //    public static readonly DependencyProperty DisplayKeyPathProperty = NumbersControl.DisplayKeyPathProperty.AddOwner(typeof(MeasurementsControl));
    //    public static readonly DependencyProperty DisplayMemberPathProperty = ItemsControl.DisplayMemberPathProperty.AddOwner(typeof(MeasurementsControl));
    //    public static readonly DependencyProperty EnumFilterCollectionProperty = EnumSelectorBehavior.EnumFilterCollectionProperty.AddOwner(typeof(MeasurementsControl));

    //    public MeasurementsControl()
    //    {
    //        EnumFilterCollection = new[] { LengthUnit.Centimeter, LengthUnit.AstronomicalUnit };

    //        this.WhenAnyValue(a => a.Content).OfType<NumbersControl>()
    //            .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath), this.WhenAnyValue(a => a.DisplayKeyPath))
    //            .Subscribe(a =>
    //            {
    //                a.First.DisplayMemberPath = a.Second;
    //                a.First.DisplayKeyPath = a.Third;
    //            });
    //    }

    //    public string DisplayKeyPath
    //    {
    //        get { return (string)GetValue(DisplayKeyPathProperty); }
    //        set { SetValue(DisplayKeyPathProperty, value); }
    //    }

    //    public string DisplayMemberPath
    //    {
    //        get { return (string)GetValue(DisplayMemberPathProperty); }
    //        set { SetValue(DisplayMemberPathProperty, value); }
    //    }

    //    public Enum Unit
    //    {
    //        get { return (Enum)GetValue(UnitProperty); }
    //        set { SetValue(UnitProperty, value); }
    //    }

    //    public IEnumerable EnumFilterCollection
    //    {
    //        get { return (IEnumerable)GetValue(EnumFilterCollectionProperty); }
    //        set { SetValue(EnumFilterCollectionProperty, value); }

    //    }
    //}
}