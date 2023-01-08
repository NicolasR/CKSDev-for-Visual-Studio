using CKS.Dev.Core.Cmd.Shared;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;

namespace CKS.Dev.VisualStudio.SharePoint.Commands
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
        [SharePointCommand(StyleLibrarySharePointCommandIds.GetStyleLibraryProperties + CommandNaming.SUFFIX)]
        private static Dictionary<string, string> GetStyleLibraryProperties(ISharePointCommandContext context,
            StyleLibraryNodeInfo nodeInfo)
        {
#if V14
            throw new NotImplementedException();
#else
            return SharePointCommandServices.GetProperties(context.Site.GetCatalog(SPListTemplateType.DesignCatalog));
#endif
        }

        /// <summary>
        /// Get the default view url for the style library.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The default view of the style library</returns>
        [SharePointCommand(StyleLibrarySharePointCommandIds.GetStyleLibraryAllItemsUrl + CommandNaming.SUFFIX)]
        private static string GetStyleLibraryAllItemsUrl(ISharePointCommandContext context)
        {
#if V14
            throw new NotImplementedException();
#else
            return context.Site.GetCatalog(SPListTemplateType.DesignCatalog).DefaultViewUrl;
#endif
        }
    }
}
