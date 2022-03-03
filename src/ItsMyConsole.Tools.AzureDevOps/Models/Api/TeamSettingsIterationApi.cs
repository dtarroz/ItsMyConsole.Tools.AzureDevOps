namespace ItsMyConsole.Tools.AzureDevOps
{
    internal class TeamSettingsIterationApi
    {
        public TeamSettingsIterationApiValue[] Value { get; set; }
    }

    internal class TeamSettingsIterationApiValue
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public TeamSettingsIterationApiValueAttributes Attributes { get; set; }
    }

    internal class TeamSettingsIterationApiValueAttributes
    {
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
    }
}
