namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Champs à modifier sur un WorkItem
    /// </summary>
    public class WorkItemFields
    {
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
        /// Le nom de la personne à assigner
        /// </summary>
        public string AssignedToDisplayName { get; set; }

        /// <summary>
        /// Activité
        /// </summary>
        public string Activity { get; set; }

        /// <summary>
        /// La description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Les étapes de reproduction
        /// </summary>
        public string ReproSteps { get; set; }

        /// <summary>
        /// Les informations systèmes
        /// </summary>
        public string SystemInfo { get; set; }

        /// <summary>
        /// Les critères d'acceptation
        /// </summary>
        public string AcceptanceCriteria { get; set; }
    }
}
