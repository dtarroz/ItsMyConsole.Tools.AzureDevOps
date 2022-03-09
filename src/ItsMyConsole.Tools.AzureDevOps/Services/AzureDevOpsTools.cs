using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ItsMyConsole.Tools.AzureDevOps
{
    /// <summary>
    /// Outils pour Azure DevOps
    /// </summary>
    public class AzureDevOpsTools
    {
        private readonly AzureDevOpsServer _azureDevOpsServer;
        private static readonly List<string> IsAzureDevOpsOptionsLoaded = new List<string>();
        private static readonly HttpClient HttpClient = new HttpClient();

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
            if (workItemId <= 0)
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif",
                                            nameof(workItemId));
            await LoadAzureDevOpsOptionsAsync();
            string url = CombineUrl(_azureDevOpsServer.Url, "_apis/wit/workitems", workItemId.ToString(), "?$expand=relations");
            string content = await GetContentFromRequestAsync(HttpMethod.Get, url);
            WorkItemApi workItemApi = ConvertToWorkItemApi(content);
            return workItemApi.ToModel();
        }

        private static WorkItemApi ConvertToWorkItemApi(string json) {
            WorkItemApi workItemApi = JsonConvert.DeserializeObject<WorkItemApi>(json);
            if (workItemApi.Fields.ContainsKey("System.AssignedTo")) {
                JObject assignedTo = workItemApi.Fields["System.AssignedTo"] as JObject;
                workItemApi.Fields["System.AssignedTo"] = assignedTo?.ToObject<WorkItemApiFieldsAssignedTo>();
            }
            return workItemApi;
        }

        /// <summary>
        /// Récupération de l'itération courante d'un projet
        /// </summary>
        /// <param name="project">Le nom du projet</param>
        /// <param name="team">Le nom de l'équipe</param>
        public async Task<TeamIteration> GetCurrentTeamIterationAsync(string project, string team = null) {
            if (string.IsNullOrEmpty(project))
                throw new ArgumentException("Le projet est obligatoire", nameof(project));
            await LoadAzureDevOpsOptionsAsync();
            const string endUrl = "_apis/work/teamsettings/iterations?$timeframe=current";
            string url = CombineUrl(_azureDevOpsServer.Url, project, team, endUrl);
            string content = await GetContentFromRequestAsync(HttpMethod.Get, url);
            TeamSettingsIterationApi teamSettingsIterationApi = JsonConvert.DeserializeObject<TeamSettingsIterationApi>(content);
            return teamSettingsIterationApi.ToSingleModel();
        }

        private async Task LoadAzureDevOpsOptionsAsync() {
            if (!IsAzureDevOpsOptionsLoaded.Contains(_azureDevOpsServer.Name)) {
                // use "OPTIONS _apis" for check authorization
                // future : check version and template route
                string url = CombineUrl(_azureDevOpsServer.Url, "_apis");
                await GetContentFromRequestAsync(HttpMethod.Options, url);
                IsAzureDevOpsOptionsLoaded.Add(_azureDevOpsServer.Name);
            }
        }

        private static string CombineUrl(params string[] urlPaths) {
            return urlPaths.Where(u => !string.IsNullOrEmpty(u))
                           .Aggregate((url, urlPath) => {
                               return url.TrimEnd('/') + (urlPath.StartsWith("?") ? "" : "/") + urlPath.TrimStart('/');
                           });
        }

        private async Task<string> GetContentFromRequestAsync(HttpMethod method, string url, object body = null) {
            using (HttpRequestMessage request = new HttpRequestMessage(method, url)) {
                request.Headers.Add("Authorization", $"Basic {GetAuthorizationBase64()}");
                if (body != null) {
                    const string mediaType = "application/json-patch+json";
                    request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, mediaType);
                }
                using (HttpResponseMessage response = await HttpClient.SendAsync(request)) {
                    if (response.IsSuccessStatusCode)
                        return await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Unauthorized) {
                        string serverName = GetAzureDevOpsServerName();
                        throw new Exception($"Vous n'avez pas les accès au serveur Azure DevOps '{serverName}'");
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest) {
                        string content = await response.Content.ReadAsStringAsync();
                        ExceptionApi exceptionApi = JsonConvert.DeserializeObject<ExceptionApi>(content);
                        throw new Exception(exceptionApi.Message);
                    }
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        private string GetAuthorizationBase64() {
            string authorization = $":{_azureDevOpsServer.PersonalAccessToken}";
            byte[] authorizationBytes = Encoding.UTF8.GetBytes(authorization);
            return Convert.ToBase64String(authorizationBytes);
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
            await LoadAzureDevOpsOptionsAsync();
            const string pathUrl = "_apis/wit/workitems";
            string type = "$" + workItemFields.WorkItemType;
            const string apiVersion = "?api-version=6.0";
            string url = CombineUrl(_azureDevOpsServer.Url, workItemFields.TeamProject, pathUrl, type, apiVersion);
            List<JsonPatchApi> listJsonPatchApi = ConvertToListJsonPatch("add", workItemFields);
            string content = await GetContentFromRequestAsync(HttpMethod.Post, url, listJsonPatchApi);
            WorkItemApi workItemApi = ConvertToWorkItemApi(content);
            return workItemApi.ToModel();
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

        private static List<JsonPatchApi> ConvertToListJsonPatch(string operation, WorkItemFields workItemFields) {
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
                         .Aggregate(new List<JsonPatchApi>(), (list, field) => {
                             list.Add(new JsonPatchApi {
                                          op = operation,
                                          path = field.Key,
                                          value = field.Value
                                      });
                             return list;
                         });
        }

        /// <summary>
        /// Mise à jour d'un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem</param>
        /// <param name="workItemFields">Les champs du WorkItem à modifier</param>
        /// <returns>Le WorkItem mise à jour</returns>
        public async Task<WorkItem> UpdateWorkItemAsync(int workItemId, WorkItemFields workItemFields) {
            if (workItemFields == null)
                throw new ArgumentNullException(nameof(workItemFields));
            if (workItemId <= 0)
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif",
                                            nameof(workItemId));
            ThrowIfNotValidForUpdate(workItemFields);
            await LoadAzureDevOpsOptionsAsync();
            string pathUrl = $"_apis/wit/workitems/{workItemId}";
            const string apiVersion = "?api-version=6.0";
            string url = CombineUrl(_azureDevOpsServer.Url, pathUrl, apiVersion);
            List<JsonPatchApi> listJsonPatchApi = ConvertToListJsonPatch("replace", workItemFields);
            if (listJsonPatchApi.Count > 0) {
                string content = await GetContentFromRequestAsync(new HttpMethod("PATCH"), url, listJsonPatchApi);
                WorkItemApi workItemApi = ConvertToWorkItemApi(content);
                return workItemApi.ToModel();
            }
            return await GetWorkItemAsync(workItemId);
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
            if (workItemsToAdd.Any(w => w == null))
                throw new ArgumentException("Un WorkItem à ajouter est à null", nameof(workItemsToAdd));
            if (workItemsToAdd.Any(w => w.Id == workItemId))
                throw new ArgumentException("Impossible d'ajouter une relation sur lui même", nameof(workItemsToAdd));
            await LoadAzureDevOpsOptionsAsync();
            string pathUrl = $"_apis/wit/workitems/{workItemId}";
            const string apiVersion = "?api-version=6.0";
            string url = CombineUrl(_azureDevOpsServer.Url, pathUrl, apiVersion);
            List<JsonPatchApi> listJsonPatchApi = workItemsToAdd.Aggregate(new List<JsonPatchApi>(), (list, workItem) => {
                list.Add(new JsonPatchApi {
                             op = "add",
                             path = "/relations/-",
                             value = new {
                                 rel = linkType.GetName(),
                                 url = workItem.Url
                             }
                         });
                return list;
            });
            if (listJsonPatchApi.Count > 0)
                await GetContentFromRequestAsync(new HttpMethod("PATCH"), url, listJsonPatchApi);
        }

        /// <summary>
        /// Supprime un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem</param>
        public async Task DeleteWorkItemAsync(int workItemId) {
            if (workItemId <= 0)
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif",
                                            nameof(workItemId));
            await LoadAzureDevOpsOptionsAsync();
            string url = CombineUrl(_azureDevOpsServer.Url, "_apis/wit/workitems", workItemId.ToString(), "?api-version=6.0");
            await GetContentFromRequestAsync(HttpMethod.Delete, url);
        }
    }
}
