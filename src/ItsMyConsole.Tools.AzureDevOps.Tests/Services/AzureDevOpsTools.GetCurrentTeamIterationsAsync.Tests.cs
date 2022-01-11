using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_GetCurrentTeamIterationsAsync_Tests
{
    [Fact]
    public async Task GetCurrentTeamIterationsAsync_Server_Url_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.GetCurrentTeamIterationsAsync(ConfigForTests.TeamProject));
    }

    [Fact]
    public async Task GetCurrentTeamIterationsAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.GetCurrentTeamIterationsAsync(ConfigForTests.TeamProject));
    }

    [Fact]
    public async Task GetCurrentTeamIterationsAsync_Project_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.GetCurrentTeamIterationsAsync(null));
    }

    [Fact]
    public async Task GetCurrentTeamIterationsAsync_Project_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.GetCurrentTeamIterationsAsync(""));
    }

    [Fact]
    public async Task GetCurrentTeamIterationsAsync_Project_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.GetCurrentTeamIterationsAsync("NotExists"));
    }

    [Fact]
    public async Task GetCurrentTeamIterationsAsync_Team_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        List<TeamIteration> teamIterations = await azureDevOpsTools.GetCurrentTeamIterationsAsync(ConfigForTests.TeamProject, "");

        TeamIterationAssert.Equal(ConfigForTests.IterationPathCurrentDefaultTeam, teamIterations);
    }

    [Fact]
    public async Task GetCurrentTeamIterationsAsync_Team_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.GetCurrentTeamIterationsAsync(ConfigForTests.TeamProject, "NotExists");
        });
    }

    [Fact]
    public async Task GetCurrentTeamIterationsAsync_Valid() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        List<TeamIteration> teamIterations = await azureDevOpsTools.GetCurrentTeamIterationsAsync(ConfigForTests.TeamProject);
        TeamIterationAssert.Equal(ConfigForTests.IterationPathCurrentDefaultTeam, teamIterations);

        teamIterations = await azureDevOpsTools.GetCurrentTeamIterationsAsync(ConfigForTests.TeamProject, ConfigForTests.Team1);
        TeamIterationAssert.Equal(ConfigForTests.IterationPathCurrentTeam1, teamIterations);

        teamIterations = await azureDevOpsTools.GetCurrentTeamIterationsAsync(ConfigForTests.TeamProject, ConfigForTests.Team2);
        TeamIterationAssert.Equal(ConfigForTests.IterationPathCurrentTeam2, teamIterations);

        teamIterations = await azureDevOpsTools.GetCurrentTeamIterationsAsync(ConfigForTests.TeamProject, ConfigForTests.Team3);
        TeamIterationAssert.Equal(ConfigForTests.IterationPathCurrentTeam3, teamIterations);
    }
}
