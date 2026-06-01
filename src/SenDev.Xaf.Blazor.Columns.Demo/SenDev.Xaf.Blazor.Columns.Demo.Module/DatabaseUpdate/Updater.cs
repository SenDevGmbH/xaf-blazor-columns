using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Microsoft.Extensions.DependencyInjection;
using DemoSecondTask = SenDev.Xaf.Blazor.Columns.Demo.Module.BusinessObjects.SecondTask;
using DemoTask = SenDev.Xaf.Blazor.Columns.Demo.Module.BusinessObjects.Task;

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

    public override void UpdateDatabaseBeforeUpdateSchema()
    {
        base.UpdateDatabaseBeforeUpdateSchema();
        //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
        //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
        //}
    }
}
