using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;

namespace SenDev.Xaf.Blazor.Columns.Editors;

public static class ColumnWidthEditorSupport
{
    public static IModelBlazorColumnWidthMode? GetListViewModel(DxGridListEditorBase editor)
        => editor.Model as IModelBlazorColumnWidthMode;

    public static IModelBlazorColumnWidthMode? GetColumnsModel(DxGridListEditorBase editor)
        => editor.Model.Columns as IModelBlazorColumnWidthMode;

    public static IModelBlazorColumnWidthMode? GetApplicationOptionsModel(DxGridListEditorBase editor)
        => editor.Model.Application.Options as IModelBlazorColumnWidthMode;

    public static DxGridColumnWrapperBase CreateGridColumnWrapper(DxDataColumnBaseModel dataColumnModel, DxGridListEditorBase editor)
        => new SenDevGridColumnWrapper((DxGridDataColumnModel)dataColumnModel, editor);

    public static DxGridColumnWrapperBase CreateTreeListColumnWrapper(DxDataColumnBaseModel dataColumnModel, DxGridListEditorBase editor)
        => new SenDevTreeListColumnWrapper((DxTreeListDataColumnModel)dataColumnModel, editor);
}
