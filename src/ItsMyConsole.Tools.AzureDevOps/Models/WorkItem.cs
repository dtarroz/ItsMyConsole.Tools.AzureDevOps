namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Les informations du WorkItem
    /// </summary>
    public class WorkItem
    {
        internal WorkItem() { }

        /// <summary>
        /// L'identifiant
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Url des données
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// La zone
        /// </summary>
        public string AreaPath { get; internal set; }

        /// <summary>
        /// Le projet
        /// </summary>
        public string TeamProject { get; internal set; }

        /// <summary>
        /// L'itération
        /// </summary>
        public string IterationPath { get; internal set; }

        /// <summary>
        /// Le titre
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// L'état
        /// </summary>
        public string State { get; internal set; }

        /// <summary>
        /// Le type
        /// </summary>
        public string WorkItemType { get; internal set; }

        /// <summary>
        /// Assigner à une personne
        /// </summary>
        public string AssignedTo { get; internal set; }

        /// <summary>
        /// Activité
        /// </summary>
        public string Activity { get; internal set; }

        /// <summary>
        /// Liste des WorkItems enfants
        /// </summary>
        public int[] Childs { get; internal set; }

        /// <summary>
        /// Liste des WorkItems parents
        /// </summary>
        public int[] Parents { get; internal set; }

        /// <summary>
        /// Liste des WorkItems associés
        /// </summary>
        public int[] Related { get; internal set; }
    }
}
