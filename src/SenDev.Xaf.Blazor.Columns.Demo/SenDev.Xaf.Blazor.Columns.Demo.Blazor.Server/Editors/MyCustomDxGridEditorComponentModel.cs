using DevExpress.ExpressApp.Blazor.Editors.Models;

namespace SenDev.Xaf.Blazor.Columns.Demo.Blazor.Server.Editors;

public class MyCustomDxGridEditorComponentModel : DxGridModel
{
    public override Type ComponentType => typeof(MyCustomDxGridEditorComponent);

    public string HeaderText {
        get => GetPropertyValue<string>();
        set => SetPropertyValue(value);
    }
}
