using System;

namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Les informations d'une itération d'un projet
    /// </summary>
    public class Iteration
    {
        /// <summary>
        /// Le nom de l'itération
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Le chemin relatif de l'itération
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// Date de début de l'itération
        /// </summary>
        public DateTime? StartDate { get; internal set; }

        /// <summary>
        /// Date de fin de l'itération
        /// </summary>
        public DateTime? FinishDate { get; internal set; }
    }
}
