namespace Utility.Enums
{
    /// <summary>
    /// Defines the valid units for a <see cref="Dimension"/>.
    /// </summary>
    public enum DimensionUnitType
    {
        /// <summary>
        /// The row or column is auto-sized to fit its content.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// The row or column is sized in device independent pixels.
        /// </summary>
        Pixel = 1,

        /// <summary>
        /// The row or column is sized as a weighted proportion of available space.
        /// </summary>
        Star = 2,
    }
}