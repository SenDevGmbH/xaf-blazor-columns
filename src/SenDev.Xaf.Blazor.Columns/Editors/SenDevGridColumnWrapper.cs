using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;

namespace SenDev.Xaf.Blazor.Columns.Editors;

public class SenDevGridColumnWrapper : DxGridColumnWrapper
{
    public SenDevGridColumnWrapper(DxGridDataColumnModel dxGridDataColumnModel, DxGridListEditorBase gridListEditor) : base(dxGridDataColumnModel, gridListEditor.GridSummary)
    {
        if (gridListEditor is ISupportsColumnWidthMode editor)
            GridListEditor = editor;
    }

    private ISupportsColumnWidthMode GridListEditor { get; }

    public override int Width
    {
        get => ColumnWidthHelper.GetWidth(DxGridDataColumnModel, GridListEditor, base.Width);

        set
        {
            if (!ColumnWidthHelper.TrySetProportionalWidth(DxGridDataColumnModel, GridListEditor, value))
                base.Width = value;
        }
    }

}
