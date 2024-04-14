namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Champs sur un WorkItem
    /// </summary>
    public class WorkItemFields
    {
        internal WorkItemFields() { }

        /// <summary>
        /// La zone du WorkItem
        /// </summary>
        public string AreaPath { get; set; }

        /// <summary>
        /// Le projet du WorkItem
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// L'itération du WorkItem
        /// </summary>
        public string IterationPath { get; set; }

        /// <summary>
        /// Le titre du WorkItem
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// L'état du WorkItem
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Le type du WorkItem
        /// </summary>
        public string WorkItemType { get; set; }

        /// <summary>
        /// Le nom de la personne à assigner au WorkItem
        /// </summary>
        public string AssignedToDisplayName { get; set; }

        /// <summary>
        /// L'activité du WorkItem
        /// </summary>
        public string Activity { get; set; }

        /// <summary>
        /// La description du WorkItem
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Les étapes de reproduction du WorkItem
        /// </summary>
        public string ReproSteps { get; set; }

        /// <summary>
        /// Les informations systèmes du WorkItem
        /// </summary>
        public string SystemInfo { get; set; }

        /// <summary>
        /// Les critères d'acceptation du WorkItem
        /// </summary>
        public string AcceptanceCriteria { get; set; }

        /// <summary>
        /// L'effort du WorkItem
        /// </summary>
        public double? Effort { get; set; }

        /// <summary>
        /// L'estimation d'origine
        /// </summary>
        public double? OriginalEstimate { get; set; }
    }
}
