namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Information sur un serveur Azure DevOps
    /// </summary>
    public class AzureDevOpsServer
    {
        /// <summary>
        /// Nom unique du serveur Azure DevOps qui sert de désignation lors de son utilisation
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// L'URL du serveur Azure DevOps
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Le token d'accès personnel au serveur Azure DevOps
        /// </summary>
        public string PersonalAccessToken { get; set; }
    }
}
