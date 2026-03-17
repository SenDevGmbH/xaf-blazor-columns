using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.SystemModule;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using SenDev.Xaf.Blazor.Columns.Editors;

namespace SenDev.Xaf.Blazor.Columns;

/// <summary>
/// XAF Module for enhanced column width management in Blazor applications.
/// Provides support for Relative and BestFit column width modes.
/// </summary>
public sealed partial class SenDevXafBlazorColumnsModule : ModuleBase
{
    public SenDevXafBlazorColumnsModule()
    {
        InitializeComponent();
    }


    public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
    {
        base.ExtendModelInterfaces(extenders);
        extenders.Add<IModelOptionsBlazor, IModelBlazorColumnWidthMode>();
        extenders.Add<IModelColumns, IModelBlazorColumnWidthMode>();
    }

    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) => [];

    public override void Setup(XafApplication application)
    {
        base.Setup(application);
    }

    public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
    {
        base.AddGeneratorUpdaters(updaters);
        updaters.Add(new ReplaceDxGridWithSenDevUpdater());
    }
    protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
    {
        base.RegisterEditorDescriptors(editorDescriptorsFactory);
        editorDescriptorsFactory.RegisterListEditorAlias(nameof(SenDevGridListEditor), typeof(object), true);
        editorDescriptorsFactory.RegisterListEditor(nameof(SenDevGridListEditor), typeof(object), typeof(SenDevGridListEditor), true);
        editorDescriptorsFactory.RegisterListEditorAlias(nameof(SenDevTreeListEditor), typeof(object), true);
        editorDescriptorsFactory.RegisterListEditor(nameof(SenDevTreeListEditor), typeof(object), typeof(SenDevTreeListEditor), true);
    }

}
