using System;
using System.Net.Http;
using System.Threading.Tasks;
using ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_CreateWorkItemAsync_Tests
{
    [Fact]
    public async Task CreateWorkItemAsync_Server_Url_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "Url_Fail";
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();

        await Assert.ThrowsAsync<HttpRequestException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "PersonalAccessToken_Fail";
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();

        Exception exception = await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));

        Assert.Equal($"Vous n'avez pas les accès au serveur Azure DevOps '{azureDevOpsServer.Name}'", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentNullException>(() => azureDevOpsTools.CreateWorkItemAsync(null));

        Assert.Equal("Value cannot be null. (Parameter 'workItemFields')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = new WorkItemFields();

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        });

        Assert.Equal("Le projet est obligatoire (Parameter 'TeamProject')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Valid() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
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
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AreaPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AreaPath = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AreaPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AreaPath = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        });

        Assert.Equal("La zone ne doit pas être vide (Parameter 'AreaPath')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AreaPath_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AreaPath = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));

        Assert.StartsWith("TF401347: ", exception.Message);
        Assert.Contains(" -1, ", exception.Message);
        Assert.Contains(" 'System.AreaPath'.", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_TeamProject_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.TeamProject = null;

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        });

        Assert.Equal("Le projet est obligatoire (Parameter 'TeamProject')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_TeamProject_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.TeamProject = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        });

        Assert.Equal("Le projet est obligatoire (Parameter 'TeamProject')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_TeamProject_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.TeamProject = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));

        Assert.StartsWith("TF200016: ", exception.Message);
        Assert.Contains(": NotExist. ", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_IterationPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.IterationPath = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_IterationPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.IterationPath = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        });

        Assert.Equal("L'itération ne doit pas être vide (Parameter 'IterationPath')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_IterationPath_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.IterationPath = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));

        Assert.StartsWith("TF401347: ", exception.Message);
        Assert.Contains(" -1, ", exception.Message);
        Assert.Contains(" 'System.IterationPath'.", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Title_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Title = null;

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        });

        Assert.Equal("Le titre est obligatoire (Parameter 'Title')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Title_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Title = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        });

        Assert.Equal("Le titre est obligatoire (Parameter 'Title')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_State_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.State = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_State_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.State = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        });

        Assert.Equal("L'état ne doit pas être vide (Parameter 'State')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_State_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.State = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));

        Assert.Contains(" 'State' ", exception.Message);
        Assert.Contains(" 'NotExist' ", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_WorkItemType_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.WorkItemType = null;

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        });

        Assert.Equal("Le type est obligatoire (Parameter 'WorkItemType')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_WorkItemType_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.WorkItemType = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        });

        Assert.Equal("Le type est obligatoire (Parameter 'WorkItemType')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_WorkItemType_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.WorkItemType = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        });

        Assert.StartsWith("VS402323: ", exception.Message);
        Assert.Contains(" NotExist ", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AssignedToDisplayName_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AssignedToDisplayName = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AssignedToDisplayName_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AssignedToDisplayName = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AssignedToDisplayName_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AssignedToDisplayName = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));

        Assert.Contains(" 'NotExist' ", exception.Message);
        Assert.Contains(" 'Assigned To' ", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Activity_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Activity = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Activity_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Activity = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Activity_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Activity = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(() => azureDevOpsTools.CreateWorkItemAsync(workItemFields));

        Assert.Contains(" 'Activity' ", exception.Message);
        Assert.Contains(" 'NotExist' ", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Description_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Description = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Description_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.Description = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_ReproSteps_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.ReproSteps = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_ReproSteps_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.ReproSteps = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_SystemInfo_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.SystemInfo = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_SystemInfo_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.SystemInfo = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AcceptanceCriteria_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AcceptanceCriteria = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AcceptanceCriteria_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        workItemFields.AcceptanceCriteria = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckNew(workItemFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }
}
