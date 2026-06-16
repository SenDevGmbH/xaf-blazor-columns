using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using SenDev.Xaf.Blazor.Columns.Demo.Blazor.Server.Editors;
using System.ComponentModel;

namespace SenDev.Xaf.Blazor.Columns.Demo.Blazor.Server;

[ToolboxItemFilter("Xaf.Platform.Blazor")]
// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class SenDevXafBlazorColumnsDemoBlazorModule : ModuleBase
{
    public SenDevXafBlazorColumnsDemoBlazorModule()
    {
    }
    public override void Setup(XafApplication application)
    {
        base.Setup(application);
        application.SetupComplete += ApplicationOnSetupComplete;
        application.LoggedOn += ApplicationOnLoggedOn;
    }

    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
    {
        return ModuleUpdater.EmptyModuleUpdaters;
    }


    protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
    {
        base.RegisterEditorDescriptors(editorDescriptorsFactory);
        editorDescriptorsFactory.RegisterListEditorAlias(MyCustomDxGridEditor.Alias, typeof(object), true);
        editorDescriptorsFactory.RegisterListEditor(MyCustomDxGridEditor.Alias, typeof(object), typeof(MyCustomDxGridEditor), false);
    }
}
