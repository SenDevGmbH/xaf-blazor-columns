using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using DevExpress.ExpressApp.Model;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace SenDev.Xaf.Blazor.Columns.Editors;

public class SenDevTreeListColumnWrapper : DxTreeListColumnWrapper
{
    public SenDevTreeListColumnWrapper(DxTreeListDataColumnModel dxGridDataColumnModel, DxGridListEditorBase gridListEditor) : base(dxGridDataColumnModel, gridListEditor.GridSummary)
    {
        DataColumnModel = dxGridDataColumnModel;
        if (gridListEditor is ISupportsColumnWidthMode editor)
            GridListEditor = editor;
    }

    public DxDataColumnBaseModel DataColumnModel { get; }

    private ISupportsColumnWidthMode GridListEditor { get; }

    public override int Width
    {
        get
        {
            if (GridListEditor?.ColumnWidthMode == ColumnWidthMode.Proportional)
            {
                string widthText = DataColumnModel.Width?.Replace("%", "");
                if (BindConverter.TryConvertToDouble(widthText, CultureInfo.InvariantCulture, out double width))
                    return Convert.ToInt32(Math.Round(width));
            }
            return base.Width;
        }

        set
        {
            if (GridListEditor?.ColumnWidthMode == ColumnWidthMode.Proportional && GridListEditor is DxGridListEditorBase editor)
            {
                int totalWidth = editor.Model.Columns.OfType<IModelColumn>().Where(c => c.Index >= 0).Select(c => c.Width).Sum();
                if (totalWidth > 0)
                    DataColumnModel.Width = value * 100 / totalWidth + "%";
            }
            else
                base.Width = value;
        }
    }

}
