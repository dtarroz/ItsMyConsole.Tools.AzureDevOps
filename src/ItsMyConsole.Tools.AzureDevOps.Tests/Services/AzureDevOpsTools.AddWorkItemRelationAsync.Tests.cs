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

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')", exception.Message);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_Id_Zero(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(0, workItem, linkType);
        });

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')", exception.Message);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_Id_NotExists(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

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
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
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
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

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
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Child);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Child, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Child);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Child, workItemToAdd2.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
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
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Parent);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Parent, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Parent);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItemUpdated);
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
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Related);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Related, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Related);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Related, workItemToAdd2.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_AddRelations_Successor() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Successor);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Successor, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Successor);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Successor, workItemToAdd2.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_AddRelations_Predecessor() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Predecessor);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Predecessor, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Predecessor);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Predecessor, workItemToAdd2.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_AddAllRelations() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd4 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd5 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Child);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Child, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Parent);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Parent, workItemToAdd2.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd3, LinkType.Related);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Related, workItemToAdd3.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd4, LinkType.Successor);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Successor, workItemToAdd4.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd5, LinkType.Predecessor);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Predecessor, workItemToAdd5.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd4.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd5.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Child_Parent() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Child);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Child, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Parent);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItemUpdated);
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
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Child);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Child, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Related);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Related, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Parent_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Parent);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Parent, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Related);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Related, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Successor_Child() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Successor);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Successor, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Child);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Child, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Successor_Parent() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Successor);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Successor, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Parent);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Parent, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Successor_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Successor);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Successor, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Related);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Related, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Predecessor_Child() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Predecessor);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Predecessor, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Child);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Child, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Predecessor_Parent() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Predecessor);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Predecessor, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Parent);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Parent, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Predecessor_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Predecessor);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Predecessor, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Related);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Related, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Predecessor_Successor() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Predecessor);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Predecessor, workItemToAdd.Id);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Successor);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItemUpdated);
        Assert.StartsWith("TF201035: ", exception.Message);
        Assert.Contains(" Successor ", exception.Message);
        Assert.Contains($" {workItem.Id} ", exception.Message);
        Assert.Contains($" {workItemToAdd.Id} ", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Order_Child() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateField = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItemToAdd1 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateField);
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateField);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateField);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateField);

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
        Assert.Equal(1, workItem.Rev);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd1.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Order_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItemToAdd1 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

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
        Assert.Equal(1, workItem.Rev);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd1.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Order_Successor() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItemToAdd1 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd3, LinkType.Successor);
        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd1, LinkType.Successor);
        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Successor);
        workItem = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);

        var relations = new Dictionary<LinkType, List<int>> {
            {
                LinkType.Successor, new List<int> {
                    workItemToAdd1.Id,
                    workItemToAdd2.Id,
                    workItemToAdd3.Id
                }
            }
        };
        WorkItemAssert.CheckRelations(relations, workItem);
        Assert.Equal(1, workItem.Rev);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd1.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Order_Predecessor() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItemToAdd1 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd3, LinkType.Predecessor);
        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd1, LinkType.Predecessor);
        await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Predecessor);
        workItem = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);

        var relations = new Dictionary<LinkType, List<int>> {
            {
                LinkType.Predecessor, new List<int> {
                    workItemToAdd1.Id,
                    workItemToAdd2.Id,
                    workItemToAdd3.Id
                }
            }
        };
        WorkItemAssert.CheckRelations(relations, workItem);
        Assert.Equal(1, workItem.Rev);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd1.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
    }
}
