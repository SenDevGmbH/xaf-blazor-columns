using DevExpress.ExpressApp.Blazor.Editors;

namespace SenDev.Xaf.Blazor.Columns.Editors;

internal static class ColumnWidthEditorTypeResolver
{
    public static Type? GetReplacementEditorType(Type? editorType)
    {
        if (editorType is null || ColumnWidthEditorAdapterFactory.IsGeneratedAdapterType(editorType))
        {
            return null;
        }

        if (typeof(SenDevTreeListEditor).IsAssignableFrom(editorType) ||
            typeof(SenDevGridListEditor).IsAssignableFrom(editorType))
        {
            return null;
        }

        if (editorType == typeof(DxTreeListEditor))
        {
            return typeof(SenDevTreeListEditor);
        }

        if (editorType == typeof(DxGridListEditor))
        {
            return typeof(SenDevGridListEditor);
        }

        if (typeof(DxTreeListEditor).IsAssignableFrom(editorType))
        {
            return ColumnWidthEditorAdapterFactory.GetTreeAdapterType(editorType);
        }

        if (typeof(DxGridListEditor).IsAssignableFrom(editorType))
        {
            return ColumnWidthEditorAdapterFactory.GetGridAdapterType(editorType);
        }

        return null;
    }
}
