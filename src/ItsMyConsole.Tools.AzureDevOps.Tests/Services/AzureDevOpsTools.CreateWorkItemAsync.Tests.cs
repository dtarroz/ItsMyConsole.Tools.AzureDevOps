using System;
using System.Linq;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_CreateWorkItemAsync_Tests
{
    [Fact]
    public async System.Threading.Tasks.Task CreateWorkItemAsync_ArgumentNullException() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<ArgumentNullException>(() => azureDevOpsTools.CreateWorkItemAsync(null));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateWorkItemAsync_ArgumentException_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = new WorkItemFields();

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateWorkItemAsync_ArgumentException_Title() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = new WorkItemFields {
            TeamProject = ConfigForTests.GetTeamProject(),
            WorkItemType = ConfigForTests.GetWorkItemTypes().First()
        };

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateWorkItemAsync_ArgumentException_TeamProject() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = new WorkItemFields {
            Title = "<TITLE>",
            WorkItemType = ConfigForTests.GetWorkItemTypes().First()
        };

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateWorkItemAsync_ArgumentException_WorkItemType() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = new WorkItemFields {
            TeamProject = ConfigForTests.GetTeamProject(),
            Title = "<TITLE>"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }
}
