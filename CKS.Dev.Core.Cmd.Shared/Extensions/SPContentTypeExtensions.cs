using Microsoft.SharePoint;

namespace CKS.Dev.VisualStudio.SharePoint.Commands.Extensions
{
    /// <summary>
    /// Extension methods for the SPContentType type.
    /// </summary>
    public static class SPContentTypeExtensions
    {
        /// <summary>
        /// Safes the name.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        internal static string SafeName(this SPContentType contentType)
        {
            return ContentTypeSharePointCommands.SafeContentTypeName(contentType.Name);
        }
    }
}
