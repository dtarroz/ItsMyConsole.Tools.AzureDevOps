using Microsoft.TeamFoundation.Work.WebApi;

namespace ItsMyConsole.Tools.AzureDevOps
{
    internal static class WebApiTeamSettingsIteration
    {
        public static TeamIteration ToModel(this TeamSettingsIteration teamSettingsIteration) {
            return new TeamIteration {
                Name = teamSettingsIteration.Name,
                Path = teamSettingsIteration.Path,
                StartDate = teamSettingsIteration.Attributes.StartDate,
                FinishDate = teamSettingsIteration.Attributes.FinishDate
            };
        }
    }
}
