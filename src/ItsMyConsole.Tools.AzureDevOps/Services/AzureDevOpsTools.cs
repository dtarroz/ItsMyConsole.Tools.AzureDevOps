using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Outils pour Azure DevOps
    /// </summary>
    public class AzureDevOpsTools
    {
        private readonly AzureDevOpsServer _azureDevOpsServer;

        static AzureDevOpsTools() {
            Environment.SetEnvironmentVariable("VSS_ALLOW_UNSAFE_BASICAUTH", "true");
        }

        internal AzureDevOpsTools(AzureDevOpsServer azureDevOpsServer) {
            _azureDevOpsServer = azureDevOpsServer;
        }

        /// <summary>
        /// Récupération du nom configuré du serveur Azure DevOps
        /// </summary>
        /// <returns></returns>
        public string GetAzureDevOpsServerName() {
            return _azureDevOpsServer.Name;
        }

        /// <summary>
        /// Récupération des informations sur un WorkItem par son identifiant
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem</param>
        public async Task<WorkItem> GetWorkItemAsync(int workItemId) {
            return await TryCatchExceptionAsync(async () => {
                using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient())
                    return (await workItemTrackingHttpClient.GetWorkItemAsync(workItemId, expand: WorkItemExpand.Relations))
                        .ToModel();
            });
        }

        private WorkItemTrackingHttpClient GetWorkItemTrackingHttpClient() {
            VssBasicCredential credentials = new VssBasicCredential("", _azureDevOpsServer.PersonalAccessToken);
            return new WorkItemTrackingHttpClient(new Uri(_azureDevOpsServer.Url), credentials);
        }

        /// <summary>
        /// Récupération des itérations courantes d'un projet
        /// </summary>
        /// <param name="project">Le nom du projet</param>
        /// <param name="team">Le nom de l'équipe</param>
        public async Task<List<TeamIteration>> GetCurrentTeamIterationsAsync(string project, string team = null) {
            if (string.IsNullOrEmpty(project))
                throw new ArgumentException("La projet est obligatoire", nameof(project));
            return await TryCatchExceptionAsync(async () => {
                using (WorkHttpClient workHttpClient = GetWorkHttpClient())
                    return (await workHttpClient.GetTeamIterationsAsync(new TeamContext(project, team), "Current"))
                           .Select(t => t.ToModel())
                           .ToList();
            });
        }

        private WorkHttpClient GetWorkHttpClient() {
            VssBasicCredential credentials = new VssBasicCredential("", _azureDevOpsServer.PersonalAccessToken);
            return new WorkHttpClient(new Uri(_azureDevOpsServer.Url), credentials);
        }

        /// <summary>
        /// Création d'un WorkItem
        /// </summary>
        /// <param name="workItemFields">Les champs du WorkItem</param>
        /// <returns>Le WorkItem crée</returns>
        public async Task<WorkItem> CreateWorkItemAsync(WorkItemFields workItemFields) {
            if (workItemFields == null)
                throw new ArgumentNullException(nameof(workItemFields));
            ThrowIfNotValidForCreate(workItemFields);
            return await TryCatchExceptionAsync(async () => {
                using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient()) {
                    JsonPatchDocument document = CreateJsonPatchDocument(Operation.Add, workItemFields);
                    return (await workItemTrackingHttpClient.CreateWorkItemAsync(document, workItemFields.TeamProject,
                                                                                 workItemFields.WorkItemType)).ToModel();
                }
            });
        }

        private static void ThrowIfNotValidForCreate(WorkItemFields workItemFields) {
            if (workItemFields.AreaPath == "")
                throw new ArgumentException("La zone ne doit pas être vide", nameof(workItemFields.AreaPath));
            if (string.IsNullOrEmpty(workItemFields.TeamProject))
                throw new ArgumentException("Le projet est obligatoire", nameof(workItemFields.TeamProject));
            if (workItemFields.IterationPath == "")
                throw new ArgumentException("L'itération ne doit pas être vide", nameof(workItemFields.IterationPath));
            if (string.IsNullOrEmpty(workItemFields.Title))
                throw new ArgumentException("Le titre est obligatoire", nameof(workItemFields.Title));
            if (workItemFields.State == "")
                throw new ArgumentException("L'état ne doit pas être vide", nameof(workItemFields.State));
            if (string.IsNullOrEmpty(workItemFields.WorkItemType))
                throw new ArgumentException("Le type est obligatoire", nameof(workItemFields.WorkItemType));
        }

        private static async Task<T> TryCatchExceptionAsync<T>(Func<Task<T>> callback) {
            try {
                return await callback();
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        private static async Task TryCatchExceptionAsync(Func<Task> callback) {
            try {
                await callback();
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        private static JsonPatchDocument CreateJsonPatchDocument(Operation operation, WorkItemFields workItemFields) {
            Dictionary<string, string> fields = new Dictionary<string, string> {
                { "/fields/System.AreaPath", workItemFields.AreaPath },
                { "/fields/System.TeamProject", workItemFields.TeamProject },
                { "/fields/System.IterationPath", workItemFields.IterationPath },
                { "/fields/System.Title", workItemFields.Title },
                { "/fields/System.State", workItemFields.State },
                { "/fields/System.WorkItemType", workItemFields.WorkItemType },
                { "/fields/System.AssignedTo", workItemFields.AssignedToDisplayName },
                { "/fields/Microsoft.VSTS.Common.Activity", workItemFields.Activity },
                { "/fields/System.Description", workItemFields.Description },
                { "/fields/Microsoft.VSTS.TCM.ReproSteps", workItemFields.ReproSteps },
                { "/fields/Microsoft.VSTS.TCM.SystemInfo", workItemFields.SystemInfo },
                { "/fields/Microsoft.VSTS.Common.AcceptanceCriteria", workItemFields.AcceptanceCriteria }
            };
            return fields.Where(f => f.Value != null)
                         .Aggregate(new JsonPatchDocument(), (document, field) => {
                             document.Add(new JsonPatchOperation {
                                              Operation = operation,
                                              Path = field.Key,
                                              Value = field.Value
                                          });
                             return document;
                         });
        }

        /// <summary>
        /// Mise à jour d'un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem</param>
        /// <param name="workItemFields">Les champs du WorkItem à modifier</param>
        public async Task UpdateWorkItemAsync(int workItemId, WorkItemFields workItemFields) {
            if (workItemFields == null)
                throw new ArgumentNullException(nameof(workItemFields));
            if (workItemId <= 0)
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif",
                                            nameof(workItemId));
            ThrowIfNotValidForUpdate(workItemFields);
            await TryCatchExceptionAsync(async () => {
                using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient()) {
                    JsonPatchDocument jsonPatchDocument = CreateJsonPatchDocument(Operation.Replace, workItemFields);
                    if (jsonPatchDocument.Count > 0)
                        await workItemTrackingHttpClient.UpdateWorkItemAsync(jsonPatchDocument, workItemId);
                }
            });
        }

        private static void ThrowIfNotValidForUpdate(WorkItemFields workItemFields) {
            if (workItemFields.AreaPath == "")
                throw new ArgumentException("La zone ne doit pas être vide", nameof(workItemFields.AreaPath));
            if (workItemFields.TeamProject == "")
                throw new ArgumentException("Le projet ne doit pas être vide", nameof(workItemFields.TeamProject));
            if (workItemFields.IterationPath == "")
                throw new ArgumentException("L'itération ne doit pas être vide", nameof(workItemFields.IterationPath));
            if (workItemFields.Title == "")
                throw new ArgumentException("Le titre ne doit pas être vide", nameof(workItemFields.Title));
            if (workItemFields.State == "")
                throw new ArgumentException("L'état ne doit pas être vide", nameof(workItemFields.State));
            if (workItemFields.WorkItemType == "")
                throw new ArgumentException("Le type ne doit pas être vide", nameof(workItemFields.WorkItemType));
        }

        /// <summary>
        /// Ajoute une relation de WorkItem à un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem qui va recevoir la relation</param>
        /// <param name="workItemToAdd">Le WorkItem à ajouter</param>
        /// <param name="linkType">Le type de lien entre le WorkItem est celui que l'on veut ajouter</param>
        public async Task AddWorkItemRelationAsync(int workItemId, WorkItem workItemToAdd, LinkType linkType) {
            if (workItemId <= 0)
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif",
                                            nameof(workItemId));
            if (workItemToAdd == null)
                throw new ArgumentNullException(nameof(workItemToAdd));
            if (workItemId == workItemToAdd.Id)
                throw new ArgumentException("Impossible d'ajouter une relation sur lui même", nameof(workItemToAdd));
            await AddWorkItemRelationsAsync(workItemId, new List<WorkItem> { workItemToAdd }, linkType);
        }

        /// <summary>
        /// Ajoute plusieurs relations de WorkItems à un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem qui va recevoir la relation</param>
        /// <param name="workItemsToAdd">Les WorkItems à ajouter</param>
        /// <param name="linkType">Le type de lien entre le WorkItem est ceux que l'on veut ajouter</param>
        public async Task AddWorkItemRelationsAsync(int workItemId, List<WorkItem> workItemsToAdd, LinkType linkType) {
            if (workItemId <= 0)
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif",
                                            nameof(workItemId));
            if (workItemsToAdd == null)
                throw new ArgumentNullException(nameof(workItemsToAdd));
            if (linkType == LinkType.Parent && workItemsToAdd.Count > 1)
                throw new ArgumentException("Un WorkItem possède un seul parent", nameof(workItemsToAdd));
            await TryCatchExceptionAsync(async () => {
                using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient()) {
                    JsonPatchDocument jsonPatchDocument = workItemsToAdd.Aggregate(new JsonPatchDocument(), (document, field) => {
                        document.Add(new JsonPatchOperation {
                                         Operation = Operation.Add,
                                         Path = "/relations/-",
                                         Value = new {
                                             rel = linkType.GetName(),
                                             url = field.Url
                                         }
                                     });
                        return document;
                    });
                    if (jsonPatchDocument.Count > 0)
                        await workItemTrackingHttpClient.UpdateWorkItemAsync(jsonPatchDocument, workItemId);
                }
            });
        }

        /// <summary>
        /// Supprime un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem</param>
        public async Task DeleteWorkItemAsync(int workItemId) {
            if (workItemId <= 0)
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif",
                                            nameof(workItemId));
            await TryCatchExceptionAsync(async () => {
                using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient())
                    await workItemTrackingHttpClient.DeleteWorkItemAsync(workItemId);
            });
        }
    }
}
