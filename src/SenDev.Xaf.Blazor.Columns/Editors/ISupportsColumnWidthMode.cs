namespace SenDev.Xaf.Blazor.Columns.Editors;

public interface ISupportsColumnWidthMode
{
    public ColumnWidthMode ColumnWidthMode
    {
        get
        {
            if (ListViewModel is not null)
            {
                var columnWidthMode = ListViewModel.ColumnWidthMode;
                if (columnWidthMode != ColumnWidthMode.Default)
                    return columnWidthMode;
            }

            if (ColumnsModel is not null)
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

    IModelBlazorColumnWidthMode? ListViewModel { get; }

    IModelBlazorColumnWidthMode? ColumnsModel { get; }

    IModelBlazorColumnWidthMode? ApplicationOptionsModel { get; }
}
