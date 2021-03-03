using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.SharePoint.Commands;
using System.Collections.Generic;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands
{
    /// <summary>
    /// The design catalog commands.
    /// </summary>
    class DesignCatalogSharePointCommands
    {
        /// <summary>
        /// Gets additional property data for the design catalog.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="nodeInfo">The node info.</param>
        /// <returns></returns>
        [SharePointCommand(DesignCatalogSharePointCommandIds.GetDesignCatalogProperties)]
        private static Dictionary<string, string> GetDesignCatalogProperties(ISharePointCommandContext context,
            DesignCatalogNodeInfo nodeInfo)
        {
            return SharePointCommandServices.GetProperties(context.Site.GetCatalog(SPListTemplateType.DesignCatalog));
        }

        /// <summary>
        /// Get the default view url for the design catalog.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The default view of the design catalog</returns>
        [SharePointCommand(DesignCatalogSharePointCommandIds.GetDesignCatalogAllItemsUrl)]
        private static string GetDesignCatalogAllItemsUrl(ISharePointCommandContext context)
        {
            return context.Site.GetCatalog(SPListTemplateType.DesignCatalog).DefaultViewUrl;
        }
    }
}
