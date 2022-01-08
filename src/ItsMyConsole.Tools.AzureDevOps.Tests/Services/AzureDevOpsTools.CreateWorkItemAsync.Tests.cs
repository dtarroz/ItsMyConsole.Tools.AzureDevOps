using System;
using System.Threading.Tasks;
using ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_CreateWorkItemAsync_Tests
{
    [Fact]
    public async Task CreateWorkItemAsync_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<ArgumentNullException>(() => azureDevOpsTools.CreateWorkItemAsync(null));
    }

    [Fact]
    public async Task CreateWorkItemAsync_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = new WorkItemFields();

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_Server_Url_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_Valid() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    private static void CheckWorkItemCreated(WorkItemFields workItemFields, WorkItem workItem) {
        WorkItemAssert.CheckNew(workItem);
        WorkItemAssert.Equal(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Valid_Min() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = new WorkItemFields {
            TeamProject = ConfigForTests.TeamProject,
            Title = "TITLE",
            WorkItemType = ConfigForTests.WorkItemTypeNew
        };

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AreaPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AreaPath = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AreaPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AreaPath = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_AreaPath_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AreaPath = "NotExist";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_TeamProject_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.TeamProject = null;

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_TeamProject_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.TeamProject = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_TeamProject_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.TeamProject = "NotExist";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_IterationPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.IterationPath = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_IterationPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.IterationPath = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_IterationPath_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.IterationPath = "NotExist";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_Title_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Title = null;

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_Title_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Title = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_State_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.State = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_State_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.State = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_State_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.State = "NotExist";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_WorkItemType_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.WorkItemType = null;

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_WorkItemType_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.WorkItemType = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_WorkItemType_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.WorkItemType = "NotExist";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_AssignedToDisplayName_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AssignedToDisplayName = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AssignedToDisplayName_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AssignedToDisplayName = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AssignedToDisplayName_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AssignedToDisplayName = "NotExist";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_Activity_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Activity = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Activity_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Activity = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Activity_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Activity = "NotExist";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_Description_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Description = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Description_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Description = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_ReproSteps_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.ReproSteps = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_ReproSteps_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.ReproSteps = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_SystemInfo_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.SystemInfo = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_SystemInfo_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.SystemInfo = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AcceptanceCriteria_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AcceptanceCriteria = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AcceptanceCriteria_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AcceptanceCriteria = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        CheckWorkItemCreated(workItemFields, workItem);
    }
}
