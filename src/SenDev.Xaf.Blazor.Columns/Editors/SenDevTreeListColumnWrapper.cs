using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;

namespace SenDev.Xaf.Blazor.Columns.Editors;

public class SenDevTreeListColumnWrapper : DxTreeListColumnWrapper
{
    public SenDevTreeListColumnWrapper(DxTreeListDataColumnModel dxGridDataColumnModel, DxGridListEditorBase gridListEditor) : base(dxGridDataColumnModel, gridListEditor.GridSummary)
    {
        if (gridListEditor is ISupportsColumnWidthMode editor)
            GridListEditor = editor;
    }

    private ISupportsColumnWidthMode GridListEditor { get; }

    public override int Width
    {
        get => ColumnWidthHelper.GetWidth(DxTreeListDataColumnModel, GridListEditor, base.Width);

        set
        {
            if (!ColumnWidthHelper.TrySetProportionalWidth(DxTreeListDataColumnModel, GridListEditor, value))
                base.Width = value;
        }
    }

}
