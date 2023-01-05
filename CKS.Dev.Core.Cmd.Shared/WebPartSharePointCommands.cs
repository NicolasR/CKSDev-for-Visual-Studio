using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.SharePoint.Commands;
using System.Collections.Generic;
using System.IO;

namespace CKS.Dev.VisualStudio.SharePoint.Commands
{
    /// <summary>
    /// The web part commands
    /// </summary>
    internal class WebPartSharePointCommands
    {
        #region Methods

        /// <summary>
        /// Gets additional property data for a specific Web Part.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="nodeInfo">The node info</param>
        /// <returns>The properties</returns>
        [SharePointCommand(WebPartSharePointCommandIds.GetWebPartProperties)]
        private static Dictionary<string, string> GetWebPartProperties(ISharePointCommandContext context,
            FileNodeInfo nodeInfo)
        {
            SPList webParts = context.Site.GetCatalog(SPListTemplateType.WebPartCatalog);
            SPListItem webPart = webParts.Items[nodeInfo.UniqueId];

            return SharePointCommandServices.GetProperties(webPart);
        }

        /// <summary>
        /// Gets additional property data for a specific Web Part.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="nodeInfo">The node info</param>
        /// <returns>The properties</returns>
        [SharePointCommand(WebPartSharePointCommandIds.GetWebPartDefinition)]
        public static string GetWebPartDefinition(ISharePointCommandContext context, WebPartNodeInfo nodeInfo)
        {
            SPList webParts = context.Site.GetCatalog(SPListTemplateType.WebPartCatalog);
            SPListItem webPart = webParts.Items[nodeInfo.UniqueId];
            using (StreamReader reader = new StreamReader(webPart.File.OpenBinaryStream(SPOpenBinaryOptions.None)))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion
    }
}
