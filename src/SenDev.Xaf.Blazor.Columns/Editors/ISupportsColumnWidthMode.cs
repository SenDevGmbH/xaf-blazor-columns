namespace SenDev.Xaf.Blazor.Columns.Editors;

public interface ISupportsColumnWidthMode
{
   public ColumnWidthMode ColumnWidthMode
    {
        get
        {
            if (ColumnsModel  is not  null)
            {
                var columnWidthMode = ColumnsModel.ColumnWidthMode;
                if (columnWidthMode != ColumnWidthMode.Default)
                    return columnWidthMode;
            }

            if (ApplicationOptionsModel is not null)
            {
                var columnWidthMode = ApplicationOptionsModel.ColumnWidthMode;
                if (columnWidthMode != ColumnWidthMode.Default)
                    return columnWidthMode;
            }
            return ColumnWidthMode.Default;
        }
    }

    IModelBlazorColumnWidthMode? ColumnsModel { get; }

    IModelBlazorColumnWidthMode? ApplicationOptionsModel { get; }

}
