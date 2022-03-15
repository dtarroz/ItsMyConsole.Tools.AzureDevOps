using System.Collections.Generic;

namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Champs à initialiser sur un WorkItem
    /// </summary>
    public class WorkItemCreateFields : WorkItemFields
    {
        /// <summary>
        /// Liste des balises du WorkItem
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
    }
}
