using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using DevExpress.ExpressApp.Model;

namespace SenDev.Xaf.Blazor.Columns.Editors;

public class SenDevTreeListEditor : DxTreeListEditor
{
    public SenDevTreeListEditor(IModelListView model) : base(model)
    {
    }

    protected override DxGridColumnWrapperBase CreateColumnWrapper(DxDataColumnBaseModel dataColumnModel)
    {
        return new SenDevTreeListColumnWrapper((DxTreeListDataColumnModel)dataColumnModel, this);
    }

    public ColumnWidthMode ColumnWidthMode
    {
        get
        {
            if (Model.Columns is IModelBlazorColumnWidthMode columnWidthModeModel)
            {
                var columnWidthMode = columnWidthModeModel.ColumnWidthMode;
                if (columnWidthMode != ColumnWidthMode.Default)
                {
                    return columnWidthMode;
                }
            }

            if (Model.Application.Options is IModelBlazorColumnWidthMode optionsColumnWidthModeModel)
            {
                var columnWidthMode = optionsColumnWidthModeModel.ColumnWidthMode;
                if (columnWidthMode != ColumnWidthMode.Default)
                {
                    return columnWidthMode;
                }
            }

            return ColumnWidthMode.Default;
        }
    }
}
