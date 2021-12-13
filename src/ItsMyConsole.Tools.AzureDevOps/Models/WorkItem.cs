namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Les informations du WorkItem
    /// </summary>
    public class WorkItem
    {
        internal WorkItem() { }

        internal string Url { get; set; }

        /// <summary>
        /// L'identifiant du WorkItem
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// La zone du WorkItem
        /// </summary>
        public string AreaPath { get; internal set; }

        /// <summary>
        /// Le projet du WorkItem
        /// </summary>
        public string TeamProject { get; internal set; }

        /// <summary>
        /// L'itération du WorkItem
        /// </summary>
        public string IterationPath { get; internal set; }

        /// <summary>
        /// Le titre du WorkItem
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// L'état du WorkItem
        /// </summary>
        public string State { get; internal set; }

        /// <summary>
        /// Le type du WorkItem
        /// </summary>
        public string WorkItemType { get; internal set; }

        /// <summary>
        /// La personne assignée au WorkItem
        /// </summary>
        public string AssignedTo { get; internal set; }

        /// <summary>
        /// L'activité du WorkItem
        /// </summary>
        public string Activity { get; internal set; }

        /// <summary>
        /// Liste des WorkItems enfants du WorkItem
        /// </summary>
        public int[] Childs { get; internal set; }

        /// <summary>
        /// Liste des WorkItems parents du WorkItem
        /// </summary>
        public int[] Parents { get; internal set; }

        /// <summary>
        /// Liste des WorkItems associés du WorkItem
        /// </summary>
        public int[] Related { get; internal set; }
    }
}
