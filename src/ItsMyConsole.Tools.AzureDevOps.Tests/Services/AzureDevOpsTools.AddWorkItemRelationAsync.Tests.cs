using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;
using ItsMyConsole.Tools.AzureDevOps.Tests.Data;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_AddWorkItemRelationAsync_Tests
{
    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_Server_Url_Fail(LinkType linkType) {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "Url_Fail";
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        await Assert.ThrowsAsync<HttpRequestException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(1, workItem, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_Server_PersonalAccessToken_Fail(LinkType linkType) {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "PersonalAccessToken_Fail";
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(1, workItem, linkType);
        });

        Assert.Equal($"Vous n'avez pas les accès au serveur Azure DevOps '{azureDevOpsServer.Name}'", exception.Message);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_Id_Negative(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(-1, workItem, linkType);
        });

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')",
                     exception.Message);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_Id_Zero(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(0, workItem, linkType);
        });

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')",
                     exception.Message);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_Id_NotExists(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(999999, workItem, linkType);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        Assert.StartsWith("TF401232: ", exception.Message);
        Assert.Contains(" 999999 ", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    private static async Task CheckWorkItemNotModifiedAsync(AzureDevOpsTools azureDevOpsTools, WorkItem workItem) {
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        WorkItemAssert.Equal(workItem, workItemGet);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_WorkItem_Null(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(1, null, linkType);
        });

        Assert.Equal("Value cannot be null. (Parameter 'workItemToAdd')", exception.Message);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_WorkItem_NotExists(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, linkType);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        Assert.StartsWith("TF401232: ", exception.Message);
        Assert.Contains($" {workItemToAdd.Id} ", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_WorkItem_Himself(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItem, linkType);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        Assert.Equal("Impossible d'ajouter une relation sur lui même (Parameter 'workItemToAdd')", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_AddRelations_Child() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Child);
        AddAndCheckRelations(ref relations, workItem, LinkType.Child, workItemToAdd.Id);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd2, LinkType.Child);
        AddAndCheckRelations(ref relations, workItem, LinkType.Child, workItemToAdd2.Id);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    private static async Task<WorkItem> AddWorkItemRelationAsync(AzureDevOpsTools azureDevOpsTools, int workItemId,
                                                                 WorkItem workItemToAdd, LinkType linkType) {
        await azureDevOpsTools.AddWorkItemRelationAsync(workItemId, workItemToAdd, linkType);
        return await azureDevOpsTools.GetWorkItemAsync(workItemId);
    }

    private static void AddAndCheckRelations(ref Dictionary<LinkType, List<int>> relations, WorkItem workItem, LinkType linkType,
                                             int workItemToAddId) {
        if (!relations.ContainsKey(linkType))
            relations.Add(linkType, new List<int>());
        relations[linkType].Add(workItemToAddId);
        WorkItemAssert.CheckRelations(relations, workItem);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_AddRelations_Parent() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Parent);
        AddAndCheckRelations(ref relations, workItem, LinkType.Parent, workItemToAdd.Id);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Parent);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        Assert.StartsWith("TF201036: ", exception.Message);
        Assert.Contains(" Parent ", exception.Message);
        Assert.Contains($" {workItem.Id} ", exception.Message);
        Assert.Contains($" {workItemToAdd2.Id} ", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_AddRelations_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Related);
        AddAndCheckRelations(ref relations, workItem, LinkType.Related, workItemToAdd.Id);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd2, LinkType.Related);
        AddAndCheckRelations(ref relations, workItem, LinkType.Related, workItemToAdd2.Id);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_AddAllRelations() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Child);
        AddAndCheckRelations(ref relations, workItem, LinkType.Child, workItemToAdd.Id);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd2, LinkType.Parent);
        AddAndCheckRelations(ref relations, workItem, LinkType.Parent, workItemToAdd2.Id);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd3, LinkType.Related);
        AddAndCheckRelations(ref relations, workItem, LinkType.Related, workItemToAdd3.Id);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Child_Parent() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Child);
        AddAndCheckRelations(ref relations, workItem, LinkType.Child, workItemToAdd.Id);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Parent);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        Assert.StartsWith("TF201035: ", exception.Message);
        Assert.Contains(" Parent ", exception.Message);
        Assert.Contains($" {workItem.Id} ", exception.Message);
        Assert.Contains($" {workItemToAdd.Id} ", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Child_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Child);
        AddAndCheckRelations(ref relations, workItem, LinkType.Child, workItemToAdd.Id);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Related);
        AddAndCheckRelations(ref relations, workItem, LinkType.Related, workItemToAdd.Id);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Parent_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Parent);
        AddAndCheckRelations(ref relations, workItem, LinkType.Parent, workItemToAdd.Id);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Related);
        AddAndCheckRelations(ref relations, workItem, LinkType.Related, workItemToAdd.Id);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Order_Child() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItemToAdd1 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd3, LinkType.Child);
        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd1, LinkType.Child);
        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Child);
        workItem = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);

        var relations = new Dictionary<LinkType, List<int>> {
            {
                LinkType.Child, new List<int> {
                    workItemToAdd1.Id,
                    workItemToAdd2.Id,
                    workItemToAdd3.Id
                }
            }
        };
        WorkItemAssert.CheckRelations(relations, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd1.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Order_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItemToAdd1 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd3, LinkType.Related);
        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd1, LinkType.Related);
        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Related);
        workItem = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);

        var relations = new Dictionary<LinkType, List<int>> {
            {
                LinkType.Related, new List<int> {
                    workItemToAdd1.Id,
                    workItemToAdd2.Id,
                    workItemToAdd3.Id
                }
            }
        };
        WorkItemAssert.CheckRelations(relations, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd1.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
    }
}
