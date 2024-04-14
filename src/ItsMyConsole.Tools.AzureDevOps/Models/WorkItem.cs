using System.Collections.Generic;

namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Les informations du WorkItem
    /// </summary>
    public class WorkItem
    {
        internal WorkItem() { }

        internal string Url { get; set; }
        internal int Rev { get; set; }

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
        public string Project { get; internal set; }

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
        /// Le nom de la personne assignée au WorkItem
        /// </summary>
        public string AssignedToDisplayName { get; internal set; }

        /// <summary>
        /// L'activité du WorkItem
        /// </summary>
        public string Activity { get; internal set; }

        /// <summary>
        /// La description du WorkItem
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Les étapes de reproduction du WorkItem
        /// </summary>
        public string ReproSteps { get; internal set; }

        /// <summary>
        /// Les informations systèmes du WorkItem
        /// </summary>
        public string SystemInfo { get; internal set; }

        /// <summary>
        /// Les critères d'acceptation du WorkItem
        /// </summary>
        public string AcceptanceCriteria { get; internal set; }

        /// <summary>
        /// La liste des identifiants des WorkItems enfants
        /// </summary>
        public int[] Children { get; internal set; }

        /// <summary>
        /// L'identifiant du WorkItem parent
        /// </summary>
        public int? Parent { get; internal set; }

        /// <summary>
        /// La liste des identifiants des WorkItems associés
        /// </summary>
        public int[] Related { get; internal set; }

        /// <summary>
        /// La liste des identifiants des WorkItems prédécesseurs
        /// </summary>
        public int[] Predecessors { get; internal set; }

        /// <summary>
        /// La liste des identifiants des WorkItems successeurs
        /// </summary>
        public int[] Successors { get; internal set; }

        /// <summary>
        /// Indicateur si des ensembles de modifications sont liés au Workitem
        /// </summary>
        public bool IsFixedInChangeset { get; internal set; }

        /// <summary>
        /// Liste des balises du WorkItem
        /// </summary>
        public string[] Tags { get; internal set; }

        /// <summary>
        /// L'effort du WorkItem
        /// </summary>
        public double? Effort { get; internal set; }

        /// <summary>
        /// Les champs personnalisés du WorkItem
        /// </summary>
        public Dictionary<string, object> CustomFields { get; internal set; }

        /// <summary>
        /// L'estimation d'origine
        /// </summary>
        public double? OriginalEstimate { get; internal set; }

        /// <summary>
        /// Le travail restant
        /// </summary>
        public double? RemainingWork { get; internal set; }

        /// <summary>
        /// Le travail accompli
        /// </summary>
        public double? CompletedWork { get; internal set; }
    }
}
