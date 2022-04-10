using System;
using System.Net.Http;
using System.Threading.Tasks;
using ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_GetCurrentIterationAsync_Tests
{
    [Fact]
    public async Task GetCurrentIterationAsync_Server_Url_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "Url_Fail";
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);

        await Assert.ThrowsAsync<HttpRequestException>(async () => {
            await azureDevOpsTools.GetCurrentIterationAsync(ConfigForTests.TeamProject);
        });
    }

    [Fact]
    public async Task GetCurrentIterationAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "PersonalAccessToken_Fail";
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.GetCurrentIterationAsync(ConfigForTests.TeamProject);
        });

        Assert.Equal($"Vous n'avez pas les accès au serveur Azure DevOps '{azureDevOpsServer.Name}'", exception.Message);
    }

    [Fact]
    public async Task GetCurrentIterationAsync_Project_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.GetCurrentIterationAsync(null);
        });

        Assert.Equal("Le projet est obligatoire (Parameter 'project')", exception.Message);
    }

    [Fact]
    public async Task GetCurrentIterationAsync_Project_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.GetCurrentIterationAsync("");
        });

        Assert.Equal("Le projet est obligatoire (Parameter 'project')", exception.Message);
    }

    [Fact]
    public async Task GetCurrentIterationAsync_Project_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.GetCurrentIterationAsync("NotExists");
        });

        Assert.StartsWith("TF200016: ", exception.Message);
        Assert.Contains(": NotExists.", exception.Message);
    }

    [Fact]
    public async Task GetCurrentIterationAsync_Team_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Iteration iteration = await azureDevOpsTools.GetCurrentIterationAsync(ConfigForTests.TeamProject, "");

        IterationAssert.Equal(ConfigForTests.IterationPathCurrentDefault, iteration);
    }

    [Fact]
    public async Task GetCurrentIterationAsync_Team_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.GetCurrentIterationAsync(ConfigForTests.TeamProject, "NotExists");
        });

        Assert.Contains(" 'NotExists' ", exception.Message);
    }

    [Fact]
    public async Task GetCurrentIterationAsync_Valid() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Iteration iteration = await azureDevOpsTools.GetCurrentIterationAsync(ConfigForTests.TeamProject);
        IterationAssert.Equal(ConfigForTests.IterationPathCurrentDefault, iteration);

        iteration = await azureDevOpsTools.GetCurrentIterationAsync(ConfigForTests.TeamProject, ConfigForTests.Team1);
        IterationAssert.Equal(ConfigForTests.IterationPathCurrentTeam1, iteration);

        iteration = await azureDevOpsTools.GetCurrentIterationAsync(ConfigForTests.TeamProject, ConfigForTests.Team2);
        IterationAssert.Equal(ConfigForTests.IterationPathCurrentTeam2, iteration);

        iteration = await azureDevOpsTools.GetCurrentIterationAsync(ConfigForTests.TeamProject, ConfigForTests.Team3);
        IterationAssert.Equal(ConfigForTests.IterationPathCurrentTeam3, iteration);
    }
}
