using System;
using System.Net.Http;
using System.Threading.Tasks;
using ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_UpdateWorkItemAsync_Tests
{
    [Fact]
    public async Task UpdateWorkItemAsync_Server_Url_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "Url_Fail";
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        await Assert.ThrowsAsync<HttpRequestException>(() => azureDevOpsTools.UpdateWorkItemAsync(1, workItemFields));
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "PersonalAccessToken_Fail";
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        Exception exception = await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.UpdateWorkItemAsync(1, workItemFields));

        Assert.Equal($"Vous n'avez pas les accès au serveur Azure DevOps '{azureDevOpsServer.Name}'", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Fields_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(1, null);
        });

        Assert.Equal("Value cannot be null. (Parameter 'workItemFields')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Fields_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = new WorkItemFields();

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Id_Negative() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(-1, workItemFields);
        });

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')",
                     exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Id_Zero() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(0, workItemFields);
        });

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')",
                     exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Id_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(999999, workItemFields);
        });

        Assert.StartsWith("TF401232: ", exception.Message);
        Assert.Contains(" 999999 ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Valid() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AreaPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AreaPath = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AreaPath_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AreaPath = workItem.AreaPath;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AreaPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AreaPath = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Equal("La zone ne doit pas être vide (Parameter 'AreaPath')", exception.Message);
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

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.StartsWith("TF401347: ", exception.Message);
        Assert.Contains($" {workItem.Id}, ", exception.Message);
        Assert.Contains(" 'System.AreaPath'.", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_TeamProject_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.TeamProject = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_TeamProject_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.TeamProject = workItem.TeamProject;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_TeamProject_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.TeamProject = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Equal("Le projet ne doit pas être vide (Parameter 'TeamProject')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_TeamProject_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.TeamProject = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.StartsWith("VS402834: ", exception.Message);
        Assert.Contains(": NotExists. ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.IterationPath = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.IterationPath = workItem.IterationPath;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.IterationPath = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Equal("L'itération ne doit pas être vide (Parameter 'IterationPath')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.IterationPath = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.StartsWith("TF401347: ", exception.Message);
        Assert.Contains($" {workItem.Id}, ", exception.Message);
        Assert.Contains(" 'System.IterationPath'.", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Title_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Title = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Title_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Title = workItem.Title;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Title_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Title = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Equal("Le titre ne doit pas être vide (Parameter 'Title')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.IterationPath = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.State = workItem.State;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.State = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Equal("L'état ne doit pas être vide (Parameter 'State')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.State = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Contains(" 'State' ", exception.Message);
        Assert.Contains(" 'NotExists' ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.WorkItemType = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.WorkItemType = workItem.WorkItemType;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.WorkItemType = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Equal("Le type ne doit pas être vide (Parameter 'WorkItemType')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.WorkItemType = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.StartsWith("VS403072: ", exception.Message);
        Assert.Contains(" NotExists ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AssignedToDisplayName = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AssignedToDisplayName = workItem.AssignedToDisplayName;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AssignedToDisplayName = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AssignedToDisplayName = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Contains(" 'NotExists' ", exception.Message);
        Assert.Contains(" 'Assigned To' ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Activity = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Activity = workItem.Activity;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Activity = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Activity = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Contains(" 'Activity' ", exception.Message);
        Assert.Contains(" 'NotExists' ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Description_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Description = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Description_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Description = workItem.Description;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Description_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.Description = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_ReproSteps_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.ReproSteps = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_ReproSteps_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.ReproSteps = workItem.ReproSteps;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_ReproSteps_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.ReproSteps = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_SystemInfo_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.SystemInfo = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_SystemInfo_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.SystemInfo = workItem.SystemInfo;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_SystemInfo_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.SystemInfo = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AcceptanceCriteria_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AcceptanceCriteria = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AcceptanceCriteria_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AcceptanceCriteria = workItem.AcceptanceCriteria;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AcceptanceCriteria_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();
        workItemFields.AcceptanceCriteria = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AddRelation() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Child);
        workItem = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        workItemFields = ConfigForTests.GetWorkItemFieldsUpdate();

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }
}
