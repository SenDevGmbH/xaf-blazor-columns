using System.ComponentModel;

namespace SenDev.Xaf.Blazor.Columns;

public interface IModelBlazorColumnWidthMode
{
    [DefaultValue(ColumnWidthMode.Default)]
    [Category("Layout")]
    ColumnWidthMode ColumnWidthMode { get; set; }
}
