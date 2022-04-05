using System.Collections.Generic;

namespace ItsMyConsole.Tools.AzureDevOps
{
    internal class WorkItemApi
    {
        public int Id { get; set; }
        public Dictionary<string, object> Fields { get; set; }
        public WorkItemApiRelation[] Relations { get; set; }
        public string Url { get; set; }
        public int Rev { get; set; }
    }

    internal class WorkItemApiRelation
    {
        public string Rel { get; set; }
        public string Url { get; set; }
        public WorkItemApiRelationAttributes Attributes { get; set; }
    }

    internal class WorkItemApiRelationAttributes
    {
        public string Name { get; set; }
    }

    internal class WorkItemApiFieldsAssignedTo
    {
        public string DisplayName { get; set; }
    }
}
