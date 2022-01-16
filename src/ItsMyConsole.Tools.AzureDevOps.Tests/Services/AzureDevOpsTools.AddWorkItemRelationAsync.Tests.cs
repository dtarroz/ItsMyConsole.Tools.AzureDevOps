using System;
using System.Collections.Generic;
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
        azureDevOpsServer.Url = "https://noexists.com/";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(1, workItem, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_Server_PersonalAccessToken_Fail(LinkType linkType) {
        AzureDevOpsServer azureDevOpsServer = ConfigForTests.GetAzureDevOpsServer();
        azureDevOpsServer.PersonalAccessToken = "NotExists";
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(azureDevOpsServer);
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(1, workItem, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_Id_Negative(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(-1, workItem, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_Id_Zero(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItem workItem = new WorkItem { Url = "https://noexists.com/" };

        await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(0, workItem, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_Id_NotExists(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(999999, workItem, linkType);
        });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_WorkItem_Null(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());

        await Assert.ThrowsAsync<ArgumentNullException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(1, null, linkType);
        });
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_WorkItem_NotExists(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, linkType);
        });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Theory]
    [ClassData(typeof(LinkTypeData))]
    public async Task AddWorkItemRelationAsync_WorkItem_Himself(LinkType linkType) {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        await Assert.ThrowsAsync<ArgumentException>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItem, linkType);
        });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_AddRelations_Child() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Child);

        var relations = new Dictionary<LinkType, List<int>> { { LinkType.Child, new List<int> { workItemToAdd.Id } } };
        WorkItemAssert.CheckRelations(relations, workItem);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd2, LinkType.Child);

        relations[LinkType.Child].Add(workItemToAdd2.Id);
        WorkItemAssert.CheckRelations(relations, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd2.Id);
    }

    private static async Task<WorkItem> AddWorkItemRelationAsync(AzureDevOpsTools azureDevOpsTools, int workItemId,
                                                                 WorkItem workItemToAdd, LinkType linkType) {
        await azureDevOpsTools.AddWorkItemRelationAsync(workItemId, workItemToAdd, linkType);
        return await azureDevOpsTools.GetWorkItemAsync(workItemId);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_AddRelations_Parent() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd2 = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Parent);

        var relations = new Dictionary<LinkType, List<int>> { { LinkType.Parent, new List<int> { workItemToAdd.Id } } };
        WorkItemAssert.CheckRelations(relations, workItem);

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd2, LinkType.Parent);
        });

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

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Related);

        var relations = new Dictionary<LinkType, List<int>> { { LinkType.Related, new List<int> { workItemToAdd.Id } } };
        WorkItemAssert.CheckRelations(relations, workItem);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd2, LinkType.Related);

        relations[LinkType.Related].Add(workItemToAdd2.Id);
        WorkItemAssert.CheckRelations(relations, workItem);

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

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Child);

        var relations = new Dictionary<LinkType, List<int>> { { LinkType.Child, new List<int> { workItemToAdd.Id } } };
        WorkItemAssert.CheckRelations(relations, workItem);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd2, LinkType.Parent);

        relations.Add(LinkType.Parent, new List<int> { workItemToAdd2.Id });
        WorkItemAssert.CheckRelations(relations, workItem);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd3, LinkType.Related);

        relations.Add(LinkType.Related, new List<int> { workItemToAdd3.Id });
        WorkItemAssert.CheckRelations(relations, workItem);

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

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Child);

        var relations = new Dictionary<LinkType, List<int>> { { LinkType.Child, new List<int> { workItemToAdd.Id } } };
        WorkItemAssert.CheckRelations(relations, workItem);

        await Assert.ThrowsAsync<Exception>(async () => {
            await azureDevOpsTools.AddWorkItemRelationAsync(workItem.Id, workItemToAdd, LinkType.Parent);
        });

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Child_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Child);

        var relations = new Dictionary<LinkType, List<int>> { { LinkType.Child, new List<int> { workItemToAdd.Id } } };
        WorkItemAssert.CheckRelations(relations, workItem);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Related);

        relations.Add(LinkType.Related, new List<int> { workItemToAdd.Id });
        WorkItemAssert.CheckRelations(relations, workItem);

        await azureDevOpsTools.DeleteWorkItemAsync(workItem.Id);
        await azureDevOpsTools.DeleteWorkItemAsync(workItemToAdd.Id);
    }

    [Fact]
    public async Task AddWorkItemRelationAsync_Same_Parent_Related() {
        AzureDevOpsTools azureDevOpsTools = new AzureDevOpsTools(ConfigForTests.GetAzureDevOpsServer());
        WorkItemFields workItemFields = ConfigForTests.GetWorkItemFieldsNew();
        WorkItem workItem = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);
        WorkItem workItemToAdd = await azureDevOpsTools.CreateWorkItemAsync(workItemFields);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Parent);

        var relations = new Dictionary<LinkType, List<int>> { { LinkType.Parent, new List<int> { workItemToAdd.Id } } };
        WorkItemAssert.CheckRelations(relations, workItem);

        workItem = await AddWorkItemRelationAsync(azureDevOpsTools, workItem.Id, workItemToAdd, LinkType.Related);

        relations.Add(LinkType.Related, new List<int> { workItemToAdd.Id });
        WorkItemAssert.CheckRelations(relations, workItem);

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