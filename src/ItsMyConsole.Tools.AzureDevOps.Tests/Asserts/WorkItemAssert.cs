using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;

public static class WorkItemAssert
{
    public static void CheckCreate(WorkItemCreateFields workItemCreateFields, WorkItem workItem) {
        Assert.NotNull(workItem);
        Assert.Matches($"^{ConfigForTests.ServerUrl}.*/[_]apis/wit/workItems/{workItem.Id}$", workItem.Url);
        Assert.True(workItem.Id > 0);
        CheckRelations(null, workItem);
        Assert.False(workItem.IsFixedInChangeset);
        EqualNew(workItemCreateFields, workItem);
    }

    private static void EqualNew(WorkItemCreateFields workItemCreateFields, WorkItem workItem) {
        Assert.NotNull(workItemCreateFields);
        Assert.NotNull(workItem);
        Assert.Equal(NullToDefault(workItemCreateFields.AreaPath, ConfigForTests.AreaPathDefault), workItem.AreaPath);
        Assert.Equal(workItemCreateFields.TeamProject, workItem.TeamProject);
        Assert.Equal(NullToDefault(workItemCreateFields.IterationPath, ConfigForTests.IterationPathDefault), workItem.IterationPath);
        Assert.Equal(workItemCreateFields.Title, workItem.Title);
        Assert.Equal(NullToDefault(workItemCreateFields.State, ConfigForTests.StateDefault), workItem.State);
        Assert.Equal(workItemCreateFields.WorkItemType, workItem.WorkItemType);
        Assert.Equal(EmptyToNull(workItemCreateFields.AssignedToDisplayName), workItem.AssignedToDisplayName);
        Assert.Equal(EmptyToNull(workItemCreateFields.Activity), workItem.Activity);
        Assert.Equal(EmptyToNull(workItemCreateFields.Description), workItem.Description);
        Assert.Equal(EmptyToNull(workItemCreateFields.ReproSteps), workItem.ReproSteps);
        Assert.Equal(EmptyToNull(workItemCreateFields.SystemInfo), workItem.SystemInfo);
        Assert.Equal(EmptyToNull(workItemCreateFields.AcceptanceCriteria), workItem.AcceptanceCriteria);
        if (workItemCreateFields.Tags == null || !workItemCreateFields.Tags.Any())
            Assert.Null(workItem.Tags);
        else
            Assert.Equal(CleanTags(workItemCreateFields.Tags), workItem.Tags);
    }

    private static IEnumerable<string> CleanTags(IEnumerable<string> tags) {
        return tags.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.ToUpper().Trim()).Distinct().OrderBy(t => t);
    }

    private static string EmptyToNull(string text) {
        return text == "" ? null : text;
    }

    private static string NullToDefault(string text, string defaultText) {
        return text ?? defaultText;
    }

    public static void CheckUpdate(WorkItem workItem, WorkItemUpdateFields workItemUpdateFields, WorkItem workItemUpdate) {
        Assert.NotNull(workItem);
        Assert.NotNull(workItemUpdateFields);
        Assert.NotNull(workItemUpdate);
        Assert.Equal(workItem.Url, workItemUpdate.Url);
        Assert.Equal(workItem.Id, workItemUpdate.Id);
        Assert.Equal(workItem.Children, workItemUpdate.Children);
        Assert.Equal(workItem.Parent, workItemUpdate.Parent);
        Assert.Equal(workItem.Related, workItemUpdate.Related);
        Assert.False(workItemUpdate.IsFixedInChangeset);
        Assert.Equal(workItem.Tags, workItemUpdate.Tags);
        WorkItemUpdateFields workItemUpdateFieldsExpected = new WorkItemUpdateFields {
            AreaPath = workItemUpdateFields.AreaPath ?? workItem.AreaPath,
            TeamProject = workItemUpdateFields.TeamProject ?? workItem.TeamProject,
            IterationPath = workItemUpdateFields.IterationPath ?? workItem.IterationPath,
            Title = workItemUpdateFields.Title ?? workItem.Title,
            State = workItemUpdateFields.State ?? workItem.State,
            WorkItemType = workItemUpdateFields.WorkItemType ?? workItem.WorkItemType,
            AssignedToDisplayName = workItemUpdateFields.AssignedToDisplayName ?? workItem.AssignedToDisplayName,
            Activity = workItemUpdateFields.Activity ?? workItem.Activity,
            Description = workItemUpdateFields.Description ?? workItem.Description,
            ReproSteps = workItemUpdateFields.ReproSteps ?? workItem.ReproSteps,
            SystemInfo = workItemUpdateFields.SystemInfo ?? workItem.SystemInfo,
            AcceptanceCriteria = workItemUpdateFields.AcceptanceCriteria ?? workItem.AcceptanceCriteria
        };
        EqualUpdate(workItemUpdateFieldsExpected, workItemUpdate);
    }

    private static void EqualUpdate(WorkItemUpdateFields workItemUpdateFields, WorkItem workItem) {
        Assert.NotNull(workItemUpdateFields);
        Assert.NotNull(workItem);
        Assert.Equal(workItemUpdateFields.AreaPath, workItem.AreaPath);
        Assert.Equal(workItemUpdateFields.TeamProject, workItem.TeamProject);
        Assert.Equal(workItemUpdateFields.IterationPath, workItem.IterationPath);
        Assert.Equal(workItemUpdateFields.Title, workItem.Title);
        Assert.Equal(workItemUpdateFields.State, workItem.State);
        Assert.Equal(workItemUpdateFields.WorkItemType, workItem.WorkItemType);
        Assert.Equal(EmptyToNull(workItemUpdateFields.AssignedToDisplayName), workItem.AssignedToDisplayName);
        Assert.Equal(EmptyToNull(workItemUpdateFields.Activity), workItem.Activity);
        Assert.Equal(workItemUpdateFields.Description, workItem.Description);
        Assert.Equal(workItemUpdateFields.ReproSteps, workItem.ReproSteps);
        Assert.Equal(workItemUpdateFields.SystemInfo, workItem.SystemInfo);
        Assert.Equal(workItemUpdateFields.AcceptanceCriteria, workItem.AcceptanceCriteria);
    }

    public static void CheckRelations(Dictionary<LinkType, List<int>> relations, WorkItem workItem) {
        Assert.NotNull(workItem);
        if (relations == null || relations.Count == 0) {
            Assert.Null(workItem.Children);
            Assert.Null(workItem.Parent);
            Assert.Null(workItem.Related);
        }
        else {
            foreach ((LinkType linkType, List<int> workItemIds) in relations) {
                switch (linkType) {
                    case LinkType.Child:
                        Assert.Equal(workItemIds, workItem.Children);
                        break;
                    case LinkType.Parent:
                        Assert.True(workItemIds.Count <= 1);
                        Assert.Equal(workItemIds.FirstOrDefault(), workItem.Parent);
                        break;
                    case LinkType.Related:
                        Assert.Equal(workItemIds, workItem.Related);
                        break;
                    default: throw new NotImplementedException();
                }
            }
        }
    }

    public static void Equal(WorkItem expected, WorkItem actual) {
        Assert.NotNull(expected);
        Assert.NotNull(actual);
        Assert.Equal(expected.Url, actual.Url);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.AreaPath, actual.AreaPath);
        Assert.Equal(expected.TeamProject, actual.TeamProject);
        Assert.Equal(expected.IterationPath, actual.IterationPath);
        Assert.Equal(expected.Title, actual.Title);
        Assert.Equal(expected.State, actual.State);
        Assert.Equal(expected.WorkItemType, actual.WorkItemType);
        Assert.Equal(expected.AssignedToDisplayName, actual.AssignedToDisplayName);
        Assert.Equal(expected.Activity, actual.Activity);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.ReproSteps, actual.ReproSteps);
        Assert.Equal(expected.SystemInfo, actual.SystemInfo);
        Assert.Equal(expected.AcceptanceCriteria, actual.AcceptanceCriteria);
        Assert.Equal(expected.Children, actual.Children);
        Assert.Equal(expected.Parent, actual.Parent);
        Assert.Equal(expected.Related, actual.Related);
        Assert.Equal(expected.IsFixedInChangeset, actual.IsFixedInChangeset);
        Assert.Equal(expected.Tags, actual.Tags);
    }
}
