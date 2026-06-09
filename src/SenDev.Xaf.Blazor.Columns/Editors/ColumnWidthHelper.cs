using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using DevExpress.ExpressApp.Model;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace SenDev.Xaf.Blazor.Columns.Editors;

internal static class ColumnWidthHelper
{
    public static int GetWidth(DxDataColumnBaseModel dataColumnModel, ISupportsColumnWidthMode? gridListEditor, int baseWidth)
    {
        if (gridListEditor?.ColumnWidthMode == ColumnWidthMode.Proportional)
        {
            string widthText = dataColumnModel.Width?.Replace("%", "");
            if (BindConverter.TryConvertToDouble(widthText, CultureInfo.InvariantCulture, out double width))
                return Convert.ToInt32(Math.Round(width));
        }

        return baseWidth;
    }

    public static bool TrySetProportionalWidth(DxDataColumnBaseModel dataColumnModel, ISupportsColumnWidthMode? gridListEditor, int value)
    {
        if (gridListEditor?.ColumnWidthMode == ColumnWidthMode.Proportional && gridListEditor is DxGridListEditorBase editor)
        {
            int totalWidth = editor.Model.Columns.OfType<IModelColumn>().Where(c => c.Index >= 0).Select(c => c.Width).Sum();
            if (totalWidth > 0)
            {
                dataColumnModel.Width = value * 100 / totalWidth + "%";
            }

            return true;
        }

        return false;
    }
}
