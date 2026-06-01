using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SenDev.Xaf.Blazor.Columns.Demo.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(Name))]
[ImageName("BO_Task")]
[NavigationItem("Demo")]
public class SecondTask : BaseObject, ITreeNode
{
    public SecondTask(Session session) : base(session)
    {
    }

    string name;
    [Size(255)]
    public string Name
    {
        get => name;
        set => SetPropertyValue(nameof(Name), ref name, value);
    }

    string description;
    [Size(SizeAttribute.Unlimited)]
    public string Description
    {
        get => description;
        set => SetPropertyValue(nameof(Description), ref description, value);
    }

    DateTime startDate;
    public DateTime StartDate
    {
        get => startDate;
        set => SetPropertyValue(nameof(StartDate), ref startDate, value);
    }

    DateTime dueDate;
    public DateTime DueDate
    {
        get => dueDate;
        set => SetPropertyValue(nameof(DueDate), ref dueDate, value);
    }

    int percentComplete;
    public int PercentComplete
    {
        get => percentComplete;
        set => SetPropertyValue(nameof(PercentComplete), ref percentComplete, value);
    }

    SecondTask parent;
    [Association("SecondTask-Children")]
    public SecondTask Parent
    {
        get => parent;
        set => SetPropertyValue(nameof(Parent), ref parent, value);
    }

    [Association("SecondTask-Children")]
    [Aggregated]
    public XPCollection<SecondTask> Children => GetCollection<SecondTask>(nameof(Children));

    ITreeNode ITreeNode.Parent => Parent;

    IBindingList ITreeNode.Children => Children;
}
