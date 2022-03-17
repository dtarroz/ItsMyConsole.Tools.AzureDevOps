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
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();

        await Assert.ThrowsAsync<HttpRequestException>(() => azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields));
    }

    [Fact]
    public async Task CreateWorkItemAsync_Server_PersonalAccessToken_Fail() {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "PersonalAccessToken_Fail";
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal($"Vous n'avez pas les accès au serveur Azure DevOps '{azureDevOpsServer.Name}'", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentNullException>(() => azureDevOpsTools.CreateWorkItemAsync(null));

        Assert.Equal("Value cannot be null. (Parameter 'workItemCreateFields')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = new WorkItemCreateFields();

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal("Le projet est obligatoire (Parameter 'TeamProject')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Valid() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Valid_Min() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = new WorkItemCreateFields {
            TeamProject = ConfigForTests.TeamProject,
            Title = "TITLE",
            WorkItemType = ConfigForTests.WorkItemTypeNew
        };

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AreaPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.AreaPath = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AreaPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.AreaPath = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal("La zone ne doit pas être vide (Parameter 'AreaPath')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AreaPath_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.AreaPath = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.StartsWith("TF401347: ", exception.Message);
        Assert.Contains(" -1, ", exception.Message);
        Assert.Contains(" 'System.AreaPath'.", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_TeamProject_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.TeamProject = null;

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal("Le projet est obligatoire (Parameter 'TeamProject')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_TeamProject_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.TeamProject = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal("Le projet est obligatoire (Parameter 'TeamProject')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_TeamProject_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.TeamProject = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.StartsWith("TF200016: ", exception.Message);
        Assert.Contains(": NotExist. ", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_IterationPath_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.IterationPath = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_IterationPath_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.IterationPath = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal("L'itération ne doit pas être vide (Parameter 'IterationPath')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_IterationPath_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.IterationPath = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.StartsWith("TF401347: ", exception.Message);
        Assert.Contains(" -1, ", exception.Message);
        Assert.Contains(" 'System.IterationPath'.", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Title_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Title = null;

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal("Le titre est obligatoire (Parameter 'Title')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Title_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Title = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal("Le titre est obligatoire (Parameter 'Title')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_State_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.State = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_State_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.State = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal("L'état ne doit pas être vide (Parameter 'State')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_State_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.State = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Contains(" 'State' ", exception.Message);
        Assert.Contains(" 'NotExist' ", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_WorkItemType_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.WorkItemType = null;

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal("Le type est obligatoire (Parameter 'WorkItemType')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_WorkItemType_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.WorkItemType = "";

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal("Le type est obligatoire (Parameter 'WorkItemType')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_WorkItemType_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.WorkItemType = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.StartsWith("VS402323: ", exception.Message);
        Assert.Contains(" NotExist ", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AssignedToDisplayName_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.AssignedToDisplayName = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AssignedToDisplayName_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.AssignedToDisplayName = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AssignedToDisplayName_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.AssignedToDisplayName = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Contains(" 'NotExist' ", exception.Message);
        Assert.Contains(" 'Assigned To' ", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Activity_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Activity = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Activity_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Activity = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Activity_NotExists() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Activity = "NotExist";

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Contains(" 'Activity' ", exception.Message);
        Assert.Contains(" 'NotExist' ", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Description_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Description = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Description_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Description = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_ReproSteps_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.ReproSteps = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_ReproSteps_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.ReproSteps = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_SystemInfo_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.SystemInfo = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_SystemInfo_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.SystemInfo = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AcceptanceCriteria_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.AcceptanceCriteria = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_AcceptanceCriteria_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.AcceptanceCriteria = "";

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_List_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Tags = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_List_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Tags = Array.Empty<string>();

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Tags = new[] { null, "TAGS_NULL" };

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Tags = new[] { "", "TAGS_EMPTY" };

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_One() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Tags = new[] { "TAGS_ONE" };

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_Multi() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_Spaces() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Tags = new[] { "  SPACES  EVERYWHERE  " };

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_Order() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Tags = new[] { "C", "A", "B" };

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_SameValue() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Tags = new[] { "SAME_VALUE", "SAME_VALUE" };

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_SameValue_CaseInsensitive() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Tags = new[] { "SAME_VALUE_CI", "Same_Value_ci" };

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_Exists_CaseInsensitive() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        string tag = Guid.NewGuid().ToString().ToUpper();
        workItemCreateFields.Tags = new[] { tag };
        WorkItem workItemUpper = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

        workItemCreateFields.Tags = new[] { tag.ToLower() };
        WorkItem workItemLower = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItemLower.Id);

        await azureDevOpsTools.DeleteWorkItemAsync(workItemUpper.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemLower.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItemLower);
        WorkItemAssert.Equal(workItemLower, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Tags_Semicolon() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Tags = new[] { "SEMI;COLON" };

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        });

        Assert.Equal("Une balise ne doit pas contenir de \";\" (Parameter 'Tags')", exception.Message);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Effort_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Effort = null;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Effort_Negative() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Effort = -1;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Effort_Zero() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Effort = 0;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Effort_Double() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Effort = 3.14159265;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Fact]
    public async Task CreateWorkItemAsync_Effort_Int() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        workItemCreateFields.Effort = 10;

        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);

        WorkItemAssert.CheckCreate(workItemCreateFields, workItem);
        WorkItemAssert.Equal(workItem, workItemGet);
    }
}
