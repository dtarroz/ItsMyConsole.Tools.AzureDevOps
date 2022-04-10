using System;
using System.Net.Http;
using System.Threading.Tasks;
using ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_UpdateWorkItemAsync_Id_Tests
{
    [Fact]
    public async Task UpdateWorkItemAsync_Server_Url_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "Url_Fail";
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();

        await Assert.ThrowsAsync<HttpRequestException>(() => azureDevOpsTools.UpdateWorkItemAsync(1, workItemUpdateFields));
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "PersonalAccessToken_Fail";
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(1, workItemUpdateFields);
        });

        Assert.Equal($"Vous n'avez pas les accès au serveur Azure DevOps '{azureDevOpsServer.Name}'", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Fields_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(1, null);
        });

        Assert.Equal("Value cannot be null. (Parameter 'workItemUpdateFields')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Fields_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = new WorkItemUpdateFields();

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Id_Negative() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(-1, workItemUpdateFields);
        });

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')",
                     exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Id_Zero() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(0, workItemUpdateFields);
        });

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')",
                     exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Id_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(999999, workItemUpdateFields);
        });

        Assert.StartsWith("TF401232: ", exception.Message);
        Assert.Contains(" 999999 ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Valid() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AreaPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.AreaPath = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AreaPath_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.AreaPath = workItem.AreaPath;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AreaPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.AreaPath = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
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
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.AreaPath = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.StartsWith("TF401347: ", exception.Message);
        Assert.Contains($" {workItem.Id}, ", exception.Message);
        Assert.Contains(" 'System.AreaPath'.", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Project_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemCreateFields.Project = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Project_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Project = workItem.Project;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Project_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Project = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Equal("Le projet ne doit pas être vide (Parameter 'Project')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Project_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Project = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.StartsWith("VS402834: ", exception.Message);
        Assert.Contains(" NotExists ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.IterationPath = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.IterationPath = workItem.IterationPath;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.IterationPath = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Equal("L'itération ne doit pas être vide (Parameter 'IterationPath')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_IterationPath_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.IterationPath = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
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
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Title = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Title_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Title = workItem.Title;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Title_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Title = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Equal("Le titre ne doit pas être vide (Parameter 'Title')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.IterationPath = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.State = workItem.State;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.State = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Equal("L'état ne doit pas être vide (Parameter 'State')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_State_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.State = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Contains(" 'State' ", exception.Message);
        Assert.Contains(" 'NotExists' ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.WorkItemType = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.WorkItemType = workItem.WorkItemType;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.WorkItemType = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Equal("Le type ne doit pas être vide (Parameter 'WorkItemType')", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_WorkItemType_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.WorkItemType = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.StartsWith("VS403072: ", exception.Message);
        Assert.Contains(" NotExists ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.AssignedToDisplayName = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.AssignedToDisplayName = workItem.AssignedToDisplayName;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.AssignedToDisplayName = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AssignedToDisplayName_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.AssignedToDisplayName = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Contains(" 'NotExists' ", exception.Message);
        Assert.Contains(" 'Assigned To' ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Activity = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Activity = workItem.Activity;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Activity = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Activity_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Activity = "NotExists";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        });
        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        Assert.Contains(" 'Activity' ", exception.Message);
        Assert.Contains(" 'NotExists' ", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Description_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Description = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Description_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Description = workItem.Description;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Description_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Description = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_ReproSteps_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.ReproSteps = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_ReproSteps_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.ReproSteps = workItem.ReproSteps;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_ReproSteps_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.ReproSteps = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_SystemInfo_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.SystemInfo = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_SystemInfo_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.SystemInfo = workItem.SystemInfo;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_SystemInfo_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.SystemInfo = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AcceptanceCriteria_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.AcceptanceCriteria = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AcceptanceCriteria_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.AcceptanceCriteria = workItem.AcceptanceCriteria;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AcceptanceCriteria_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.AcceptanceCriteria = "";

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_AddRelation() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Child);
        workItem = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Effort_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Effort = null;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Effort_Negative() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Effort = -1;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Effort_Zero() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Effort = 0;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Effort_Double() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Effort = 3.14159265;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Effort_Int() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Effort = 10;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Effort_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();
        workItemUpdateFields.Effort = workItem.Effort;

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_SameCall() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();

        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }

    [Fact]
    public async Task UpdateWorkItemAsync_Rev3() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItemUpdateFields workItemUpdateFields = ConfigForTests.GetWorkItemUpdateFields();

        workItem = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);

        workItemUpdateFields.Title = "TITLE_UPDATE_2";
        WorkItem workItemUpdate = await azureDevOpsTools.UpdateWorkItemAsync(workItem.Id, workItemUpdateFields);

        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckUpdate(workItem, workItemUpdateFields, workItemUpdate);
        WorkItemAssert.Equal(workItemUpdate, workItemGet);
    }
}
