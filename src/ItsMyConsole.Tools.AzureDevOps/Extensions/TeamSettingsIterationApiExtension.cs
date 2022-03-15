using System;

namespace ItsMyConsole.Tools.AzureDevOps
{
    internal static class TeamSettingsIterationApiExtension
    {
        public static TeamIteration ToSingleModel(this TeamSettingsIterationApi teamSettingsIterationApi) {
            return new TeamIteration {
                Name = teamSettingsIterationApi.Value[0].Name,
                Path = teamSettingsIterationApi.Value[0].Path,
                StartDate = ConvertToDateTime(teamSettingsIterationApi.Value[0].Attributes.StartDate),
                FinishDate = ConvertToDateTime(teamSettingsIterationApi.Value[0].Attributes.FinishDate)
            };
        }

        private static DateTime? ConvertToDateTime(string date) {
            return date == null ? (DateTime?)null : DateTime.Parse(date).Date;
        }
    }
}
