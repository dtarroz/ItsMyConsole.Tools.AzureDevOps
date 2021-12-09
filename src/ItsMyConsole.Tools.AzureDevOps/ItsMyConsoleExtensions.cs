using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Extension de ItsMyConsole pour inclure les outils Azure Dev Ops
    /// </summary>
    public static class ItsMyConsoleExtensions
    {
        private static readonly List<AzureDevOpsServer> AzureDevOpsServers = new List<AzureDevOpsServer>();

        /// <summary>
        /// Configuration d'un serveur Azure Dev Ops pour son utilisation pendant l'exécution d'une ligne de commande
        /// </summary>
        /// <param name="ccli">La console qui configure Azure Dev Ops</param>
        /// <param name="azureDevOpsServer">Les informations d'un serveur Azure Dev Ops</param>
        public static void AddAzureDevOpsServer(this ConsoleCommandLineInterpreter ccli, AzureDevOpsServer azureDevOpsServer) {
            if (AzureDevOpsServers.Any(a => a.Name == azureDevOpsServer.Name))
                throw new ArgumentException("Nom déjà présent", nameof(azureDevOpsServer.Name));
            if (string.IsNullOrWhiteSpace(azureDevOpsServer.Url))
                throw new ArgumentException("Url Obligatoire", nameof(azureDevOpsServer.Url));
            if (string.IsNullOrWhiteSpace(azureDevOpsServer.PersonalAccessToken))
                throw new ArgumentException("Token Obligatoire", nameof(azureDevOpsServer.PersonalAccessToken));
            AzureDevOpsServers.Add(azureDevOpsServer);
        }

        /// <summary>
        /// L'accès aux serveurs Azure Dev Ops configurés
        /// </summary>
        /// <param name="commandTools">Les outils de commandes pour accéder aux serveurs Azure Dev Ops</param>
        /// <param name="name">Nom du serveur Azure Dev Ops configuré</param>
        public static AzureDevOpsTools AzureDevOps(this CommandTools commandTools, string name = null) {
            AzureDevOpsServer azureDevOpsServer = GetAzureDevOpsServer(name);
            return new AzureDevOpsTools(azureDevOpsServer);
        }

        private static AzureDevOpsServer GetAzureDevOpsServer(string name) {
            AzureDevOpsServer server = AzureDevOpsServers.FirstOrDefault(a => a.Name == name);
            if (server == null)
                throw new Exception($"Le serveur Azure Dev Ops '{name}' n'a pas été trouvé");
            return server;
        }
    }
}
