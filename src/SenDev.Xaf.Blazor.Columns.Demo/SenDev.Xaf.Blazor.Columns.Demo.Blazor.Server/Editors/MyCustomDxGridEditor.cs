using DevExpress.Blazor.Grid.Internal.Base;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using System.Reflection;

namespace SenDev.Xaf.Blazor.Columns.Demo.Blazor.Server.Editors;

[ListEditor(typeof(object), false)]
public class MyCustomDxGridEditor : DxGridListEditor
{
    public const string Alias = "MyCustomDxGridEditor";

    public MyCustomDxGridEditor(IModelListView model) : base(model)
    {
    }

    protected override object CreateControlsCore()
    {
        var gridComponentModel = (DxGridModel)base.CreateControlsCore();
        var customModel = new MyCustomDxGridEditorComponentModel {
            HeaderText = "My custom grid from model"
        };

        CopyGridModel(gridComponentModel, customModel);
        ComponentAdapter = new DxGridAdapter(customModel);
        return customModel;
    }

    private static void CopyGridModel(DxGridModel source, DxGridModel target)
    {
        foreach (var property in source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || !property.CanWrite || property.GetIndexParameters().Length > 0 || property.Name == "ComponentInstance")
            {
                continue;
            }

            var targetProperty = target.GetType().GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public);
            if (targetProperty is null || !targetProperty.CanWrite || targetProperty.GetIndexParameters().Length > 0)
            {
                continue;
            }

            if (!targetProperty.PropertyType.IsAssignableFrom(property.PropertyType))
            {
                continue;
            }

            var value = property.GetValue(source);
            targetProperty.SetValue(target, value);
        }
    }
}
