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
        Assert.Equal(NullToDefault(workItemFields.AreaPath, ConfigForTests.AreaPathDefault), workItem.AreaPath);
        Assert.Equal(workItemFields.TeamProject, workItem.TeamProject);
        Assert.Equal(NullToDefault(workItemFields.IterationPath, ConfigForTests.IterationPathDefault), workItem.IterationPath);
        Assert.Equal(workItemFields.Title, workItem.Title);
        Assert.Equal(NullToDefault(workItemFields.State, ConfigForTests.StateDefault), workItem.State);
        Assert.Equal(workItemFields.WorkItemType, workItem.WorkItemType);
        Assert.Equal(EmptyToNull(workItemFields.AssignedToDisplayName), workItem.AssignedToDisplayName);
        Assert.Equal(EmptyToNull(workItemFields.Activity), workItem.Activity);
        Assert.Equal(EmptyToNull(workItemFields.Description), workItem.Description);
        Assert.Equal(EmptyToNull(workItemFields.ReproSteps), workItem.ReproSteps);
        Assert.Equal(EmptyToNull(workItemFields.SystemInfo), workItem.SystemInfo);
        Assert.Equal(EmptyToNull(workItemFields.AcceptanceCriteria), workItem.AcceptanceCriteria);
    }

    private static string EmptyToNull(string text) {
        return text == "" ? null : text;
    }

    private static string NullToDefault(string text, string defaultText) {
        return text ?? defaultText;
    }
}
