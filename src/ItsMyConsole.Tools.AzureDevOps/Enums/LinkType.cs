namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Type de lien entre WorkItem
    /// </summary>
    public enum LinkType
    {
        /// <summary>
        /// Lien Enfant
        /// </summary>
        Child,

        /// <summary>
        /// Lien Parent
        /// </summary>
        Parent,

        /// <summary>
        /// Lien Associé
        /// </summary>
        Related,

        /// <summary>
        /// Prédécesseur
        /// </summary>
        Predecessor,

        /// <summary>
        /// Successeur
        /// </summary>
        Successor
    }
}
