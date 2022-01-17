using System;
using System.Threading.Tasks;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_GetWorkItemAsync_Tests
{
    [Fact]
    public async Task GetWorkItemAsync_Server_Url_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.GetWorkItemAsync(1);
        });
    }

    [Fact]
    public async Task GetWorkItemAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.GetWorkItemAsync(1);
        });
    }

    [Fact]
    public async Task GetWorkItemAsync_Id_Negative() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.GetWorkItemAsync(-1);
        });
    }

    [Fact]
    public async Task GetWorkItemAsync_Id_Zero() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.GetWorkItemAsync(0);
        });
    }

    [Fact]
    public async Task GetWorkItemAsync_Id_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.GetWorkItemAsync(999999);
        });
    }
}
