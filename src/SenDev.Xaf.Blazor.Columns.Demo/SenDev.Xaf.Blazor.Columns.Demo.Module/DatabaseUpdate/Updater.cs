using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DemoGridColumnPixelSample = SenDev.Xaf.Blazor.Columns.Demo.Module.BusinessObjects.GridColumnPixelSample;
using DemoGridColumnProportionalSample = SenDev.Xaf.Blazor.Columns.Demo.Module.BusinessObjects.GridColumnProportionalSample;
using DemoSecondTask = SenDev.Xaf.Blazor.Columns.Demo.Module.BusinessObjects.TreeListColumnProportionalSample;
using DemoTask = SenDev.Xaf.Blazor.Columns.Demo.Module.BusinessObjects.TreeListColumnPixelSample;

namespace SenDev.Xaf.Blazor.Columns.Demo.Module.DatabaseUpdate;
// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater
{
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion)
    {
    }
    public override void UpdateDatabaseAfterUpdateSchema()
    {
        base.UpdateDatabaseAfterUpdateSchema();

        SeedTasks();
        SeedSecondTasks();
        SeedGridColumnPixelSamples();
        SeedGridColumnProportionalSamples();

        ObjectSpace.CommitChanges();
    }

    void SeedTasks()
    {
        if (ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Name = ?", "Demo project")) != null)
        {
            return;
        }

        DateTime today = DateTime.Today;

        DemoTask demoProject = CreateTask(
            "Demo project",
            null,
            today,
            today.AddDays(14),
            35,
            "Root task used to test tree rendering.");

        DemoTask planning = CreateTask(
            "Planning",
            demoProject,
            today,
            today.AddDays(2),
            100,
            "Completed planning branch.");

        CreateTask(
            "Requirements",
            planning,
            today,
            today.AddDays(1),
            100,
            "Collect requirements for the tree demo.");

        CreateTask(
            "Column layout",
            planning,
            today.AddDays(1),
            today.AddDays(2),
            100,
            "Prepare columns for visual checks.");

        DemoTask implementation = CreateTask(
            "Implementation",
            demoProject,
            today.AddDays(3),
            today.AddDays(10),
            25,
            "Active implementation branch.");

        CreateTask(
            "Persistent object",
            implementation,
            today.AddDays(3),
            today.AddDays(4),
            100,
            "Implement the XPO object.");

        CreateTask(
            "Tree view smoke test",
            implementation,
            today.AddDays(5),
            today.AddDays(7),
            20,
            "Verify that child rows expand under their parents.");

        CreateTask(
            "Release",
            demoProject,
            today.AddDays(11),
            today.AddDays(14),
            0,
            "Pending release branch.");
    }

    DemoTask CreateTask(string name, DemoTask parent, DateTime startDate, DateTime dueDate, int percentComplete, string description)
    {
        DemoTask task = ObjectSpace.CreateObject<DemoTask>();
        task.Name = name;
        task.Parent = parent;
        task.StartDate = startDate;
        task.DueDate = dueDate;
        task.PercentComplete = percentComplete;
        task.Description = description;
        return task;
    }

    void SeedGridColumnPixelSamples()
    {
        if (ObjectSpace.FindObject<DemoGridColumnPixelSample>(CriteriaOperator.Parse("Title = ?", "Pixel grid baseline")) != null)
        {
            return;
        }

        DateTime today = DateTime.Today;

        CreateGridColumnPixelSample("Pixel grid baseline", "Meyer", "Backlog", today.AddDays(-7), today.AddDays(3), 2, 1250m, false, "Short text used to check compact pixel columns.");
        CreateGridColumnPixelSample("Long title with many words for width testing", "Schmidt", "Analysis", today.AddDays(-5), today.AddDays(5), 1, 4900m, false, "Longer notes verify that the grid keeps configured pixel widths instead of expanding every text column.");
        CreateGridColumnPixelSample("Blocked integration task", "Keller", "Implementation", today.AddDays(-2), today.AddDays(8), 3, 8700m, true, "Boolean column and money column should stay readable next to a wide notes column.");
        CreateGridColumnPixelSample("QA smoke check", "Fischer", "Validation", today, today.AddDays(10), 4, 820m, false, "Small currency value and medium-length owner name.");
        CreateGridColumnPixelSample("Release approval", "Weber", "Approval", today.AddDays(1), today.AddDays(12), 1, 2300m, false, "Final row for a normal one-line comment.");
        CreateGridColumnPixelSample("Documentation refresh", "Wagner", "Documentation", today.AddDays(2), today.AddDays(14), 5, 610m, true, "Another blocked row with a narrow priority column.");
    }

    DemoGridColumnPixelSample CreateGridColumnPixelSample(string title, string owner, string phase, DateTime startDate, DateTime dueDate, int priority, decimal estimatedCost, bool blocked, string notes)
    {
        DemoGridColumnPixelSample sample = ObjectSpace.CreateObject<DemoGridColumnPixelSample>();
        sample.Title = title;
        sample.Owner = owner;
        sample.Phase = phase;
        sample.StartDate = startDate;
        sample.DueDate = dueDate;
        sample.Priority = priority;
        sample.EstimatedCost = estimatedCost;
        sample.Blocked = blocked;
        sample.Notes = notes;
        return sample;
    }

    void SeedSecondTasks()
    {
        if (ObjectSpace.FindObject<DemoSecondTask>(CriteriaOperator.Parse("Name = ?", "Second demo project")) != null)
        {
            return;
        }

        DateTime today = DateTime.Today;

        DemoSecondTask secondDemoProject = CreateSecondTask(
            "Second demo project",
            null,
            today,
            today.AddDays(21),
            40,
            "Root SecondTask used to test a second tree list.");

        DemoSecondTask discovery = CreateSecondTask(
            "Discovery",
            secondDemoProject,
            today,
            today.AddDays(4),
            100,
            "Completed discovery branch for the second tree.");

        CreateSecondTask(
            "Sample data design",
            discovery,
            today,
            today.AddDays(2),
            100,
            "Define representative rows and nesting depth.");

        CreateSecondTask(
            "Navigation check",
            discovery,
            today.AddDays(2),
            today.AddDays(4),
            100,
            "Verify the SecondTask navigation item opens the expected list.");

        DemoSecondTask validation = CreateSecondTask(
            "Validation",
            secondDemoProject,
            today.AddDays(5),
            today.AddDays(14),
            35,
            "Branch for validating expand/collapse behavior.");

        CreateSecondTask(
            "Collapsed parent",
            validation,
            today.AddDays(5),
            today.AddDays(8),
            50,
            "Parent row with children for expansion testing.");

        DemoSecondTask nestedBranch = CreateSecondTask(
            "Nested branch",
            validation,
            today.AddDays(9),
            today.AddDays(12),
            20,
            "Intermediate node to verify deeper tree levels.");

        CreateSecondTask(
            "Leaf A",
            nestedBranch,
            today.AddDays(10),
            today.AddDays(11),
            10,
            "First leaf under a nested SecondTask branch.");

        CreateSecondTask(
            "Leaf B",
            nestedBranch,
            today.AddDays(11),
            today.AddDays(12),
            0,
            "Second leaf under a nested SecondTask branch.");

        CreateSecondTask(
            "Publish results",
            secondDemoProject,
            today.AddDays(15),
            today.AddDays(21),
            0,
            "Final branch for the second test tree.");
    }

    DemoSecondTask CreateSecondTask(string name, DemoSecondTask parent, DateTime startDate, DateTime dueDate, int percentComplete, string description)
    {
        DemoSecondTask task = ObjectSpace.CreateObject<DemoSecondTask>();
        task.Name = name;
        task.Parent = parent;
        task.StartDate = startDate;
        task.DueDate = dueDate;
        task.PercentComplete = percentComplete;
        task.Description = description;
        return task;
    }

    void SeedGridColumnProportionalSamples()
    {
        if (ObjectSpace.FindObject<DemoGridColumnProportionalSample>(CriteriaOperator.Parse("Title = ?", "Proportional grid baseline")) != null)
        {
            return;
        }

        DateTime today = DateTime.Today;

        CreateGridColumnProportionalSample("Proportional grid baseline", "Acme", "North", today.AddDays(-10), 12, 24800m, 0.31m, true, "Baseline row for percentage-based grid column widths.");
        CreateGridColumnProportionalSample("High revenue account with long title", "Contoso", "West", today.AddDays(-8), 48, 131500m, 0.27m, false, "A long text value makes proportional resizing easier to inspect.");
        CreateGridColumnProportionalSample("Low volume follow-up", "Northwind", "South", today.AddDays(-6), 5, 4200m, 0.12m, true, "Small numeric values should not claim too much horizontal space.");
        CreateGridColumnProportionalSample("Expansion candidate", "Fabrikam", "East", today.AddDays(-4), 22, 37200m, 0.22m, false, "Useful for comparing relative Title, Account, and Comment column widths.");
        CreateGridColumnProportionalSample("Renewal review", "Adventure Works", "Central", today.AddDays(-2), 17, 18900m, 0.18m, true, "Reviewed rows include a checked boolean cell.");
        CreateGridColumnProportionalSample("Unreviewed exception", "Tailspin", "International", today, 9, 9600m, 0.09m, false, "The region text is intentionally longer than most values.");
    }

    DemoGridColumnProportionalSample CreateGridColumnProportionalSample(string title, string account, string region, DateTime reportDate, int itemCount, decimal revenue, decimal margin, bool reviewed, string comment)
    {
        DemoGridColumnProportionalSample sample = ObjectSpace.CreateObject<DemoGridColumnProportionalSample>();
        sample.Title = title;
        sample.Account = account;
        sample.Region = region;
        sample.ReportDate = reportDate;
        sample.ItemCount = itemCount;
        sample.Revenue = revenue;
        sample.Margin = margin;
        sample.Reviewed = reviewed;
        sample.Comment = comment;
        return sample;
    }

    public override void UpdateDatabaseBeforeUpdateSchema()
    {
        base.UpdateDatabaseBeforeUpdateSchema();
        //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
        //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
        //}
    }
}
