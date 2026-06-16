using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Model;
using SenDev.Xaf.Blazor.Columns.Editors;

namespace SenDev.Xaf.Blazor.Columns;

internal static class BlazorColumnWidthEditorModelUpdater
{
    public static void Apply(IModelApplication? model)
    {
        if (model is null)
        {
            return;
        }

        Apply(model.Views);
    }

    public static void Apply(IModelViews views)
    {
        ReplaceDefaultListEditor(views);

        foreach (var listView in views.OfType<IModelListView>())
        {
            ReplaceEditor(listView);
        }
    }

    private static void ReplaceDefaultListEditor(IModelViews views)
    {
        var replacementType = ColumnWidthEditorTypeResolver.GetReplacementEditorType(GetEditorType(views, "DefaultListEditor"));
        if (replacementType is not null)
        {
            SetEditorType(views, "DefaultListEditor", replacementType);
        }
    }

    private static void ReplaceEditor(IModelListView listView)
    {
        var editorType = listView.EditorType ?? GetEditorType(listView, "EditorTypeName");
        var replacementType = ColumnWidthEditorTypeResolver.GetReplacementEditorType(editorType);
        if (replacementType is not null)
        {
            listView.EditorType = replacementType;
            SetEditorType(listView, "EditorTypeName", replacementType);
        }
    }

    private static Type? GetEditorType(object modelNode, string propertyName)
    {
        var propertyValue = GetPropertyValue(modelNode, propertyName);
        return propertyValue switch
        {
            Type editorType => editorType,
            string editorTypeName => ResolveEditorType(editorTypeName),
            _ => null
        };
    }

    private static void SetEditorType(object modelNode, string propertyName, Type replacementType)
    {
        var property = modelNode.GetType().GetProperty(propertyName);
        if (property is null || !property.CanWrite)
        {
            return;
        }

        if (property.PropertyType.IsInstanceOfType(replacementType))
        {
            property.SetValue(modelNode, replacementType);
            return;
        }

        if (property.PropertyType == typeof(string))
        {
            var replacementName = GetRegisteredReplacementEditorTypeName(replacementType);
            if (replacementName is not null)
            {
                property.SetValue(modelNode, replacementName);
            }
        }
    }

    private static string? GetRegisteredReplacementEditorTypeName(Type editorType)
        => editorType == typeof(SenDevGridListEditor)
            ? typeof(SenDevGridListEditor).FullName
            : editorType == typeof(SenDevTreeListEditor)
                ? typeof(SenDevTreeListEditor).FullName
                : null;

    private static object? GetPropertyValue(object modelNode, string propertyName)
        => modelNode.GetType().GetProperty(propertyName)?.GetValue(modelNode);

    private static Type? ResolveEditorType(string? editorTypeName)
    {
        if (string.IsNullOrWhiteSpace(editorTypeName))
        {
            return null;
        }

        var editorType = Type.GetType(editorTypeName, throwOnError: false);
        if (editorType is not null)
        {
            return editorType;
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            editorType = assembly.GetType(editorTypeName, throwOnError: false);
            if (editorType is not null)
            {
                return editorType;
            }
        }

        return editorTypeName switch
        {
            nameof(DxGridListEditor) => typeof(DxGridListEditor),
            nameof(DxTreeListEditor) => typeof(DxTreeListEditor),
            nameof(SenDevGridListEditor) => typeof(SenDevGridListEditor),
            nameof(SenDevTreeListEditor) => typeof(SenDevTreeListEditor),
            _ => null
        };
    }
}
