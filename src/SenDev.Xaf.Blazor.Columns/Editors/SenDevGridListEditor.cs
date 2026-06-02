using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using DevExpress.ExpressApp.Model;

namespace SenDev.Xaf.Blazor.Columns.Editors;

public class SenDevGridListEditor : DxGridListEditor, ISupportsColumnWidthMode
{
    public SenDevGridListEditor(IModelListView model) : base(model)
    { }

    protected override DxGridColumnWrapperBase CreateColumnWrapper(DxDataColumnBaseModel dataColumnModel)
        => new SenDevGridColumnWrapper((DxGridDataColumnModel)dataColumnModel, this);

    public IModelBlazorColumnWidthMode? ColumnsModel => (IModelBlazorColumnWidthMode)Model.Columns;

    public IModelBlazorColumnWidthMode? ApplicationOptionsModel => (IModelBlazorColumnWidthMode)Model.Application.Options;

}

