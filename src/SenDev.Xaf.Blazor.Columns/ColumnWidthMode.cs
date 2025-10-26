namespace SenDev.Xaf.Blazor.Columns;

public enum ColumnWidthMode
{

    Default,
    /// <summary>
    /// The width is specified in pixels.
    /// </summary>
    Pixel,
    /// <summary>
    /// The width is specified as a proportion relative to the sum of all column widths.
    /// </summary>
    Proportional
}
