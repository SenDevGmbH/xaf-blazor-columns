using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraRichEdit.API.Native;

namespace SenDev.Xaf.Blazor.Columns.Editors;
public class SenDevGridListEditor : DxGridListEditor
{
    public SenDevGridListEditor(IModelListView model) : base(model)
    {
    }

    

    protected override DxGridColumnWrapperBase CreateColumnWrapper(DxDataColumnBaseModel dataColumnModel)
    {
        return new SenDevGridColumnWrapper((DxGridDataColumnModel)dataColumnModel, this);
        
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

