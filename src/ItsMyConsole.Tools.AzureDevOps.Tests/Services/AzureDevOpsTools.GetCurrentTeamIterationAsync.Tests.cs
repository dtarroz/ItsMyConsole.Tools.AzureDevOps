using System;
using System.Net.Http;
using System.Threading.Tasks;
using ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_GetCurrentTeamIterationAsync_Tests
{
    [Fact]
    public async Task GetCurrentTeamIterationAsync_Server_Url_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "Url_Fail";
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);

        await Assert.ThrowsAsync<HttpRequestException>(async () => {
            await azureDevOpsTools.GetCurrentTeamIterationAsync(ConfigForTests.TeamProject);
        });
    }

    [Fact]
    public async Task GetCurrentTeamIterationAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "PersonalAccessToken_Fail";
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.GetCurrentTeamIterationAsync(ConfigForTests.TeamProject);
        });

        Assert.Equal($"Vous n'avez pas les accès au serveur Azure DevOps '{azureDevOpsServer.Name}'", exception.Message);
    }

    [Fact]
    public async Task GetCurrentTeamIterationAsync_Project_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.GetCurrentTeamIterationAsync(null);
        });

        Assert.Equal("Le projet est obligatoire (Parameter 'project')", exception.Message);
    }

    [Fact]
    public async Task GetCurrentTeamIterationAsync_Project_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.GetCurrentTeamIterationAsync("");
        });

        Assert.Equal("Le projet est obligatoire (Parameter 'project')", exception.Message);
    }

    [Fact]
    public async Task GetCurrentTeamIterationAsync_Project_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.GetCurrentTeamIterationAsync("NotExists");
        });

        Assert.StartsWith("TF200016: ", exception.Message);
        Assert.Contains(": NotExists.", exception.Message);
    }

    [Fact]
    public async Task GetCurrentTeamIterationAsync_Team_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        TeamIteration teamIteration = await azureDevOpsTools.GetCurrentTeamIterationAsync(ConfigForTests.TeamProject, "");

        TeamIterationAssert.Equal(ConfigForTests.IterationPathCurrentDefaultTeam, teamIteration);
    }

    [Fact]
    public async Task GetCurrentTeamIterationAsync_Team_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.GetCurrentTeamIterationAsync(ConfigForTests.TeamProject, "NotExists");
        });

        Assert.Contains(" 'NotExists' ", exception.Message);
    }

    [Fact]
    public async Task GetCurrentTeamIterationAsync_Valid() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        TeamIteration teamIteration = await azureDevOpsTools.GetCurrentTeamIterationAsync(ConfigForTests.TeamProject);
        TeamIterationAssert.Equal(ConfigForTests.IterationPathCurrentDefaultTeam, teamIteration);

        teamIteration = await azureDevOpsTools.GetCurrentTeamIterationAsync(ConfigForTests.TeamProject, ConfigForTests.Team1);
        TeamIterationAssert.Equal(ConfigForTests.IterationPathCurrentTeam1, teamIteration);

        teamIteration = await azureDevOpsTools.GetCurrentTeamIterationAsync(ConfigForTests.TeamProject, ConfigForTests.Team2);
        TeamIterationAssert.Equal(ConfigForTests.IterationPathCurrentTeam2, teamIteration);

        teamIteration = await azureDevOpsTools.GetCurrentTeamIterationAsync(ConfigForTests.TeamProject, ConfigForTests.Team3);
        TeamIterationAssert.Equal(ConfigForTests.IterationPathCurrentTeam3, teamIteration);
    }
}
