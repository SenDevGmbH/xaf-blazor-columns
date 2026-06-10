using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using System.ComponentModel;

namespace SenDev.Xaf.Blazor.Columns.Demo.Blazor.Server;

[ToolboxItemFilter("Xaf.Platform.Blazor")]
// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class SenDevXafBlazorColumnsDemoBlazorModule : ModuleBase
{
    public SenDevXafBlazorColumnsDemoBlazorModule()
    {
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
    {
        return ModuleUpdater.EmptyModuleUpdaters;
    }
    public override void Setup(XafApplication application)
    {
        base.Setup(application);
    }
}
