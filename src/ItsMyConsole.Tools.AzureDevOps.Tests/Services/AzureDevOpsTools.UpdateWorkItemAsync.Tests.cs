using System;
using System.Threading.Tasks;
using ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_UpdateWorkItemAsync_Tests
{
    [Fact]
    public async Task UpdateWorkItemAsync_Server_Url_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.UpdateWorkItemAsync(1, workItemFields));
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.UpdateWorkItemAsync(1, workItemFields));
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Fields_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<ArgumentNullException>(() => azureDevOpsTools.UpdateWorkItemAsync(1, null));
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Fields_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = new WorkItemFields();

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Id_Negative() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.UpdateWorkItemAsync(-1, workItemFields));
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Id_Zero() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.UpdateWorkItemAsync(0, workItemFields));
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Id_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.UpdateWorkItemAsync(999999, workItemFields));
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Valid() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AreaPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AreaPath = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AreaPath_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AreaPath = workItem.AreaPath;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AreaPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AreaPath = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    private static async Task CheckWorkItemNotModifiedAsync(AzureDevOpsTools azureDevOpsTools, WorkItem workItem) {
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AreaPath_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AreaPath = "NotExists";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_TeamProject_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.TeamProject = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_TeamProject_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.TeamProject = workItem.TeamProject;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_TeamProject_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.TeamProject = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_TeamProject_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.TeamProject = "NotExists";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.IterationPath = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.IterationPath = workItem.IterationPath;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.IterationPath = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.IterationPath = "NotExists";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Title_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Title = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Title_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Title = workItem.Title;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Title_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Title = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.IterationPath = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.State = workItem.State;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.State = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.State = "NotExists";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.WorkItemType = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.WorkItemType = workItem.WorkItemType;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.WorkItemType = "";

        await Assert.ThrowsAsync<ArgumentException>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.WorkItemType = "NotExists";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AssignedToDisplayName = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AssignedToDisplayName = workItem.AssignedToDisplayName;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AssignedToDisplayName = "";

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AssignedToDisplayName = "NotExists";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Activity = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Activity = workItem.Activity;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Activity = "";

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Activity = "NotExists";

        await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields));
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Description_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Description = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Description_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Description = workItem.Description;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Description_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Description = "";

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_ReproSteps_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.ReproSteps = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_ReproSteps_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.ReproSteps = workItem.ReproSteps;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_ReproSteps_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.ReproSteps = "";

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_SystemInfo_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.SystemInfo = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_SystemInfo_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.SystemInfo = workItem.SystemInfo;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_SystemInfo_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.SystemInfo = "";

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AcceptanceCriteria_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AcceptanceCriteria = null;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AcceptanceCriteria_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AcceptanceCriteria = workItem.AcceptanceCriteria;

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AcceptanceCriteria_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AcceptanceCriteria = "";

        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemUpdate = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
    }
}
