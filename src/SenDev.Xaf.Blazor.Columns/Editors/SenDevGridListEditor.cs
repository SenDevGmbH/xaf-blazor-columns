using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using DevExpress.ExpressApp.Model;

namespace SenDev.Xaf.Blazor.Columns.Editors;

public class SenDevGridListEditor : DxGridListEditor, ISupportsColumnWidthMode
{
    public SenDevGridListEditor(IModelListView model) : base(model)
    { }

    protected override DxGridColumnWrapperBase CreateColumnWrapper(DxDataColumnBaseModel dataColumnModel)
        => ColumnWidthEditorSupport.CreateGridColumnWrapper(dataColumnModel, this);

    public IModelBlazorColumnWidthMode? ListViewModel => ColumnWidthEditorSupport.GetListViewModel(this);

    public IModelBlazorColumnWidthMode? ColumnsModel => ColumnWidthEditorSupport.GetColumnsModel(this);

    public IModelBlazorColumnWidthMode? ApplicationOptionsModel => ColumnWidthEditorSupport.GetApplicationOptionsModel(this);

}

