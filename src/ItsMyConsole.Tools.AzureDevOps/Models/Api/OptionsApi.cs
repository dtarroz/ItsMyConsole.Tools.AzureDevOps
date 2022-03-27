namespace ItsMyConsole.Tools.AzureDevOps
{
    internal class OptionsApi
    {
        public OptionsApiValue[] Value { get; set; }
    }

    internal class OptionsApiValue
    {
        public string Id { get; set; }
        public string ReleasedVersion { get; set; }
    }
}
