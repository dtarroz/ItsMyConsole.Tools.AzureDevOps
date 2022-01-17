using System;
using System.Collections.Generic;
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
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(1, new List<WorkItem> { workItem }, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_Server_PersonalAccessToken_Fail(LinkType linkType) {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(1, new List<WorkItem> { workItem }, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_Id_Negative(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(-1, new List<WorkItem> { workItem }, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_Id_Zero(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(0, new List<WorkItem> { workItem }, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_Id_NotExists(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(999999, new List<WorkItem> { workItem }, linkType);
        });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_WorkItem_Null(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<ArgumentNullException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(1, null, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_WorkItem_List_Null(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> {
                                                                 workItemToAdd,
                                                                 null
                                                             }, linkType);
        });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_WorkItem_List_Empty() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem>(), LinkType.Child);
        WorkItemAssert.CheckRelations(null, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_WorkItem_NotExists(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemDelete = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemDelete.Id);
        List<WorkItem> workItemsToAdd = new List<WorkItem> { workItemDelete };
        if (linkType != LinkType.Parent)
            workItemsToAdd.Add(workItemToAdd);

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, workItemsToAdd, linkType);
        });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationsAsync_WorkItem_Himself(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> {
                                                                 workItem,
                                                                 workItemToAdd
                                                             }, linkType);
        });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_AddRelations_Child() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem> {
                                                       workItemToAdd,
                                                       workItemToAdd2
                                                   }, LinkType.Child);
        AddAndCheckRelations(ref relations, workItem, LinkType.Child, new[] { workItemToAdd.Id, workItemToAdd2.Id });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    private static async Task<WorkItem> AddWorkItemRelationsAsync(AzureDevOpsTools azureDevOpsTools, int workItemId,
                                                                  List<WorkItem> workItemsToAdd, LinkType linkType) {
        await azureDevOpsTools.AddWorkItemRelationsAsync(workItemId, workItemsToAdd, linkType);
        return await azureDevOpsTools.GetWorkItemAsync(workItemId);
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
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> {
                                                                 workItemToAdd,
                                                                 workItemToAdd2
                                                             }, LinkType.Parent);
        });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_AddRelations_Parent_One() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem> { workItemToAdd },
                                                   LinkType.Parent);
        AddAndCheckRelations(ref relations, workItem, LinkType.Parent, new[] { workItemToAdd.Id });

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd2 }, LinkType.Parent);
        });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_AddRelations_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem> {
                                                       workItemToAdd,
                                                       workItemToAdd2
                                                   }, LinkType.Related);
        AddAndCheckRelations(ref relations, workItem, LinkType.Related, new[] { workItemToAdd.Id, workItemToAdd2.Id });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_AddAllRelations() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem> { workItemToAdd },
                                                   LinkType.Child);
        AddAndCheckRelations(ref relations, workItem, LinkType.Child, new[] { workItemToAdd.Id });

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem> { workItemToAdd2 },
                                                   LinkType.Parent);
        AddAndCheckRelations(ref relations, workItem, LinkType.Parent, new[] { workItemToAdd2.Id });

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem> { workItemToAdd3 },
                                                   LinkType.Related);
        AddAndCheckRelations(ref relations, workItem, LinkType.Related, new[] { workItemToAdd3.Id });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_Same_Child_Parent() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem> { workItemToAdd },
                                                   LinkType.Child);
        AddAndCheckRelations(ref relations, workItem, LinkType.Child, new[] { workItemToAdd.Id });

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationsAsync(workItem.Id, new List<WorkItem> { workItemToAdd }, LinkType.Parent);
        });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_Same_Child_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem> { workItemToAdd },
                                                   LinkType.Child);
        AddAndCheckRelations(ref relations, workItem, LinkType.Child, new[] { workItemToAdd.Id });

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem> { workItemToAdd },
                                                   LinkType.Related);
        AddAndCheckRelations(ref relations, workItem, LinkType.Related, new[] { workItemToAdd.Id });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_Same_Parent_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        Dictionary<LinkType, List<int>> relations = new Dictionary<LinkType, List<int>>();

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem> { workItemToAdd },
                                                   LinkType.Parent);
        AddAndCheckRelations(ref relations, workItem, LinkType.Parent, new[] { workItemToAdd.Id });

        workItem = await AddWorkItemRelationsAsync(azureDevOpsTools, workItem.Id, new List<WorkItem> { workItemToAdd },
                                                   LinkType.Related);
        AddAndCheckRelations(ref relations, workItem, LinkType.Related, new[] { workItemToAdd.Id });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_Order_Child() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItemToAdd1 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

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

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd1.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationsAsync_Order_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItemToAdd1 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd3 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

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

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd1.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd3.Id);
    }
}
