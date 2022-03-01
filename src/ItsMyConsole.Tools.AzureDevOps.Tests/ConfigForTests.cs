using System;

namespace ItsMyConsole.Tools.AzureDevOps.Tests;

public static class ConfigForTests
{
    public static AzureDevOpsServer GetAzureDevOpsServer() {
        return new AzureDevOpsServer {
            Name = "<NAME>",
            Url = "<URL>",
            PersonalAccessToken = "<PERSONAL_ACCESS_TOKEN>"
        };
    }

    public static WorkItemFields GetWorkItemFieldsNew() {
        return new WorkItemFields {
            AreaPath = AreaPathNew,
            TeamProject = TeamProject,
            IterationPath = IterationPathNew,
            Title = "TITLE",
            State = StateNew,
            WorkItemType = WorkItemTypeNew,
            AssignedToDisplayName = AssignedToDisplayName,
            Activity = ActivityNew,
            Description = "DESCRIPTION",
            ReproSteps = "REPRO_STEPS",
            SystemInfo = "SYSTEM_INFO",
            AcceptanceCriteria = "ACCEPTANCE_CRITERIA"
        };
    }

    public static WorkItemFields GetWorkItemFieldsUpdate() {
        return new WorkItemFields {
            AreaPath = AreaPathUpdate,
            TeamProject = TeamProject,
            IterationPath = IterationPathUpdate,
            Title = "TITLE_UPDATE",
            State = StateUpdate,
            WorkItemType = WorkItemTypeUpdate,
            AssignedToDisplayName = null,
            Activity = ActivityUpdate,
            Description = "DESCRIPTION_UPDATE",
            ReproSteps = "REPRO_STEPS_UPDATE",
            SystemInfo = "SYSTEM_INFO_UPDATE",
            AcceptanceCriteria = "ACCEPTANCE_CRITERIA_UPDATE"
        };
    }

    public const string ServerUrl = "<SERVER_URL>";

    public const string AreaPathDefault = "<AREA_PATH_DEFAULT>";
    public const string AreaPathNew = "<AREA_PATH_NEW>";
    public const string AreaPathUpdate = "<AREA_PATH_UPDATE>";

    public const string TeamProject = "<TEAM_PROJECT>";

    public const string IterationPathDefault = "<ITERATION_PATH_DEFAULT>";
    public const string IterationPathNew = "<ITERATION_PATH_NEW>";
    public const string IterationPathUpdate = "<ITERATION_PATH_UPDATE>";

    public const string StateDefault = "New";
    public const string StateNew = "New";
    public const string StateUpdate = "Active";

    public const string WorkItemTypeNew = "Feature";
    public const string WorkItemTypeUpdate = "Bug";

    public const string AssignedToDisplayName = "<ASSIGNED_TO_DISPLAY_NAME>";

    public const string ActivityNew = "Development";
    public const string ActivityUpdate = "Design";

    public static readonly TeamIteration IterationPathCurrentDefaultTeam = new() {
        Name = "<NAME>",
        Path = "<PATH>",
        StartDate = new DateTime(2022, 1, 1),
        FinishDate = new DateTime(2022, 12, 31)
    };

    public const string Team1 = "<TEAM_1>";

    public static readonly TeamIteration IterationPathCurrentTeam1 = new() {
        Name = "<NAME>",
        Path = "<PATH>",
        StartDate = new DateTime(2022, 1, 1),
        FinishDate = new DateTime(2022, 12, 31)
    };

    public const string Team2 = "<TEAM_2>";

    public static readonly TeamIteration IterationPathCurrentTeam2 = new() {
        Name = "<NAME>",
        Path = "<PATH>",
        StartDate = new DateTime(2022, 1, 1),
        FinishDate = new DateTime(2022, 12, 31)
    };

    public const string Team3 = "<TEAM_3>";

    public static readonly TeamIteration IterationPathCurrentTeam3 = new() {
        Name = "<NAME>",
        Path = "<PATH>",
        StartDate = new DateTime(2022, 1, 1),
        FinishDate = new DateTime(2022, 12, 31)
    };
}
