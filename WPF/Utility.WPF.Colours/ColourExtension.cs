using System;
using System.Windows.Data;
using System.Windows.Markup;
using Leepfrog.WpfFramework;
using MaterialDesignColors.Recommended;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Colour = Utility.Enums.Colour;

namespace Utility.WPF.Colours
{
    public class RedExtension : ColourExtension
    {
        public RedExtension() : this(ColourIntensity.Default, null)
        {
        }

        public RedExtension(ColourIntensity value) : this(value, null)
        {
        }

        public RedExtension(ColourIntensity value, Type? type) : base(Colour.Red, value, type)
        {
        }
    }

    public class PinkExtension : ColourExtension
    {
        public PinkExtension() : this(ColourIntensity.Default, null)
        {
        }

        public PinkExtension(ColourIntensity value) : this(value, null)
        {
        }

        public PinkExtension(ColourIntensity value, Type? type) : base(Colour.Pink, value, type)
        {
        }
    }
    public class PurpleExtension : ColourExtension
    {
        public PurpleExtension() : this(ColourIntensity.Default, null)
        {
        }

        public PurpleExtension(ColourIntensity value) : this(value, null)
        {
        }

        public PurpleExtension(ColourIntensity value, Type? type) : base(Colour.Purple, value, type)
        {
        }
    }
    public class DeepPurpleExtension : ColourExtension
    {
        public DeepPurpleExtension() : this(ColourIntensity.Default, null)
        {
        }

        public DeepPurpleExtension(ColourIntensity value) : this(value, null)
        {
        }

        public DeepPurpleExtension(ColourIntensity value, Type? type) : base(Colour.DeepPurple, value, type)
        {
        }
    }
    public class IndigoExtension : ColourExtension
    {
        public IndigoExtension() : this(ColourIntensity.Default, null)
        {
        }

        public IndigoExtension(ColourIntensity value) : this(value, null)
        {
        }

        public IndigoExtension(ColourIntensity value, Type? type) : base(Colour.Indigo, value, type)
        {
        }
    }
    public class BlueExtension : ColourExtension
    {
        public BlueExtension() : this(ColourIntensity.Default, null)
        {
        }

        public BlueExtension(ColourIntensity value) : this(value, null)
        {
        }

        public BlueExtension(ColourIntensity value, Type? type) : base(Colour.Blue, value, type)
        {
        }
    }
    public class TealExtension : ColourExtension
    {
        public TealExtension() : this(ColourIntensity.Default, null)
        {
        }

        public TealExtension(ColourIntensity value) : this(value, null)
        {
        }

        public TealExtension(ColourIntensity value, Type? type) : base(Colour.Teal, value, type)
        {
        }
    }
    public class CyanExtension : ColourExtension
    {
        public CyanExtension() : this(ColourIntensity.Default, null)
        {
        }

        public CyanExtension(ColourIntensity value) : this(value, null)
        {
        }

        public CyanExtension(ColourIntensity value, Type? type) : base(Colour.Cyan, value, type)
        {
        }
    }
    public class GreenExtension : ColourExtension
    {
        public GreenExtension() : this(ColourIntensity.Default, null)
        {
        }

        public GreenExtension(ColourIntensity value) : this(value, null)
        {
        }

        public GreenExtension(ColourIntensity value, Type? type) : base(Colour.Green, value, type)
        {
        }
    }
    public class LightGreenExtension : ColourExtension
    {
        public LightGreenExtension() : this(ColourIntensity.Default, null)
        {
        }

        public LightGreenExtension(ColourIntensity value) : this(value, null)
        {
        }

        public LightGreenExtension(ColourIntensity value, Type? type) : base(Colour.LightGreen, value, type)
        {
        }
    }
    public class LimeExtension : ColourExtension
    {
        public LimeExtension() : this(ColourIntensity.Default, null)
        {
        }

        public LimeExtension(ColourIntensity value) : this(value, null)
        {
        }

        public LimeExtension(ColourIntensity value, Type? type) : base(Colour.Lime, value, type)
        {
        }
    }

    public class YellowExtension : ColourExtension
    {
        public YellowExtension() : this(ColourIntensity.Default, null)
        {
        }

        public YellowExtension(ColourIntensity value) : this(value, null)
        {
        }

        public YellowExtension(ColourIntensity value, Type? type) : base(Colour.Yellow, value, type)
        {
        }
    }

    public class AmberExtension : ColourExtension
    {
        public AmberExtension() : this(ColourIntensity.Default, null)
        {
        }

        public AmberExtension(ColourIntensity value) : this(value, null)
        {
        }

        public AmberExtension(ColourIntensity value, Type? type) : base(Colour.Amber, value, type)
        {
        }
    }

    public class OrangeExtension : ColourExtension
    {
        public OrangeExtension() : this(ColourIntensity.Default, null)
        {
        }

        public OrangeExtension(ColourIntensity value) : this(value, null)
        {
        }

        public OrangeExtension(ColourIntensity value, Type? type) : base(Colour.Orange, value, type)
        {
        }
    }

    public class DeepOrangeExtension : ColourExtension
    {
        public DeepOrangeExtension() : this(ColourIntensity.Default, null)
        {
        }

        public DeepOrangeExtension(ColourIntensity value) : this(value, null)
        {
        }

        public DeepOrangeExtension(ColourIntensity value, Type? type) : base(Colour.DeepOrange, value, type)
        {
        }
    }


    public class BrownExtension : ColourExtension
    {
        public BrownExtension() : this(ColourIntensity.Default, null)
        {
        }

        public BrownExtension(ColourIntensity value) : this(value, null)
        {
        }

        public BrownExtension(ColourIntensity value, Type? type) : base(Colour.Brown, value, type)
        {
        }
    }


    public class GreyExtension : ColourExtension
    {
        public GreyExtension() : this(ColourIntensity.Default, null)
        {
        }

        public GreyExtension(ColourIntensity value) : this(value, null)
        {
        }

        public GreyExtension(ColourIntensity value, Type? type) : base(Colour.Grey, value, type)
        {
        }
    }


    public class BlueGreyExtension : ColourExtension
    {
        public BlueGreyExtension() : this(ColourIntensity.Default, null)
        {
        }

        public BlueGreyExtension(ColourIntensity value) : this(value, null)
        {
        }

        public BlueGreyExtension(ColourIntensity value, Type? type) : base(Colour.BlueGrey, value, type)
        {
        }
    }


    public class ColourExtension : MarkupExtension
    {


        public enum ColourIntensity
        {
            Fifty, OneHundred, TwoHundred, ThreeHundred, FourHundred, FiveHundred, SixHundred, SevenHundred, EightHundred, NineHundred, AlphaOneHundred, AlphaTwoHundred, AlphaFourHundred, AlphaSevenHundred, Default = TwoHundred
        }


        [ConstructorArgument("value")]
        public ColourIntensity Value { get; set; }

        [ConstructorArgument("type")]
        public Type? Type { get; set; }

        private Color _color;

        public ColourExtension()
        {
        }

        public ColourExtension(Colour colour, ColourIntensity value) : this(colour, value, null)
        {
        }

        public ColourExtension(Colour colour, ColourIntensity value, Type? type)
        {
            Value = value;
            _color = Convert(value, colour);
            Type = type;
        }

        public static Color Convert(ColourIntensity intensity, Colour colour)
        {
            return (intensity, colour) switch
            {
                //case ColorType.Background: return Colours.Background;
                (ColourIntensity.Fifty, Colour.Red) => RedSwatch.Red50,
                (ColourIntensity.OneHundred, Colour.Red) => RedSwatch.Red100,
                (ColourIntensity.TwoHundred, Colour.Red) => RedSwatch.Red200,
                (ColourIntensity.ThreeHundred, Colour.Red) => RedSwatch.Red300,
                (ColourIntensity.FourHundred, Colour.Red) => RedSwatch.Red400,
                (ColourIntensity.FiveHundred, Colour.Red) => RedSwatch.Red500,
                (ColourIntensity.SixHundred, Colour.Red) => RedSwatch.Red600,
                (ColourIntensity.SevenHundred, Colour.Red) => RedSwatch.Red700,
                (ColourIntensity.EightHundred, Colour.Red) => RedSwatch.Red800,
                (ColourIntensity.NineHundred, Colour.Red) => RedSwatch.Red900,
                (ColourIntensity.AlphaOneHundred, Colour.Red) => RedSwatch.RedA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Red) => RedSwatch.RedA200,
                (ColourIntensity.AlphaFourHundred, Colour.Red) => RedSwatch.RedA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Red) => RedSwatch.RedA700,
                (ColourIntensity.Fifty, Colour.Pink) => PinkSwatch.Pink50,
                (ColourIntensity.OneHundred, Colour.Pink) => PinkSwatch.Pink100,
                (ColourIntensity.TwoHundred, Colour.Pink) => PinkSwatch.Pink200,
                (ColourIntensity.ThreeHundred, Colour.Pink) => PinkSwatch.Pink300,
                (ColourIntensity.FourHundred, Colour.Pink) => PinkSwatch.Pink400,
                (ColourIntensity.FiveHundred, Colour.Pink) => PinkSwatch.Pink500,
                (ColourIntensity.SixHundred, Colour.Pink) => PinkSwatch.Pink600,
                (ColourIntensity.SevenHundred, Colour.Pink) => PinkSwatch.Pink700,
                (ColourIntensity.EightHundred, Colour.Pink) => PinkSwatch.Pink800,
                (ColourIntensity.NineHundred, Colour.Pink) => PinkSwatch.Pink900,
                (ColourIntensity.AlphaOneHundred, Colour.Pink) => PinkSwatch.PinkA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Pink) => PinkSwatch.PinkA200,
                (ColourIntensity.AlphaFourHundred, Colour.Pink) => PinkSwatch.PinkA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Pink) => PinkSwatch.PinkA700,
                (ColourIntensity.Fifty, Colour.Purple) => PurpleSwatch.Purple50,
                (ColourIntensity.OneHundred, Colour.Purple) => PurpleSwatch.Purple100,
                (ColourIntensity.TwoHundred, Colour.Purple) => PurpleSwatch.Purple200,
                (ColourIntensity.ThreeHundred, Colour.Purple) => PurpleSwatch.Purple300,
                (ColourIntensity.FourHundred, Colour.Purple) => PurpleSwatch.Purple400,
                (ColourIntensity.FiveHundred, Colour.Purple) => PurpleSwatch.Purple500,
                (ColourIntensity.SixHundred, Colour.Purple) => PurpleSwatch.Purple600,
                (ColourIntensity.SevenHundred, Colour.Purple) => PurpleSwatch.Purple700,
                (ColourIntensity.EightHundred, Colour.Purple) => PurpleSwatch.Purple800,
                (ColourIntensity.NineHundred, Colour.Purple) => PurpleSwatch.Purple900,
                (ColourIntensity.AlphaOneHundred, Colour.Purple) => PurpleSwatch.PurpleA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Purple) => PurpleSwatch.PurpleA200,
                (ColourIntensity.AlphaFourHundred, Colour.Purple) => PurpleSwatch.PurpleA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Purple) => PurpleSwatch.PurpleA700,
                (ColourIntensity.Fifty, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurple50,
                (ColourIntensity.OneHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurple100,
                (ColourIntensity.TwoHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurple200,
                (ColourIntensity.ThreeHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurple300,
                (ColourIntensity.FourHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurple400,
                (ColourIntensity.FiveHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurple500,
                (ColourIntensity.SixHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurple600,
                (ColourIntensity.SevenHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurple700,
                (ColourIntensity.EightHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurple800,
                (ColourIntensity.NineHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurple900,
                (ColourIntensity.AlphaOneHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurpleA100,
                (ColourIntensity.AlphaTwoHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurpleA200,
                (ColourIntensity.AlphaFourHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurpleA400,
                (ColourIntensity.AlphaSevenHundred, Colour.DeepPurple) => DeepPurpleSwatch.DeepPurpleA700,
                (ColourIntensity.Fifty, Colour.Indigo) => IndigoSwatch.Indigo50,
                (ColourIntensity.OneHundred, Colour.Indigo) => IndigoSwatch.Indigo100,
                (ColourIntensity.TwoHundred, Colour.Indigo) => IndigoSwatch.Indigo200,
                (ColourIntensity.ThreeHundred, Colour.Indigo) => IndigoSwatch.Indigo300,
                (ColourIntensity.FourHundred, Colour.Indigo) => IndigoSwatch.Indigo400,
                (ColourIntensity.FiveHundred, Colour.Indigo) => IndigoSwatch.Indigo500,
                (ColourIntensity.SixHundred, Colour.Indigo) => IndigoSwatch.Indigo600,
                (ColourIntensity.SevenHundred, Colour.Indigo) => IndigoSwatch.Indigo700,
                (ColourIntensity.EightHundred, Colour.Indigo) => IndigoSwatch.Indigo800,
                (ColourIntensity.NineHundred, Colour.Indigo) => IndigoSwatch.Indigo900,
                (ColourIntensity.AlphaOneHundred, Colour.Indigo) => IndigoSwatch.IndigoA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Indigo) => IndigoSwatch.IndigoA200,
                (ColourIntensity.AlphaFourHundred, Colour.Indigo) => IndigoSwatch.IndigoA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Indigo) => IndigoSwatch.IndigoA700,
                (ColourIntensity.Fifty, Colour.Blue) => BlueSwatch.Blue50,
                (ColourIntensity.OneHundred, Colour.Blue) => BlueSwatch.Blue100,
                (ColourIntensity.TwoHundred, Colour.Blue) => BlueSwatch.Blue200,
                (ColourIntensity.ThreeHundred, Colour.Blue) => BlueSwatch.Blue300,
                (ColourIntensity.FourHundred, Colour.Blue) => BlueSwatch.Blue400,
                (ColourIntensity.FiveHundred, Colour.Blue) => BlueSwatch.Blue500,
                (ColourIntensity.SixHundred, Colour.Blue) => BlueSwatch.Blue600,
                (ColourIntensity.SevenHundred, Colour.Blue) => BlueSwatch.Blue700,
                (ColourIntensity.EightHundred, Colour.Blue) => BlueSwatch.Blue800,
                (ColourIntensity.NineHundred, Colour.Blue) => BlueSwatch.Blue900,
                (ColourIntensity.AlphaOneHundred, Colour.Blue) => BlueSwatch.BlueA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Blue) => BlueSwatch.BlueA200,
                (ColourIntensity.AlphaFourHundred, Colour.Blue) => BlueSwatch.BlueA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Blue) => BlueSwatch.BlueA700,
                (ColourIntensity.Fifty, Colour.LightBlue) => LightBlueSwatch.LightBlue50,
                (ColourIntensity.OneHundred, Colour.LightBlue) => LightBlueSwatch.LightBlue100,
                (ColourIntensity.TwoHundred, Colour.LightBlue) => LightBlueSwatch.LightBlue200,
                (ColourIntensity.ThreeHundred, Colour.LightBlue) => LightBlueSwatch.LightBlue300,
                (ColourIntensity.FourHundred, Colour.LightBlue) => LightBlueSwatch.LightBlue400,
                (ColourIntensity.FiveHundred, Colour.LightBlue) => LightBlueSwatch.LightBlue500,
                (ColourIntensity.SixHundred, Colour.LightBlue) => LightBlueSwatch.LightBlue600,
                (ColourIntensity.SevenHundred, Colour.LightBlue) => LightBlueSwatch.LightBlue700,
                (ColourIntensity.EightHundred, Colour.LightBlue) => LightBlueSwatch.LightBlue800,
                (ColourIntensity.NineHundred, Colour.LightBlue) => LightBlueSwatch.LightBlue900,
                (ColourIntensity.AlphaOneHundred, Colour.LightBlue) => LightBlueSwatch.LightBlueA100,
                (ColourIntensity.AlphaTwoHundred, Colour.LightBlue) => LightBlueSwatch.LightBlueA200,
                (ColourIntensity.AlphaFourHundred, Colour.LightBlue) => LightBlueSwatch.LightBlueA400,
                (ColourIntensity.AlphaSevenHundred, Colour.LightBlue) => LightBlueSwatch.LightBlueA700,
                (ColourIntensity.Fifty, Colour.Cyan) => CyanSwatch.Cyan50,
                (ColourIntensity.OneHundred, Colour.Cyan) => CyanSwatch.Cyan100,
                (ColourIntensity.TwoHundred, Colour.Cyan) => CyanSwatch.Cyan200,
                (ColourIntensity.ThreeHundred, Colour.Cyan) => CyanSwatch.Cyan300,
                (ColourIntensity.FourHundred, Colour.Cyan) => CyanSwatch.Cyan400,
                (ColourIntensity.FiveHundred, Colour.Cyan) => CyanSwatch.Cyan500,
                (ColourIntensity.SixHundred, Colour.Cyan) => CyanSwatch.Cyan600,
                (ColourIntensity.SevenHundred, Colour.Cyan) => CyanSwatch.Cyan700,
                (ColourIntensity.EightHundred, Colour.Cyan) => CyanSwatch.Cyan800,
                (ColourIntensity.NineHundred, Colour.Cyan) => CyanSwatch.Cyan900,
                (ColourIntensity.AlphaOneHundred, Colour.Cyan) => CyanSwatch.CyanA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Cyan) => CyanSwatch.CyanA200,
                (ColourIntensity.AlphaFourHundred, Colour.Cyan) => CyanSwatch.CyanA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Cyan) => CyanSwatch.CyanA700,
                (ColourIntensity.Fifty, Colour.Teal) => TealSwatch.Teal50,
                (ColourIntensity.OneHundred, Colour.Teal) => TealSwatch.Teal100,
                (ColourIntensity.TwoHundred, Colour.Teal) => TealSwatch.Teal200,
                (ColourIntensity.ThreeHundred, Colour.Teal) => TealSwatch.Teal300,
                (ColourIntensity.FourHundred, Colour.Teal) => TealSwatch.Teal400,
                (ColourIntensity.FiveHundred, Colour.Teal) => TealSwatch.Teal500,
                (ColourIntensity.SixHundred, Colour.Teal) => TealSwatch.Teal600,
                (ColourIntensity.SevenHundred, Colour.Teal) => TealSwatch.Teal700,
                (ColourIntensity.EightHundred, Colour.Teal) => TealSwatch.Teal800,
                (ColourIntensity.NineHundred, Colour.Teal) => TealSwatch.Teal900,
                (ColourIntensity.AlphaOneHundred, Colour.Teal) => TealSwatch.TealA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Teal) => TealSwatch.TealA200,
                (ColourIntensity.AlphaFourHundred, Colour.Teal) => TealSwatch.TealA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Teal) => TealSwatch.TealA700,
                (ColourIntensity.Fifty, Colour.Green) => GreenSwatch.Green50,
                (ColourIntensity.OneHundred, Colour.Green) => GreenSwatch.Green100,
                (ColourIntensity.TwoHundred, Colour.Green) => GreenSwatch.Green200,
                (ColourIntensity.ThreeHundred, Colour.Green) => GreenSwatch.Green300,
                (ColourIntensity.FourHundred, Colour.Green) => GreenSwatch.Green400,
                (ColourIntensity.FiveHundred, Colour.Green) => GreenSwatch.Green500,
                (ColourIntensity.SixHundred, Colour.Green) => GreenSwatch.Green600,
                (ColourIntensity.SevenHundred, Colour.Green) => GreenSwatch.Green700,
                (ColourIntensity.EightHundred, Colour.Green) => GreenSwatch.Green800,
                (ColourIntensity.NineHundred, Colour.Green) => GreenSwatch.Green900,
                (ColourIntensity.AlphaOneHundred, Colour.Green) => GreenSwatch.GreenA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Green) => GreenSwatch.GreenA200,
                (ColourIntensity.AlphaFourHundred, Colour.Green) => GreenSwatch.GreenA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Green) => GreenSwatch.GreenA700,
                (ColourIntensity.Fifty, Colour.LightGreen) => LightGreenSwatch.LightGreen50,
                (ColourIntensity.OneHundred, Colour.LightGreen) => LightGreenSwatch.LightGreen100,
                (ColourIntensity.TwoHundred, Colour.LightGreen) => LightGreenSwatch.LightGreen200,
                (ColourIntensity.ThreeHundred, Colour.LightGreen) => LightGreenSwatch.LightGreen300,
                (ColourIntensity.FourHundred, Colour.LightGreen) => LightGreenSwatch.LightGreen400,
                (ColourIntensity.FiveHundred, Colour.LightGreen) => LightGreenSwatch.LightGreen500,
                (ColourIntensity.SixHundred, Colour.LightGreen) => LightGreenSwatch.LightGreen600,
                (ColourIntensity.SevenHundred, Colour.LightGreen) => LightGreenSwatch.LightGreen700,
                (ColourIntensity.EightHundred, Colour.LightGreen) => LightGreenSwatch.LightGreen800,
                (ColourIntensity.NineHundred, Colour.LightGreen) => LightGreenSwatch.LightGreen900,
                (ColourIntensity.AlphaOneHundred, Colour.LightGreen) => LightGreenSwatch.LightGreenA100,
                (ColourIntensity.AlphaTwoHundred, Colour.LightGreen) => LightGreenSwatch.LightGreenA200,
                (ColourIntensity.AlphaFourHundred, Colour.LightGreen) => LightGreenSwatch.LightGreenA400,
                (ColourIntensity.AlphaSevenHundred, Colour.LightGreen) => LightGreenSwatch.LightGreenA700,
                (ColourIntensity.Fifty, Colour.Lime) => LimeSwatch.Lime50,
                (ColourIntensity.OneHundred, Colour.Lime) => LimeSwatch.Lime100,
                (ColourIntensity.TwoHundred, Colour.Lime) => LimeSwatch.Lime200,
                (ColourIntensity.ThreeHundred, Colour.Lime) => LimeSwatch.Lime300,
                (ColourIntensity.FourHundred, Colour.Lime) => LimeSwatch.Lime400,
                (ColourIntensity.FiveHundred, Colour.Lime) => LimeSwatch.Lime500,
                (ColourIntensity.SixHundred, Colour.Lime) => LimeSwatch.Lime600,
                (ColourIntensity.SevenHundred, Colour.Lime) => LimeSwatch.Lime700,
                (ColourIntensity.EightHundred, Colour.Lime) => LimeSwatch.Lime800,
                (ColourIntensity.NineHundred, Colour.Lime) => LimeSwatch.Lime900,
                (ColourIntensity.AlphaOneHundred, Colour.Lime) => LimeSwatch.LimeA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Lime) => LimeSwatch.LimeA200,
                (ColourIntensity.AlphaFourHundred, Colour.Lime) => LimeSwatch.LimeA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Lime) => LimeSwatch.LimeA700,
                (ColourIntensity.Fifty, Colour.Yellow) => YellowSwatch.Yellow50,
                (ColourIntensity.OneHundred, Colour.Yellow) => YellowSwatch.Yellow100,
                (ColourIntensity.TwoHundred, Colour.Yellow) => YellowSwatch.Yellow200,
                (ColourIntensity.ThreeHundred, Colour.Yellow) => YellowSwatch.Yellow300,
                (ColourIntensity.FourHundred, Colour.Yellow) => YellowSwatch.Yellow400,
                (ColourIntensity.FiveHundred, Colour.Yellow) => YellowSwatch.Yellow500,
                (ColourIntensity.SixHundred, Colour.Yellow) => YellowSwatch.Yellow600,
                (ColourIntensity.SevenHundred, Colour.Yellow) => YellowSwatch.Yellow700,
                (ColourIntensity.EightHundred, Colour.Yellow) => YellowSwatch.Yellow800,
                (ColourIntensity.NineHundred, Colour.Yellow) => YellowSwatch.Yellow900,
                (ColourIntensity.AlphaOneHundred, Colour.Yellow) => YellowSwatch.YellowA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Yellow) => YellowSwatch.YellowA200,
                (ColourIntensity.AlphaFourHundred, Colour.Yellow) => YellowSwatch.YellowA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Yellow) => YellowSwatch.YellowA700,
                (ColourIntensity.Fifty, Colour.Amber) => AmberSwatch.Amber50,
                (ColourIntensity.OneHundred, Colour.Amber) => AmberSwatch.Amber100,
                (ColourIntensity.TwoHundred, Colour.Amber) => AmberSwatch.Amber200,
                (ColourIntensity.ThreeHundred, Colour.Amber) => AmberSwatch.Amber300,
                (ColourIntensity.FourHundred, Colour.Amber) => AmberSwatch.Amber400,
                (ColourIntensity.FiveHundred, Colour.Amber) => AmberSwatch.Amber500,
                (ColourIntensity.SixHundred, Colour.Amber) => AmberSwatch.Amber600,
                (ColourIntensity.SevenHundred, Colour.Amber) => AmberSwatch.Amber700,
                (ColourIntensity.EightHundred, Colour.Amber) => AmberSwatch.Amber800,
                (ColourIntensity.NineHundred, Colour.Amber) => AmberSwatch.Amber900,
                (ColourIntensity.AlphaOneHundred, Colour.Amber) => AmberSwatch.AmberA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Amber) => AmberSwatch.AmberA200,
                (ColourIntensity.AlphaFourHundred, Colour.Amber) => AmberSwatch.AmberA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Amber) => AmberSwatch.AmberA700,
                (ColourIntensity.Fifty, Colour.Orange) => OrangeSwatch.Orange50,
                (ColourIntensity.OneHundred, Colour.Orange) => OrangeSwatch.Orange100,
                (ColourIntensity.TwoHundred, Colour.Orange) => OrangeSwatch.Orange200,
                (ColourIntensity.ThreeHundred, Colour.Orange) => OrangeSwatch.Orange300,
                (ColourIntensity.FourHundred, Colour.Orange) => OrangeSwatch.Orange400,
                (ColourIntensity.FiveHundred, Colour.Orange) => OrangeSwatch.Orange500,
                (ColourIntensity.SixHundred, Colour.Orange) => OrangeSwatch.Orange600,
                (ColourIntensity.SevenHundred, Colour.Orange) => OrangeSwatch.Orange700,
                (ColourIntensity.EightHundred, Colour.Orange) => OrangeSwatch.Orange800,
                (ColourIntensity.NineHundred, Colour.Orange) => OrangeSwatch.Orange900,
                (ColourIntensity.AlphaOneHundred, Colour.Orange) => OrangeSwatch.OrangeA100,
                (ColourIntensity.AlphaTwoHundred, Colour.Orange) => OrangeSwatch.OrangeA200,
                (ColourIntensity.AlphaFourHundred, Colour.Orange) => OrangeSwatch.OrangeA400,
                (ColourIntensity.AlphaSevenHundred, Colour.Orange) => OrangeSwatch.OrangeA700,
                (ColourIntensity.Fifty, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrange50,
                (ColourIntensity.OneHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrange100,
                (ColourIntensity.TwoHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrange200,
                (ColourIntensity.ThreeHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrange300,
                (ColourIntensity.FourHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrange400,
                (ColourIntensity.FiveHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrange500,
                (ColourIntensity.SixHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrange600,
                (ColourIntensity.SevenHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrange700,
                (ColourIntensity.EightHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrange800,
                (ColourIntensity.NineHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrange900,
                (ColourIntensity.AlphaOneHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrangeA100,
                (ColourIntensity.AlphaTwoHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrangeA200,
                (ColourIntensity.AlphaFourHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrangeA400,
                (ColourIntensity.AlphaSevenHundred, Colour.DeepOrange) => DeepOrangeSwatch.DeepOrangeA700,
                (ColourIntensity.Fifty, Colour.Brown) => BrownSwatch.Brown50,
                (ColourIntensity.OneHundred, Colour.Brown) => BrownSwatch.Brown100,
                (ColourIntensity.TwoHundred, Colour.Brown) => BrownSwatch.Brown200,
                (ColourIntensity.ThreeHundred, Colour.Brown) => BrownSwatch.Brown300,
                (ColourIntensity.FourHundred, Colour.Brown) => BrownSwatch.Brown400,
                (ColourIntensity.FiveHundred, Colour.Brown) => BrownSwatch.Brown500,
                (ColourIntensity.SixHundred, Colour.Brown) => BrownSwatch.Brown600,
                (ColourIntensity.SevenHundred, Colour.Brown) => BrownSwatch.Brown700,
                (ColourIntensity.EightHundred, Colour.Brown) => BrownSwatch.Brown800,
                (ColourIntensity.NineHundred, Colour.Brown) => BrownSwatch.Brown900,
                (ColourIntensity.Fifty, Colour.Grey) => GreySwatch.Grey50,
                (ColourIntensity.OneHundred, Colour.Grey) => GreySwatch.Grey100,
                (ColourIntensity.TwoHundred, Colour.Grey) => GreySwatch.Grey200,
                (ColourIntensity.ThreeHundred, Colour.Grey) => GreySwatch.Grey300,
                (ColourIntensity.FourHundred, Colour.Grey) => GreySwatch.Grey400,
                (ColourIntensity.FiveHundred, Colour.Grey) => GreySwatch.Grey500,
                (ColourIntensity.SixHundred, Colour.Grey) => GreySwatch.Grey600,
                (ColourIntensity.SevenHundred, Colour.Grey) => GreySwatch.Grey700,
                (ColourIntensity.EightHundred, Colour.Grey) => GreySwatch.Grey800,
                (ColourIntensity.NineHundred, Colour.Grey) => GreySwatch.Grey900,
                (ColourIntensity.Fifty, Colour.BlueGrey) => BlueGreySwatch.BlueGrey50,
                (ColourIntensity.OneHundred, Colour.BlueGrey) => BlueGreySwatch.BlueGrey100,
                (ColourIntensity.TwoHundred, Colour.BlueGrey) => BlueGreySwatch.BlueGrey200,
                (ColourIntensity.ThreeHundred, Colour.BlueGrey) => BlueGreySwatch.BlueGrey300,
                (ColourIntensity.FourHundred, Colour.BlueGrey) => BlueGreySwatch.BlueGrey400,
                (ColourIntensity.FiveHundred, Colour.BlueGrey) => BlueGreySwatch.BlueGrey500,
                (ColourIntensity.SixHundred, Colour.BlueGrey) => BlueGreySwatch.BlueGrey600,
                (ColourIntensity.SevenHundred, Colour.BlueGrey) => BlueGreySwatch.BlueGrey700,
                (ColourIntensity.EightHundred, Colour.BlueGrey) => BlueGreySwatch.BlueGrey800,
                (ColourIntensity.NineHundred, Colour.BlueGrey) => BlueGreySwatch.BlueGrey900,
                _ => throw new ArgumentOutOfRangeException($"ColorType {intensity}, {colour}"),
            };
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //=================================================================
            // if target is not yet available, and we need to wait for it...
            if (XamlHelper.IsTargetNull(serviceProvider))
            {
                // return the markupextension itself for now
                // it will be evaluated again later
                return this;
            }
            //-----------------------------------------------------------------
            Color color = _color;
            Type? type = Type;
            //-----------------------------------------------------------------
            if (type == null)
            {
                if (serviceProvider is IProvideValueTarget pvt)
                {
                    if (pvt.TargetProperty is DependencyProperty dp)
                    {
                        type = dp.PropertyType;
                    }
                    else if (pvt.TargetProperty is PropertyInfo pi)
                    {
                        type = pi.PropertyType;
                    }
                    else
                    {
                        return $"UNKNOWN - {serviceProvider?.GetType()?.Name}";
                    }
                }
            }
            if (type == typeof(Brush) || type == typeof(SolidColorBrush))
            {
                return new SolidColorBrush(color);
            }
            else if (type == typeof(Color))
            {
                return color;
            }
            else if (type == typeof(string))
            {
                return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            return new SolidColorBrush(color);
        }
    }
}



