namespace ItsMyConsole.Tools.AzureDevOps
{
    internal static class LinkTypeExtensions
    {
        public static string GetName(this LinkType linkType) {
            switch (linkType) {
                case LinkType.Child:   return "System.LinkTypes.Hierarchy-Forward";
                case LinkType.Parent:  return "System.LinkTypes.Hierarchy-Reverse";
                case LinkType.Related: return "System.LinkTypes.Related";
                default:               return null;
            }
        }
    }
}
