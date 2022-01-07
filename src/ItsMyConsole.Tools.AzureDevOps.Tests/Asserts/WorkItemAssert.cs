using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;

public static class WorkItemAssert
{
    public static void CheckNew(WorkItem workItem) {
        Assert.NotNull(workItem);
        Assert.True(workItem.Id > 0);
        Assert.Null(workItem.Childs);
        Assert.Null(workItem.Parents);
        Assert.Null(workItem.Related);
        Assert.False(workItem.IsFixedInChangeset);
        Assert.Null(workItem.Tags);
    }

    public static void Equal(WorkItemFields workItemFields, WorkItem workItem) {
        Assert.Equal(workItemFields.AreaPath, workItem.AreaPath);
        Assert.Equal(workItemFields.TeamProject, workItem.TeamProject);
        Assert.Equal(workItemFields.IterationPath, workItem.IterationPath);
        Assert.Equal(workItemFields.Title, workItem.Title);
        Assert.Equal(workItemFields.State, workItem.State);
        Assert.Equal(workItemFields.WorkItemType, workItem.WorkItemType);
        Assert.Equal(workItemFields.AssignedToDisplayName, workItem.AssignedToDisplayName);
        Assert.Equal(workItemFields.Activity, workItem.Activity);
        Assert.Equal(workItemFields.Description, workItem.Description);
        Assert.Equal(workItemFields.ReproSteps, workItem.ReproSteps);
        Assert.Equal(workItemFields.SystemInfo, workItem.SystemInfo);
        Assert.Equal(workItemFields.AcceptanceCriteria, workItem.AcceptanceCriteria);
    }
}
