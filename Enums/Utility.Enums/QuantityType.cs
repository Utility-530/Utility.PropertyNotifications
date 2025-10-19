using Utility.Enums.Attributes;

namespace Utility.Enums
{
    public enum QuantityType
    {
        // --- Miscellaneous ---
        Default = 0,

        // --- Base SI Quantities ---
        [Quantity("m", "mm", "cm", "m", "km", "in", "ft", "yd", "mi")]
        Length,

        [Quantity("kg", "mg", "g", "kg", "t", "lb", "oz")]
        Mass,

        [Quantity("s", "ms", "s", "min", "h", "day")]
        Time,

        [Quantity("A", "μA", "mA", "A")]
        ElectricCurrent,

        [Quantity("K", "°C", "K", "°F")]
        Temperature,

        [Quantity("mol")]
        AmountOfSubstance,

        [Quantity("cd")]
        LuminousIntensity,

        // --- Derived Mechanical Quantities ---
        [Quantity("m²", "m²", "km²", "ft²", "in²", "acre", "ha")]
        Area,

        [Quantity("m³", "cm³","L", "mL", "ft³", "in³")]
        Volume,

        [Quantity("m/s","km/h", "mph", "ft/s")]
        Speed,

        [Quantity("m/s²", "ft/s²", "g")]
        Acceleration,

        [Quantity("N", "kN", "lbf")]
        Force,

        [Quantity("Pa", "kPa", "bar", "psi", "atm")]
        Pressure,

        [Quantity("J", "kJ", "Wh", "kWh", "cal")]
        Energy,

        [Quantity("W", "kW", "MW", "hp")]
        Power,

        [Quantity("N·m", "lbf·ft")]
        Torque,

        [Quantity("kg/m³", "g/cm³", "lb/ft³")]
        Density,

        [Quantity("Pa·s", "cP")]
        DynamicViscosity,

        [Quantity("m²/s", "St", "cSt")]
        KinematicViscosity,

        // --- Electrical Quantities ---
        [Quantity("C", "Ah", "mAh")]
        ElectricCharge,

        [Quantity("V", "mV", "kV")]
        Voltage,

        [Quantity("F", "μF", "nF", "pF")]
        Capacitance,

        [Quantity("Ω", "kΩ", "MΩ")]
        Resistance,

        [Quantity("S", "mS", "μS")]
        Conductance,

        [Quantity("Wb")]
        MagneticFlux,

        [Quantity("T", "G")]
        MagneticFluxDensity,

        [Quantity("H", "mH", "μH")]
        Inductance,

        // --- Wave / Periodic Quantities ---
        [Quantity("Frequency", "Hz", "Hz", "kHz", "MHz", "GHz")]
        Frequency,

        [Quantity("Wavelength", "m", "nm", "μm", "mm", "cm", "m")]
        Wavelength,

        [Quantity("Angular Velocity", "rad/s")]
        AngularVelocity,

        [Quantity("Angle", "rad", "°", "gon")]
        Angle,

        [Quantity("Solid Angle", "sr")]
        SolidAngle,

        // --- Thermodynamic Quantities ---
        [Quantity("Entropy", "J/K")]
        Entropy,

        [Quantity("Heat Capacity", "J/K")]
        HeatCapacity,

        [Quantity("Specific Heat Capacity", "J/(kg·K)")]
        SpecificHeatCapacity,

        // --- Photometric Quantities ---
        [Quantity("lm")]
        LuminousFlux,

        [Quantity("lx")]
        Illuminance,

        // --- Chemical / Physical Quantities ---
        [Quantity("mol/m³", "mol/m³", "mol/L", "mol/dm³")]
        Concentration,

        [Quantity( "Bq", "Ci")]
        Radioactivity,

        [Quantity("Gy", "rad")]
        AbsorbedDose,

        [Quantity("Sv", "rem")]
        EquivalentDose,

        // --- Financial & Abstract Quantities ---
        [Quantity("$", "€", "£", "¥")]
        Currency,

        [Quantity("pcs", "units", "items")]
        Count,

        [Quantity("%")]
        Percentage,


        // --- Information & Data ---
        [Quantity("bit", "byte", "B", "kB", "MB", "GB", "TB")]
        Information,

        [Quantity("bit/s", "kbit/s", "Mbit/s", "Gbit/s")]
        DataRate,

        [Quantity("byte", "kB", "MB", "GB", "TB")]
        StorageCapacity,


    }


}