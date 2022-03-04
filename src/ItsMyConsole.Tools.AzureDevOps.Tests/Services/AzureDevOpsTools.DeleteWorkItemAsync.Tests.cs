using System;
using System.Threading.Tasks;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_DeleteWorkItemAsync_Tests
{
    [Fact]
    public async Task DeleteWorkItemAsync_Server_Url_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "Url_Fail";
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.DeleteWorkItemAsync(1));
    }

    [Fact]
    public async Task DeleteWorkItemAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "PersonalAccessToken_Fail";
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.DeleteWorkItemAsync(1));
    }

    [Fact]
    public async Task DeleteWorkItemAsync_Id_Negative() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.DeleteWorkItemAsync(-1));
    }

    [Fact]
    public async Task DeleteWorkItemAsync_Id_Zero() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.DeleteWorkItemAsync(0));
    }

    [Fact]
    public async Task DeleteWorkItemAsync_Id_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.DeleteWorkItemAsync(999999));
    }

    [Fact]
    public async Task DeleteWorkItemAsync_Valid() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.GetWorkItemAsync(workItem.Id));
    }
}
