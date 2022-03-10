namespace ItsMyConsole.Tools.AzureDevOps
{
    internal class JsonPatchApi
    {
        public string Op { get; set; }
        public string Path { get; set; }
        public object Value { get; set; }
    }
}
