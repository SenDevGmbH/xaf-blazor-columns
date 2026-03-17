using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using DevExpress.ExpressApp.Model;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace SenDev.Xaf.Blazor.Columns.Editors;

public class SenDevTreeListColumnWrapper : DxTreeListColumnWrapper
{
    public SenDevTreeListColumnWrapper(DxTreeListDataColumnModel dxTreeListDataColumnModel, SenDevTreeListEditor treeListEditor) : base(dxTreeListDataColumnModel, treeListEditor.GridSummary)
    {
        DataColumnModel = dxTreeListDataColumnModel;
        TreeListEditor = treeListEditor;
    }

    public DxTreeListDataColumnModel DataColumnModel { get; }
    private SenDevTreeListEditor TreeListEditor { get; }

    public override int Width
    {
        get
        {
            if (TreeListEditor.ColumnWidthMode == ColumnWidthMode.Proportional)
            {
                string widthText = DataColumnModel.Width?.Replace("%", "");
                if (BindConverter.TryConvertToDouble(widthText, CultureInfo.InvariantCulture, out double width))
                {
                    return Convert.ToInt32(Math.Round(width));
                }
            }
            return base.Width;
        }

        set
        {
            if (TreeListEditor.ColumnWidthMode == ColumnWidthMode.Proportional)
            {
                int totalWidth = TreeListEditor.Model.Columns.OfType<IModelColumn>().Where(c => c.Index >= 0).Select(c => c.Width).Sum();
                if (totalWidth > 0)
                    DataColumnModel.Width = value * 100 / totalWidth + "%";
            }
            else
                base.Width = value;
        }
    }
}
