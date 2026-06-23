using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;

namespace SenDev.Xaf.Blazor.Columns.Demo.Blazor.Server.Editors;

public partial class MyCustomDxGridEditorComponent : DxGrid
{

    
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        var temp = Model;
        var temp2 = ViewModel;


    }

}
