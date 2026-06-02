using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using SenDev.Xaf.Blazor.Columns.Editors;

namespace SenDev.Xaf.Blazor.Columns;

public sealed class ReplaceDxGridWithSenDevUpdater : ModelNodesGeneratorUpdater<DevExpress.ExpressApp.Model.NodeGenerators.ModelViewsNodesGenerator>
{
    public override void UpdateNode(ModelNode node)
    {
        var viewsModel = (IModelViews)node;
        foreach (var lv in viewsModel.OfType<IModelListView>())
        {
            if (lv.EditorType != null && lv.EditorType == typeof(DxGridListEditor))
                lv.EditorType = typeof(SenDevGridListEditor);
            if (lv.EditorType != null && lv.EditorType == typeof(DxTreeListEditor))
                lv.EditorType = typeof(SenDevTreeListEditor);
        }
    }

}
