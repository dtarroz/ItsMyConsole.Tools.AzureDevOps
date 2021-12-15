using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace ItsMyConsole.Tools.AzureDevOps
{
    internal static class WebApiWorkItemExtensions
    {
        public static WorkItem ToModel(this Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem) {
            return new WorkItem {
                Id = workItem.Id ?? 0,
                Url = workItem.Url,
                AreaPath = workItem.GetFieldValue<string>("System.AreaPath"),
                TeamProject = workItem.GetFieldValue<string>("System.TeamProject"),
                IterationPath = workItem.GetFieldValue<string>("System.IterationPath"),
                Title = workItem.GetFieldValue<string>("System.Title"),
                State = workItem.GetFieldValue<string>("System.State"),
                WorkItemType = workItem.GetFieldValue<string>("System.WorkItemType"),
                AssignedToDisplayName = workItem.GetFieldValue<IdentityRef>("System.AssignedTo")?.DisplayName,
                Activity = workItem.GetFieldValue<string>("Microsoft.VSTS.Common.Activity"),
                Description = workItem.GetFieldValue<string>("System.Description"),
                ReproSteps = workItem.GetFieldValue<string>("Microsoft.VSTS.TCM.ReproSteps"),
                SystemInfo = workItem.GetFieldValue<string>("Microsoft.VSTS.TCM.SystemInfo"),
                AcceptanceCriteria = workItem.GetFieldValue<string>("Microsoft.VSTS.Common.AcceptanceCriteria"),
                Childs = workItem.GetRelationIds(LinkType.Child),
                Parents = workItem.GetRelationIds(LinkType.Parent),
                Related = workItem.GetRelationIds(LinkType.Related),
                IsFixedInChangeset =
                    workItem.Relations?.Any(r => r.Attributes.ContainsKey("name")
                                                 && r.Attributes["name"].ToString() == "Fixed in Changeset") ?? false,
                Tags = workItem.GetFieldValue<string>("System.Tags")?.Split(';').Select(t => t.Trim()).ToArray()
            };
        }

        private static T GetFieldValue<T>(this Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem,
                                          string key) {
            return workItem.Fields.ContainsKey(key) ? (T)workItem.Fields[key] : default;
        }

        private static int[] GetRelationIds(this Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem,
                                            LinkType linkType) {
            return workItem.Relations?.Where(r => r.Rel == linkType.GetName())
                           .Select(r => r.Url.Substring(r.Url.LastIndexOf('/') + 1))
                           .Select(wi => Convert.ToInt32(wi))
                           .ToArray();
        }
    }
}
