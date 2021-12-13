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
                AreaPath = workItem.GetFieldValue("System.AreaPath"),
                TeamProject = workItem.GetFieldValue("System.TeamProject"),
                IterationPath = workItem.GetFieldValue("System.IterationPath"),
                Title = workItem.GetFieldValue("System.Title"),
                State = workItem.GetFieldValue("System.State"),
                WorkItemType = workItem.GetFieldValue("System.WorkItemType"),
                AssignedTo = workItem.GetFieldValue("System.AssignedTo"),
                Activity = workItem.GetFieldValue("Microsoft.VSTS.Common.Activity"),
                Childs = workItem.GetRelationIds(LinkType.Child),
                Parents = workItem.GetRelationIds(LinkType.Parent),
                Related = workItem.GetRelationIds(LinkType.Related)
            };
        }

        private static string GetFieldValue(this Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem,
                                            string key) {
            return workItem.Fields.ContainsKey(key) ? workItem.Fields[key].ToString() : null;
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
