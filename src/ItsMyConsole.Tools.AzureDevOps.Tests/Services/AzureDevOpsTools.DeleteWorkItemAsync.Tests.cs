using System;
using System.Net.Http;
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

        await Assert.ThrowsAsync<HttpRequestException>(() => azureDevOpsTools.DeleteWorkItemAsync(1));
    }

    [Fact]
    public async Task DeleteWorkItemAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "PersonalAccessToken_Fail";
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);

        Exception exception = await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.DeleteWorkItemAsync(1));

        Assert.Equal($"Vous n'avez pas les accès au serveur Azure DevOps '{azureDevOpsServer.Name}'", exception.Message);
    }

    [Fact]
    public async Task DeleteWorkItemAsync_Id_Negative() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.DeleteWorkItemAsync(-1));

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')",
                     exception.Message);
    }

    [Fact]
    public async Task DeleteWorkItemAsync_Id_Zero() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.DeleteWorkItemAsync(0));

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')",
                     exception.Message);
    }

    [Fact]
    public async Task DeleteWorkItemAsync_Id_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.DeleteWorkItemAsync(999999));

        Assert.StartsWith("TF401232: ", exception.Message);
        Assert.Contains(" 999999 ", exception.Message);
    }

    [Fact]
    public async Task DeleteWorkItemAsync_Valid() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Exception exception = await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.GetWorkItemAsync(workItem.Id));

        Assert.StartsWith("TF401232: ", exception.Message);
        Assert.Contains($" {workItem.Id} ", exception.Message);
    }
}
