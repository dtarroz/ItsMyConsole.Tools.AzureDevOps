using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;

public static class WorkItemAssert
{
    public static void CheckNew(WorkItemFields workItemFields, WorkItem workItem) {
        Assert.NotNull(workItem);
        Assert.Matches($"^{ConfigForTests.ServerUrl}.*/_apis/wit/workItems/{workItem.Id}$", workItem.Url);
        Assert.True(workItem.Id > 0);
        Assert.Null(workItem.Childs);
        Assert.Null(workItem.Parents);
        Assert.Null(workItem.Related);
        Assert.False(workItem.IsFixedInChangeset);
        Assert.Null(workItem.Tags);
        EqualNew(workItemFields, workItem);
    }

    private static void EqualNew(WorkItemFields workItemFields, WorkItem workItem) {
        Assert.NotNull(workItemFields);
        Assert.NotNull(workItem);
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

    public static void CheckUpdate(WorkItem workItem, WorkItemFields workItemFields, WorkItem workItemUpdate) {
        Assert.NotNull(workItem);
        Assert.NotNull(workItemFields);
        Assert.NotNull(workItemUpdate);
        Assert.Equal(workItem.Url, workItemUpdate.Url);
        Assert.Equal(workItem.Id, workItemUpdate.Id);
        Assert.Null(workItemUpdate.Childs);
        Assert.Null(workItemUpdate.Parents);
        Assert.Null(workItemUpdate.Related);
        Assert.False(workItemUpdate.IsFixedInChangeset);
        Assert.Null(workItemUpdate.Tags);
        WorkItemFields workItemFieldsUpdate = new WorkItemFields {
            AreaPath = workItemFields.AreaPath ?? workItem.AreaPath,
            TeamProject = workItemFields.TeamProject ?? workItem.TeamProject,
            IterationPath = workItemFields.IterationPath ?? workItem.IterationPath,
            Title = workItemFields.Title ?? workItem.Title,
            State = workItemFields.State ?? workItem.State,
            WorkItemType = workItemFields.WorkItemType ?? workItem.WorkItemType,
            AssignedToDisplayName = workItemFields.AssignedToDisplayName ?? workItem.AssignedToDisplayName,
            Activity = workItemFields.Activity ?? workItem.Activity,
            Description = workItemFields.Description ?? workItem.Description,
            ReproSteps = workItemFields.ReproSteps ?? workItem.ReproSteps,
            SystemInfo = workItemFields.SystemInfo ?? workItem.SystemInfo,
            AcceptanceCriteria = workItemFields.AcceptanceCriteria ?? workItem.AcceptanceCriteria
        };
        EqualUpdate(workItemFieldsUpdate, workItemUpdate);
    }

    private static void EqualUpdate(WorkItemFields workItemFields, WorkItem workItem) {
        Assert.NotNull(workItemFields);
        Assert.NotNull(workItem);
        Assert.Equal(workItemFields.AreaPath, workItem.AreaPath);
        Assert.Equal(workItemFields.TeamProject, workItem.TeamProject);
        Assert.Equal(workItemFields.IterationPath, workItem.IterationPath);
        Assert.Equal(workItemFields.Title, workItem.Title);
        Assert.Equal(workItemFields.State, workItem.State);
        Assert.Equal(workItemFields.WorkItemType, workItem.WorkItemType);
        Assert.Equal(EmptyToNull(workItemFields.AssignedToDisplayName), workItem.AssignedToDisplayName);
        Assert.Equal(EmptyToNull(workItemFields.Activity), workItem.Activity);
        Assert.Equal(workItemFields.Description, workItem.Description);
        Assert.Equal(workItemFields.ReproSteps, workItem.ReproSteps);
        Assert.Equal(workItemFields.SystemInfo, workItem.SystemInfo);
        Assert.Equal(workItemFields.AcceptanceCriteria, workItem.AcceptanceCriteria);
    }
}
