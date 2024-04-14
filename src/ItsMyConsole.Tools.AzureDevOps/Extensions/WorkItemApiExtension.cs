using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsMyConsole.Tools.AzureDevOps
{
    internal static class WorkItemApiExtension
    {
        public static WorkItem ToModel(this WorkItemApi workItemApi) {
            return new WorkItem {
                Id = workItemApi.Id,
                Url = workItemApi.Url,
                Rev = workItemApi.Rev,
                AreaPath = workItemApi.GetFieldValue<string>("System.AreaPath"),
                Project = workItemApi.GetFieldValue<string>("System.TeamProject"),
                IterationPath = workItemApi.GetFieldValue<string>("System.IterationPath"),
                Title = workItemApi.GetFieldValue<string>("System.Title"),
                State = workItemApi.GetFieldValue<string>("System.State"),
                WorkItemType = workItemApi.GetFieldValue<string>("System.WorkItemType"),
                AssignedToDisplayName = workItemApi.GetFieldValue<WorkItemApiFieldsAssignedTo>("System.AssignedTo")?.DisplayName,
                Activity = workItemApi.GetFieldValue<string>("Microsoft.VSTS.Common.Activity"),
                Description = workItemApi.GetFieldValue<string>("System.Description"),
                ReproSteps = workItemApi.GetFieldValue<string>("Microsoft.VSTS.TCM.ReproSteps"),
                SystemInfo = workItemApi.GetFieldValue<string>("Microsoft.VSTS.TCM.SystemInfo"),
                AcceptanceCriteria = workItemApi.GetFieldValue<string>("Microsoft.VSTS.Common.AcceptanceCriteria"),
                Children = workItemApi.GetRelationIds(LinkType.Child),
                Parent = workItemApi.GetRelationIds(LinkType.Parent)?.FirstOrDefault(),
                Related = workItemApi.GetRelationIds(LinkType.Related),
                Predecessors = workItemApi.GetRelationIds(LinkType.Predecessor),
                Successors = workItemApi.GetRelationIds(LinkType.Successor),
                IsFixedInChangeset = workItemApi.Relations?.Any(r => r.Attributes.Name == "Fixed in Changeset") ?? false,
                Tags = workItemApi.GetFieldValue<string>("System.Tags")?.Split(';').Select(t => t.Trim()).ToArray(),
                Effort = workItemApi.GetFieldValue<double?>("Microsoft.VSTS.Scheduling.Effort"),
                OriginalEstimate = workItemApi.GetFieldValue<double?>("Microsoft.VSTS.Scheduling.OriginalEstimate"),
                CustomFields = workItemApi.GetCustomFields()
            };
        }

        private static T GetFieldValue<T>(this WorkItemApi workItemApi, string key) {
            return workItemApi.Fields.ContainsKey(key) ? (T)workItemApi.Fields[key] : default;
        }

        private static int[] GetRelationIds(this WorkItemApi workItemApi, LinkType linkType) {
            List<int> ids = workItemApi.Relations?.Where(r => r.Rel == linkType.GetName())
                                       .Select(r => r.Url.Substring(r.Url.LastIndexOf('/') + 1))
                                       .Select(wi => Convert.ToInt32(wi))
                                       .ToList();
            ids?.Sort();
            return ids == null || ids.Count == 0 ? null : ids.ToArray();
        }

        private static Dictionary<string, object> GetCustomFields(this WorkItemApi workItemApi) {
            List<KeyValuePair<string, object>> customFields = workItemApi.Fields.Where(f => IsCustomField(f.Key)).ToList();
            return customFields.Any() ? customFields.ToDictionary(f => f.Key, f => f.Value) : null;
        }

        private static bool IsCustomField(string field) {
            return !field.StartsWith("System.") && !field.StartsWith("Microsoft.");
        }
    }
}
