namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Les informations du WorkItem
    /// </summary>
    public class WorkItem
    {
        /// <summary>
        /// L'identifiant
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Url des données
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// La zone
        /// </summary>
        public string AreaPath { get; set; }

        /// <summary>
        /// Le projet
        /// </summary>
        public string TeamProject { get; set; }

        /// <summary>
        /// L'itération
        /// </summary>
        public string IterationPath { get; set; }

        /// <summary>
        /// Le titre
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// L'état
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Le type
        /// </summary>
        public string WorkItemType { get; set; }

        /// <summary>
        /// Assigner à une personne
        /// </summary>
        public string AssignedTo { get; set; }

        /// <summary>
        /// Activité
        /// </summary>
        public string Activity { get; set; }
    }
}
