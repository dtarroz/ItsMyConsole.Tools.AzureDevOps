using System;
using System.Collections;
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
        private static readonly Dictionary<string, OptionsApiValue[]> AzureDevOpsOptions = new Dictionary<string, OptionsApiValue[]>();
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly object ThisLock = new object();

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
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif", nameof(workItemId));
            await LoadAzureDevOpsOptionsAsync();
            string query = $"?$expand=relations&api-version={GetVersionFromOptionId("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b")}";
            string url = CombineUrl(_azureDevOpsServer.Url, "_apis/wit/workitems", workItemId.ToString(), query);
            string content = await GetContentFromRequestAsync(HttpMethod.Get, url);
            WorkItemApi workItemApi = ConvertToWorkItemApi(content);
            return workItemApi.ToModel();
        }

        private static WorkItemApi ConvertToWorkItemApi(string json) {
            WorkItemApi workItemApi = JsonConvert.DeserializeObject<WorkItemApi>(json);
            if (workItemApi?.Fields.ContainsKey("System.AssignedTo") ?? false) {
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
            string query = $"$timeframe=current&api-version={GetVersionFromOptionId("c9175577-28a1-4b06-9197-8636af9f64ad")}";
            string endUrl = $"_apis/work/teamsettings/iterations?{query}";
            string url = CombineUrl(_azureDevOpsServer.Url, project, team, endUrl);
            string content = await GetContentFromRequestAsync(HttpMethod.Get, url);
            TeamSettingsIterationApi teamSettingsIterationApi = JsonConvert.DeserializeObject<TeamSettingsIterationApi>(content);
            return teamSettingsIterationApi.ToSingleModel();
        }

        private async Task LoadAzureDevOpsOptionsAsync() {
            if (!AzureDevOpsOptions.ContainsKey(GetAzureDevOpsServerName())) {
                string url = CombineUrl(_azureDevOpsServer.Url, "_apis");
                string content = await GetContentFromRequestAsync(HttpMethod.Options, url);
                OptionsApi optionsApi = JsonConvert.DeserializeObject<OptionsApi>(content);
                lock (ThisLock) {
                    if (!AzureDevOpsOptions.ContainsKey(GetAzureDevOpsServerName()))
                        AzureDevOpsOptions.Add(GetAzureDevOpsServerName(), optionsApi?.Value);
                }
            }
        }

        private static string CombineUrl(params string[] urlPaths) {
            return urlPaths.Where(u => !string.IsNullOrEmpty(u))
                           .Aggregate((url, urlPath) => {
                               string separator = urlPath.StartsWith("?") ? "" : "/";
                               return url.TrimEnd('/') + separator + urlPath.TrimStart('/');
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
                    switch (response.StatusCode) {
                        case HttpStatusCode.Unauthorized: {
                            string serverName = GetAzureDevOpsServerName();
                            throw new Exception($"Vous n'avez pas les accès au serveur Azure DevOps '{serverName}'");
                        }
                        case HttpStatusCode.NotFound:
                        case HttpStatusCode.BadRequest:
                        case HttpStatusCode.InternalServerError: {
                            string content = await response.Content.ReadAsStringAsync();
                            ExceptionApi exceptionApi = JsonConvert.DeserializeObject<ExceptionApi>(content);
                            throw new Exception(exceptionApi?.Message);
                        }
                        default: throw new Exception(response.ReasonPhrase);
                    }
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
        /// <param name="workItemCreateFields">Les champs du WorkItem</param>
        /// <returns>Le WorkItem crée</returns>
        public async Task<WorkItem> CreateWorkItemAsync(WorkItemCreateFields workItemCreateFields) {
            if (workItemCreateFields == null)
                throw new ArgumentNullException(nameof(workItemCreateFields));
            ThrowIfNotValidForCreate(workItemCreateFields);
            await LoadAzureDevOpsOptionsAsync();
            const string pathUrl = "_apis/wit/workitems";
            string type = "$" + workItemCreateFields.WorkItemType;
            string apiVersion = $"?api-version={GetVersionFromOptionId("62d3d110-0047-428c-ad3c-4fe872c91c74")}";
            string url = CombineUrl(_azureDevOpsServer.Url, workItemCreateFields.TeamProject, pathUrl, type, apiVersion);
            List<JsonPatchApi> listJsonPatchApi = ConvertToListJsonPatch("add", workItemCreateFields);
            string content = await GetContentFromRequestAsync(HttpMethod.Post, url, listJsonPatchApi);
            WorkItemApi workItemApi = ConvertToWorkItemApi(content);
            return workItemApi.ToModel();
        }

        private string GetVersionFromOptionId(string optionId) {
            OptionsApiValue[] optionApis = AzureDevOpsOptions[GetAzureDevOpsServerName()];
            return optionApis.First(o => o.Id == optionId).ReleasedVersion;
        }

        private static void ThrowIfNotValidForCreate(WorkItemCreateFields workItemCreateFields) {
            if (workItemCreateFields.AreaPath == "")
                throw new ArgumentException("La zone ne doit pas être vide", nameof(workItemCreateFields.AreaPath));
            if (string.IsNullOrEmpty(workItemCreateFields.TeamProject))
                throw new ArgumentException("Le projet est obligatoire", nameof(workItemCreateFields.TeamProject));
            if (workItemCreateFields.IterationPath == "")
                throw new ArgumentException("L'itération ne doit pas être vide", nameof(workItemCreateFields.IterationPath));
            if (string.IsNullOrEmpty(workItemCreateFields.Title))
                throw new ArgumentException("Le titre est obligatoire", nameof(workItemCreateFields.Title));
            if (workItemCreateFields.State == "")
                throw new ArgumentException("L'état ne doit pas être vide", nameof(workItemCreateFields.State));
            if (string.IsNullOrEmpty(workItemCreateFields.WorkItemType))
                throw new ArgumentException("Le type est obligatoire", nameof(workItemCreateFields.WorkItemType));
            if (workItemCreateFields.Tags != null && workItemCreateFields.Tags.Any(t => t?.Contains(";") ?? false))
                throw new ArgumentException("Une balise ne doit pas contenir de \";\"", nameof(workItemCreateFields.Tags));
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
                { "/fields/Microsoft.VSTS.Common.AcceptanceCriteria", workItemFields.AcceptanceCriteria },
                { "/fields/Microsoft.VSTS.Scheduling.Effort", workItemFields.Effort?.ToString() }
            };
            if (workItemFields is WorkItemCreateFields workItemCreateFields)
                fields.Add("/fields/System.Tags", ConvertTagsToSystemTags(workItemCreateFields.Tags));
            return fields.Where(f => f.Value != null)
                         .Aggregate(new List<JsonPatchApi>(), (list, field) => {
                             list.Add(new JsonPatchApi {
                                          Op = operation,
                                          Path = field.Key,
                                          Value = field.Value
                                      });
                             return list;
                         });
        }

        private static string ConvertTagsToSystemTags(IEnumerable<string> tags) {
            string[] cleanTags = tags?.Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();
            return cleanTags == null || cleanTags.Length == 0 ? null : string.Join(";", cleanTags);
        }

        /// <summary>
        /// Mise à jour d'un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem</param>
        /// <param name="workItemUpdateFields">Les champs du WorkItem à modifier</param>
        /// <returns>Le WorkItem mise à jour</returns>
        public async Task<WorkItem> UpdateWorkItemAsync(int workItemId, WorkItemUpdateFields workItemUpdateFields) {
            if (workItemUpdateFields == null)
                throw new ArgumentNullException(nameof(workItemUpdateFields));
            if (workItemId <= 0)
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif", nameof(workItemId));
            ThrowIfNotValidForUpdate(workItemUpdateFields);
            List<JsonPatchApi> listJsonPatchApi = ConvertToListJsonPatch("replace", workItemUpdateFields);
            return await UpdateWorkItemAsync(workItemId, listJsonPatchApi);
        }

        /// <summary>
        /// Mise à jour d'un WorkItem
        /// </summary>
        /// <param name="workItem">Le WorkItem</param>
        /// <param name="workItemUpdateFields">Les champs du WorkItem à modifier</param>
        /// <returns>Le WorkItem mise à jour</returns>
        public async Task<WorkItem> UpdateWorkItemAsync(WorkItem workItem, WorkItemUpdateFields workItemUpdateFields) {
            if (workItemUpdateFields == null)
                throw new ArgumentNullException(nameof(workItemUpdateFields));
            ThrowIfNotValidForUpdate(workItemUpdateFields);
            List<JsonPatchApi> listJsonPatchApi = ConvertToListJsonPatch("replace", workItemUpdateFields);
            return await UpdateWorkItemAsync(workItem.Id, listJsonPatchApi);
        }

        private async Task<WorkItem> UpdateWorkItemAsync(int workItemId, ICollection listJsonPatchApi) {
            await LoadAzureDevOpsOptionsAsync();
            string pathUrl = $"_apis/wit/workitems/{workItemId}";
            string apiVersion = $"?api-version={GetVersionFromOptionId("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b")}";
            string url = CombineUrl(_azureDevOpsServer.Url, pathUrl, apiVersion);
            if (listJsonPatchApi.Count > 0) {
                string content = await GetContentFromRequestAsync(new HttpMethod("PATCH"), url, listJsonPatchApi);
                WorkItemApi workItemApi = ConvertToWorkItemApi(content);
                return workItemApi.ToModel();
            }
            return await GetWorkItemAsync(workItemId);
        }

        private static void ThrowIfNotValidForUpdate(WorkItemUpdateFields workItemUpdateFields) {
            if (workItemUpdateFields.AreaPath == "")
                throw new ArgumentException("La zone ne doit pas être vide", nameof(workItemUpdateFields.AreaPath));
            if (workItemUpdateFields.TeamProject == "")
                throw new ArgumentException("Le projet ne doit pas être vide", nameof(workItemUpdateFields.TeamProject));
            if (workItemUpdateFields.IterationPath == "")
                throw new ArgumentException("L'itération ne doit pas être vide", nameof(workItemUpdateFields.IterationPath));
            if (workItemUpdateFields.Title == "")
                throw new ArgumentException("Le titre ne doit pas être vide", nameof(workItemUpdateFields.Title));
            if (workItemUpdateFields.State == "")
                throw new ArgumentException("L'état ne doit pas être vide", nameof(workItemUpdateFields.State));
            if (workItemUpdateFields.WorkItemType == "")
                throw new ArgumentException("Le type ne doit pas être vide", nameof(workItemUpdateFields.WorkItemType));
        }

        /// <summary>
        /// Ajoute une relation de WorkItem à un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem qui va recevoir la relation</param>
        /// <param name="workItemToAdd">Le WorkItem à ajouter</param>
        /// <param name="linkType">Le type de lien entre le WorkItem est celui que l'on veut ajouter</param>
        /// <returns>Le WorkItem mise à jour</returns>
        public async Task<WorkItem> AddWorkItemRelationAsync(int workItemId, WorkItem workItemToAdd, LinkType linkType) {
            if (workItemId <= 0)
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif", nameof(workItemId));
            if (workItemToAdd == null)
                throw new ArgumentNullException(nameof(workItemToAdd));
            if (workItemId == workItemToAdd.Id)
                throw new ArgumentException("Impossible d'ajouter une relation sur lui même", nameof(workItemToAdd));
            return await AddWorkItemRelationsAsync(workItemId, new List<WorkItem> { workItemToAdd }, linkType);
        }

        /// <summary>
        /// Ajoute plusieurs relations de WorkItems à un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem qui va recevoir la relation</param>
        /// <param name="workItemsToAdd">Les WorkItems à ajouter</param>
        /// <param name="linkType">Le type de lien entre le WorkItem est ceux que l'on veut ajouter</param>
        /// <returns>Le WorkItem mise à jour</returns>
        public async Task<WorkItem> AddWorkItemRelationsAsync(int workItemId, List<WorkItem> workItemsToAdd, LinkType linkType) {
            if (workItemId <= 0)
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif", nameof(workItemId));
            if (workItemsToAdd == null)
                throw new ArgumentNullException(nameof(workItemsToAdd));
            if (linkType == LinkType.Parent && workItemsToAdd.Count > 1)
                throw new ArgumentException("Un WorkItem possède un seul parent", nameof(workItemsToAdd));
            if (workItemsToAdd.Any(w => w == null))
                throw new ArgumentException("Un WorkItem à ajouter est à null", nameof(workItemsToAdd));
            if (workItemsToAdd.Any(w => w.Id == workItemId))
                throw new ArgumentException("Impossible d'ajouter une relation sur lui même", nameof(workItemsToAdd));
            List<JsonPatchApi> listJsonPatchApi = workItemsToAdd.Aggregate(new List<JsonPatchApi>(), (list, workItem) => {
                list.Add(new JsonPatchApi {
                             Op = "add",
                             Path = "/relations/-",
                             Value = new {
                                 Rel = linkType.GetName(),
                                 workItem.Url
                             }
                         });
                return list;
            });
            return await UpdateWorkItemAsync(workItemId, listJsonPatchApi);
        }

        /// <summary>
        /// Supprime un WorkItem
        /// </summary>
        /// <param name="workItemId">L'identifiant du WorkItem</param>
        public async Task DeleteWorkItemAsync(int workItemId) {
            if (workItemId <= 0)
                throw new ArgumentException("L'identifiant du WorkItem doit être un nombre strictement positif", nameof(workItemId));
            await LoadAzureDevOpsOptionsAsync();
            string apiVersion = $"?api-version={GetVersionFromOptionId("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b")}";
            string url = CombineUrl(_azureDevOpsServer.Url, "_apis/wit/workitems", workItemId.ToString(), apiVersion);
            await GetContentFromRequestAsync(HttpMethod.Delete, url);
        }
    }
}
