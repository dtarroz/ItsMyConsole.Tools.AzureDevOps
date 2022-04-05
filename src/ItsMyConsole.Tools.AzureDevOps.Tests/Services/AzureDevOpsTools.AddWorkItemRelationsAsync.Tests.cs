using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;
using ItsMyConsole.Tools.AzureDevOps.Tests.Data;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Services;

public class AzureDevOpsTools_AddWorkItemRelationsAsync_Tests
{
    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_Server_Url_Fail(LinkType linkType) {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "Url_Fail";
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        await Assert.ThrowsAsync<HttpRequestException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(1, new List<WorkItem> { workItem }, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_Server_PersonalAccessToken_Fail(LinkType linkType) {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.Name = "PersonalAccessToken_Fail";
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(1, new List<WorkItem> { workItem }, linkType);
        });

        Assert.Equal($"Vous n'avez pas les accès au serveur Azure DevOps '{azureDevOpsServer.Name}'", exception.Message);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_Id_Negative(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(-1, new List<WorkItem> { workItem }, linkType);
        });

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')", exception.Message);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_Id_Zero(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(0, new List<WorkItem> { workItem }, linkType);
        });

        Assert.Equal("L'identifiant du WorkItem doit être un nombre strictement positif (Parameter 'workItemId')", exception.Message);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_Id_NotExists(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(999999, new List<WorkItem> { workItem }, linkType);
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
    public async Task AddWorkItemRelationsAsync_WorkItem_Null(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        Exception exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(1, null, linkType);
        });

        Assert.Equal("Value cannot be null. (Parameter 'workItemsToAdd')", exception.Message);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_WorkItem_List_Null(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> {
                                                                 workItemToAdd,
                                                                 null
                                                             }, linkType);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        if (linkType == LinkType.Parent)
            Assert.Equal("Un WorkItem possède un seul parent (Parameter 'workItemsToAdd')", exception.Message);
        else
            Assert.Equal("Un WorkItem à ajouter est à null (Parameter 'workItemsToAdd')", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_WorkItem_List_Parent_Null() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { null }, LinkType.Parent);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        Assert.Equal("Un WorkItem à ajouter est à null (Parameter 'workItemsToAdd')", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_WorkItem_List_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem>(), LinkType.Child);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        WorkItemAssert.CheckRelations(null, workItemUpdated);
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_WorkItem_NotExists(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemDelete = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemDelete.Id);
        List<WorkItem> workItemsToAdd = new List<WorkItem> { workItemDelete };
        if (linkType != LinkType.Parent)
            workItemsToAdd.Add(workItemToAdd);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, workItemsToAdd, linkType);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        Assert.StartsWith("TF401232: ", exception.Message);
        Assert.Contains($" {workItemDelete.Id} ", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_WorkItem_Himself(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> {
                                                                 workItem,
                                                                 workItemToAdd
                                                             }, linkType);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        if (linkType == LinkType.Parent)
            Assert.Equal("Un WorkItem possède un seul parent (Parameter 'workItemsToAdd')", exception.Message);
        else
            Assert.Equal("Impossible d'ajouter une relation sur lui même (Parameter 'workItemsToAdd')", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_AddRelations_Child() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> {
                                                                                        workItemToAdd,
                                                                                        workItemToAdd2
                                                                                    }, LinkType.Child);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Child, new[] { workItemToAdd.Id, workItemToAdd2.Id });
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    private static void AddAndCheckRelations(ref Dictionary<LinkType, List<int>> relations, WorkItem workItem, LinkType linkType,
                                             IEnumerable<int> workItemToAddIds) {
        if (!relations.ContainsKey(linkType))
            relations.Add(linkType, new List<int>());
        relations[linkType].AddRange(workItemToAddIds);
        WorkItemAssert.CheckRelations(relations, workItem);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_AddRelations_Parent_Multi() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

        Exception exception = await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> {
                                                                 workItemToAdd,
                                                                 workItemToAdd2
                                                             }, LinkType.Parent);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItem);
        Assert.Equal("Un WorkItem possède un seul parent (Parameter 'workItemsToAdd')", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_AddRelations_Parent_One() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated =
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd }, LinkType.Parent);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Parent, new[] { workItemToAdd.Id });
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd2 }, LinkType.Parent);
        });

        await CheckWorkItemNotModifiedAsync(azureDevOpsTools, workItemUpdated);
        Assert.StartsWith("TF201036: ", exception.Message);
        Assert.Contains(" Parent ", exception.Message);
        Assert.Contains($" {workItem.Id}", exception.Message);
        Assert.Contains($" {workItemToAdd2.Id}", exception.Message);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_AddRelations_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated = await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> {
                                                                                        workItemToAdd,
                                                                                        workItemToAdd2
                                                                                    }, LinkType.Related);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Related, new[] { workItemToAdd.Id, workItemToAdd2.Id });
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_AddAllRelations() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated =
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd }, LinkType.Child);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Child, new[] { workItemToAdd.Id });
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated =
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd2 }, LinkType.Parent);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Parent, new[] { workItemToAdd2.Id });
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated =
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd3 }, LinkType.Related);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Related, new[] { workItemToAdd3.Id });
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_Same_Child_Parent() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated =
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd }, LinkType.Child);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Child, new[] { workItemToAdd.Id });
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        Exception exception = await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd }, LinkType.Parent);
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
    public async Task AddWorkItemRelationsAsync_Same_Child_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated =
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd }, LinkType.Child);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Child, new[] { workItemToAdd.Id });
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd },
                                                                           LinkType.Related);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Related, new[] { workItemToAdd.Id });
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_Same_Parent_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        WorkItem workItemUpdated =
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd }, LinkType.Parent);
        WorkItem workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Parent, new[] { workItemToAdd.Id });
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        workItemUpdated = await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd },
                                                                           LinkType.Related);
        workItemGet = await azureDevOpsTools.GetWorkItemAsync(workItem.Id);
        AddAndCheckRelations(ref relations, workItemUpdated, LinkType.Related, new[] { workItemToAdd.Id });
        Assert.Equal(1, workItemUpdated.Rev);
        WorkItemAssert.Equal(workItemUpdated, workItemGet);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_Order_Child() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItemToAdd1 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

        await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> {
                                                             workItemToAdd3,
                                                             workItemToAdd1,
                                                             workItemToAdd2
                                                         }, LinkType.Child);
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
    public async Task AddWorkItemRelationsAsync_Order_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemCreateFields workItemCreateFields = ConfigForTests.GetWorkItemCreateFields();
        WorkItem workItemToAdd1 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemCreateFields);

        await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> {
                                                             workItemToAdd3,
                                                             workItemToAdd1,
                                                             workItemToAdd2
                                                         }, LinkType.Related);
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
}
