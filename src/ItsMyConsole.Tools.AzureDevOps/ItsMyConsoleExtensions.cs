using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsMyConsole.Tools.AzureDevOps
{
    public static class ItsMyConsoleExtensions
    {
        private static readonly List<AzureDevOpsServer> _azureDevOpsServers = new List<AzureDevOpsServer>();

        /// <summary>
        /// Configuration d'un serveur Azure Dev Ops pour son utilisation pendant l'exécution d'une ligne de commande
        /// </summary>
        /// <param name="azureDevOpsServer">Les informations d'un serveur Azure Dev Ops</param>
        public static void AddAzureDevOpsServer(this ConsoleCommandLineInterpreter _, AzureDevOpsServer azureDevOpsServer)
        {
            if (_azureDevOpsServers.Any(a => a.Name == azureDevOpsServer.Name))
                throw new ArgumentException(nameof(azureDevOpsServer.Name), "Nom déjà présent");
            if (string.IsNullOrWhiteSpace(azureDevOpsServer.Url))
                throw new ArgumentException(nameof(azureDevOpsServer.Url), "Url Obligatoire");
            if (string.IsNullOrWhiteSpace(azureDevOpsServer.PersonalAccessToken))
                throw new ArgumentException(nameof(azureDevOpsServer.PersonalAccessToken), "Token Obligatoire");
            _azureDevOpsServers.Add(azureDevOpsServer);
        }

        /// <summary>
        /// L'accès aux serveurs Azure Dev Ops configurés
        /// </summary>
        public static AzureDevOpsTools AzureDevOps(this CommandTools _, string name = null)
        {
            AzureDevOpsServer azureDevOpsServer = GetAzureDevOpsServer(name);
            return new AzureDevOpsTools(azureDevOpsServer);
        }

        private static AzureDevOpsServer GetAzureDevOpsServer(string name)
        {
            AzureDevOpsServer server = _azureDevOpsServers.FirstOrDefault(a => a.Name == name);
            if (server == null)
                throw new Exception($"Le serveur Azure Dev Ops '{name}' n'a pas été trouvé");
            return server;
        }
    }
}
