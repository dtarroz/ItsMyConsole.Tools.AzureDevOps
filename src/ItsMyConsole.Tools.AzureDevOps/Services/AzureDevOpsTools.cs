using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Outils pour Azure Dev Ops
    /// </summary>
    public class AzureDevOpsTools
    {
        private readonly AzureDevOpsServer _azureDevOpsServer;

        static AzureDevOpsTools()
        {
            Environment.SetEnvironmentVariable("VSS_ALLOW_UNSAFE_BASICAUTH", "true");
        }

        internal AzureDevOpsTools(AzureDevOpsServer azureDevOpsServer)
        {
            _azureDevOpsServer = azureDevOpsServer;
        }

        /// <summary>
        /// Récupération des informations sur un WorkItem par son identifiant
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem</param>
        public async Task<WorkItem> GetWorkItemAsync(int workItemId)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient())
                return (await workItemTrackingHttpClient.GetWorkItemAsync(workItemId)).ToModel();
        }

        private WorkItemTrackingHttpClient GetWorkItemTrackingHttpClient()
        {
            VssBasicCredential credentials = new VssBasicCredential("", _azureDevOpsServer.PersonalAccessToken);
            return new WorkItemTrackingHttpClient(new Uri(_azureDevOpsServer.Url), credentials);
        }

        /// <summary>
        /// Récupération des itérations courantes d'un projet
        /// </summary>
        /// <param name="project">Le nom du projet</param>
        /// <param name="team">Le nom de l'équipe</param>
        public async Task<List<TeamSettingsIteration>> GetCurrentTeamIterationsAsync(string project, string team = null)
        {
            using (WorkHttpClient workHttpClient = GetWorkHttpClient())
                return await workHttpClient.GetTeamIterationsAsync(new TeamContext(project, team), "Current");
        }

        private WorkHttpClient GetWorkHttpClient()
        {
            VssBasicCredential credentials = new VssBasicCredential("", _azureDevOpsServer.PersonalAccessToken);
            return new WorkHttpClient(new Uri(_azureDevOpsServer.Url), credentials);
        }

        /// <summary>
        /// Création d'un WorkItem
        /// </summary>
        /// <param name="workItemFields">Les champs du WorkItem</param>
        /// <returns>Le WorkItem crée</returns>
        public async Task<WorkItem> CreateWorkItemAsync(WorkItemFields workItemFields)
        {
            if (workItemFields == null)
                throw new ArgumentNullException(nameof(workItemFields));
            if (string.IsNullOrEmpty(workItemFields.TeamProject))
                throw new ArgumentException("L'équipe est obligatoire", nameof(workItemFields.TeamProject));
            if (string.IsNullOrEmpty(workItemFields.WorkItemType))
                throw new ArgumentException("Le type est obligatoire", nameof(workItemFields.WorkItemType));
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient())
                return (await workItemTrackingHttpClient.CreateWorkItemAsync(CreateJsonPatchDocument(workItemFields),
                                                                            workItemFields.TeamProject,
                                                                            workItemFields.WorkItemType)).ToModel();
        }

        private JsonPatchDocument CreateJsonPatchDocument(WorkItemFields workItemFields)
        {
            Dictionary<string, string> fields = new Dictionary<string, string>
            {
                { "/fields/System.AreaPath", workItemFields.AreaPath },
                { "/fields/System.TeamProject", workItemFields.TeamProject },
                { "/fields/System.IterationPath", workItemFields.IterationPath },
                { "/fields/System.Title", workItemFields.Title },
                { "/fields/System.State", workItemFields.State },
                { "/fields/System.WorkItemType", workItemFields.WorkItemType },
                { "/fields/System.AssignedTo", workItemFields.AssignedTo },
                { "/fields/System.Microsoft.VSTS.Common.Activity", workItemFields.Activity }
            };
            JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
            foreach(var field in fields)
                AddInJsonPatchDocument(Operation.Replace, field.Key, field.Value, ref jsonPatchDocument);
            return jsonPatchDocument;
        }

        private void AddInJsonPatchDocument(Operation operation, string field, object value,
                                            ref JsonPatchDocument jsonPatchDocument)
        {
            if (value != null)
            {
                jsonPatchDocument.Add(new JsonPatchOperation
                {
                    Operation = operation,
                    Path = field,
                    Value = value
                });
            }
        }

        /// <summary>
        /// Mise à jour d'un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem</param>
        /// <param name="workItemFields">Les champs du WorkItem à modifier</param>
        public async Task UpdateWorkItemAsync(int workItemId, WorkItemFields workItemFields)
        {
            if (workItemFields == null)
                throw new ArgumentNullException(nameof(workItemFields));
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient())
                await workItemTrackingHttpClient.UpdateWorkItemAsync(CreateJsonPatchDocument(workItemFields), workItemId);
        }

        /// <summary>
        /// Ajoute plusieurs relations de WorkItems à un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem qui va recevoir la relation</param>
        /// <param name="workItemToAdd">le WorkItem à ajouter</param>
        /// <param name="linkTypes">Le type de lien (par exemple : System.LinkTypes.Hierarchy-Forward)</param>
        public async Task AddWorkItemRelationsAsync(int workItemId, WorkItem workItemToAdd,
                                                    string linkTypes)
        {
            await AddWorkItemRelationsAsync(workItemId, new List<WorkItem> { workItemToAdd }, linkTypes);
        }

        /// <summary>
        /// Ajoute plusieurs relations de WorkItems à un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem qui va recevoir la relation</param>
        /// <param name="workItemsToAdd">Les WorkItems à ajouter</param>
        /// <param name="linkTypes">Le type de lien (par exemple : System.LinkTypes.Hierarchy-Forward)</param>
        public async Task AddWorkItemRelationsAsync(int workItemId, List<WorkItem> workItemsToAdd,
                                                    string linkTypes)
        {
            if (workItemsToAdd == null)
                throw new ArgumentNullException(nameof(workItemsToAdd));
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient())
            {
                JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
                foreach (WorkItem workItem in workItemsToAdd)
                {
                    AddInJsonPatchDocument(Operation.Add, "/relations/-", new
                    {
                        rel = linkTypes,
                        url = workItem.Url,
                    }, ref jsonPatchDocument);
                }
                await workItemTrackingHttpClient.UpdateWorkItemAsync(jsonPatchDocument, workItemId);
            }
        }
    }
}
