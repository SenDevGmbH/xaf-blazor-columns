using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SenDev.Xaf.Blazor.Columns.Demo.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(Title))]
[ImageName("BO_Task")]
[NavigationItem("Demo")]
public class GridColumnProportionalSample : BaseObject
{
    public GridColumnProportionalSample(Session session) : base(session)
    {
    }

    string title;
    [Size(255)]
    public string Title
    {
        get => title;
        set => SetPropertyValue(nameof(Title), ref title, value);
    }

    string account;
    [Size(100)]
    public string Account
    {
        get => account;
        set => SetPropertyValue(nameof(Account), ref account, value);
    }

    string region;
    [Size(100)]
    public string Region
    {
        get => region;
        set => SetPropertyValue(nameof(Region), ref region, value);
    }

    DateTime reportDate;
    public DateTime ReportDate
    {
        get => reportDate;
        set => SetPropertyValue(nameof(ReportDate), ref reportDate, value);
    }

    int itemCount;
    public int ItemCount
    {
        get => itemCount;
        set => SetPropertyValue(nameof(ItemCount), ref itemCount, value);
    }

    decimal revenue;
    public decimal Revenue
    {
        get => revenue;
        set => SetPropertyValue(nameof(Revenue), ref revenue, value);
    }

    decimal margin;
    public decimal Margin
    {
        get => margin;
        set => SetPropertyValue(nameof(Margin), ref margin, value);
    }

    bool reviewed;
    public bool Reviewed
    {
        get => reviewed;
        set => SetPropertyValue(nameof(Reviewed), ref reviewed, value);
    }

    string comment;
    [Size(SizeAttribute.Unlimited)]
    public string Comment
    {
        get => comment;
        set => SetPropertyValue(nameof(Comment), ref comment, value);
    }
}
