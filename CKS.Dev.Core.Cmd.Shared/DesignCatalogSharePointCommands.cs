using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;

namespace CKS.Dev.VisualStudio.SharePoint.Commands
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
#if V14
            throw new NotImplementedException();
#else
            return SharePointCommandServices.GetProperties(context.Site.GetCatalog(SPListTemplateType.DesignCatalog));
#endif
        }

        /// <summary>
        /// Get the default view url for the design catalog.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The default view of the design catalog</returns>
        [SharePointCommand(DesignCatalogSharePointCommandIds.GetDesignCatalogAllItemsUrl)]
        private static string GetDesignCatalogAllItemsUrl(ISharePointCommandContext context)
        {
#if V14
            throw new NotImplementedException();
#else
            return context.Site.GetCatalog(SPListTemplateType.DesignCatalog).DefaultViewUrl;
#endif
        }
    }
}
