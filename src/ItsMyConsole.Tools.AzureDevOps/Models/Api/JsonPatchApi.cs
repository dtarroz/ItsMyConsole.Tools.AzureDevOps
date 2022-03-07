namespace ItsMyConsole.Tools.AzureDevOps
{
    internal class JsonPatchApi
    {
        public string op { get; set; }
        public string path { get; set; }
        public object value { get; set; }
    }
}
