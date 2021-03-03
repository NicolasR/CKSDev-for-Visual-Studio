using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.SharePoint.Commands;
using System.Collections.Generic;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands
{
    /// <summary>
    /// The style library commands.
    /// </summary>
    class StyleLibrarySharePointCommands
    {
        /// <summary>
        /// Gets additional property data for the style library.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="nodeInfo">The node info.</param>
        /// <returns></returns>
        [SharePointCommand(StyleLibrarySharePointCommandIds.GetStyleLibraryProperties)]
        private static Dictionary<string, string> GetStyleLibraryProperties(ISharePointCommandContext context,
            StyleLibraryNodeInfo nodeInfo)
        {
            return SharePointCommandServices.GetProperties(context.Site.GetCatalog(SPListTemplateType.DesignCatalog));
        }

        /// <summary>
        /// Get the default view url for the style library.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The default view of the style library</returns>
        [SharePointCommand(StyleLibrarySharePointCommandIds.GetStyleLibraryAllItemsUrl)]
        private static string GetStyleLibraryAllItemsUrl(ISharePointCommandContext context)
        {
            return context.Site.GetCatalog(SPListTemplateType.DesignCatalog).DefaultViewUrl;
        }
    }
}
