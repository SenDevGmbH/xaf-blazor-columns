using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SenDev.Xaf.Blazor.Columns.Demo.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(Title))]
[ImageName("BO_Task")]
[NavigationItem("Demo")]
public class GridColumnPixelSample : BaseObject
{
    public GridColumnPixelSample(Session session) : base(session)
    {
    }

    string title;
    [Size(255)]
    public string Title
    {
        get => title;
        set => SetPropertyValue(nameof(Title), ref title, value);
    }

    string owner;
    [Size(100)]
    public string Owner
    {
        get => owner;
        set => SetPropertyValue(nameof(Owner), ref owner, value);
    }

    string phase;
    [Size(100)]
    public string Phase
    {
        get => phase;
        set => SetPropertyValue(nameof(Phase), ref phase, value);
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

    int priority;
    public int Priority
    {
        get => priority;
        set => SetPropertyValue(nameof(Priority), ref priority, value);
    }

    decimal estimatedCost;
    public decimal EstimatedCost
    {
        get => estimatedCost;
        set => SetPropertyValue(nameof(EstimatedCost), ref estimatedCost, value);
    }

    bool blocked;
    public bool Blocked
    {
        get => blocked;
        set => SetPropertyValue(nameof(Blocked), ref blocked, value);
    }

    string notes;
    [Size(SizeAttribute.Unlimited)]
    public string Notes
    {
        get => notes;
        set => SetPropertyValue(nameof(Notes), ref notes, value);
    }
}
