using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using DevExpress.ExpressApp.Model;

namespace SenDev.Xaf.Blazor.Columns.Editors;

public class SenDevTreeListEditor : DxTreeListEditor, ISupportsColumnWidthMode
{
    public SenDevTreeListEditor(IModelListView model) : base(model)
    { }

    protected override DxGridColumnWrapperBase CreateColumnWrapper(DxDataColumnBaseModel dataColumnModel)
        => new SenDevTreeListColumnWrapper((DxTreeListDataColumnModel)dataColumnModel, this);

    public IModelBlazorColumnWidthMode? ColumnsModel => (IModelBlazorColumnWidthMode)Model.Columns;

    public IModelBlazorColumnWidthMode? ApplicationOptionsModel => (IModelBlazorColumnWidthMode)Model.Application.Options;

}

