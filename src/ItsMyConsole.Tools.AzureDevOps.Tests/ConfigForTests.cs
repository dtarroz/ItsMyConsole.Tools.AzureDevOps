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

    public static string[] GetAreaPaths() {
        return new[] { "<AreaPath_1>", "<AreaPath_2>" };
    }

    public static string GetTeamProject() {
        return "<TeamProject>";
    }

    public static string[] GetIterationPathCurrents() {
        return new[] { "<IterationPath_1>", "<IterationPath_2>" };
    }

    public static string[] GetStates() {
        return new[] { "New", "Active" };
    }

    public static string[] GetWorkItemTypes() {
        return new[] { "Feature", "Bug" };
    }

    public static string GetAssignedToDisplayName() {
        return "<AssignedToDisplayName>";
    }

    public static string[] GetActivities() {
        return new[] { "Development", "Design" };
    }
}
