namespace ItsMyConsole.Tools.AzureDevOps
{
    internal static class LinkTypeExtension
    {
        public static string GetName(this LinkType linkType) {
            switch (linkType) {
                case LinkType.Child:       return "System.LinkTypes.Hierarchy-Forward";
                case LinkType.Parent:      return "System.LinkTypes.Hierarchy-Reverse";
                case LinkType.Related:     return "System.LinkTypes.Related";
                case LinkType.Predecessor: return "System.LinkTypes.Dependency-Reverse";
                case LinkType.Successor:   return "System.LinkTypes.Dependency-Forward";
                default:                   return null;
            }
        }
    }
}
