using CKS.Dev.Core.Cmd.Imp.v4.Properties;
using CKS.Dev.VisualStudio.SharePoint.Commands.Common.ExtensionMethods;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;

namespace CKS.Dev.VisualStudio.SharePoint.Commands
{
    /// <summary>
    /// The web part gallery commands
    /// </summary>
    class WebPartGallerySharePointCommands
    {
        /// <summary>
        /// Gets data for each Web Part on the SharePoint site, and returns an array of 
        /// serializable objects that contain the data.
        /// </summary>
        /// <param name="context">The command context</param>
        /// <returns>The web part infos</returns>
        [SharePointCommand(WebPartGallerySharePointCommandIds.GetWebParts)]
        private static FileNodeInfo[] GetWebParts(ISharePointCommandContext context)
        {
            List<FileNodeInfo> nodeInfos = new List<FileNodeInfo>();
            try
            {
                context.Logger.WriteLine(Resources.WebPartGallerySharePointCommands_TryingToRetrieveAvailableWebParts, LogCategory.Status);

                SPListItemCollection webParts = context.Web.GetCatalog(SPListTemplateType.WebPartCatalog).GetItems(
                    new SPQuery
                    {
                        ViewXml = "<View />"
                    }
                );
                nodeInfos = webParts.ToFileNodeInfo();

                context.Logger.WriteLine(Resources.WebPartGallerySharePointCommands_RetrievingException, LogCategory.Status);
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.WebPartGallerySharePointCommands_RetrievingException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }

            return nodeInfos.ToArray();
        }

        /// <summary>
        /// Gets additional property data for a specific Web Part.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="nodeInfo"></param>
        /// <returns></returns>
        [SharePointCommand(WebPartGallerySharePointCommandIds.GetWebPartGalleryProperties)]
        private static Dictionary<string, string> GetWebPartGalleryProperties(ISharePointCommandContext context,
            WebPartGalleryNodeInfo nodeInfo)
        {
            return SharePointCommandServices.GetProperties(context.Site.GetCatalog(SPListTemplateType.WebPartCatalog));
        }

        /// <summary>
        /// Get the default view url for the web part gallery
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The default view of the web part gallery</returns>
        [SharePointCommand(WebPartGallerySharePointCommandIds.GetWebPartGalleryAllItemsUrl)]
        private static string GetWebPartGalleryAllItemsUrl(ISharePointCommandContext context)
        {
            return context.Site.GetCatalog(SPListTemplateType.WebPartCatalog).DefaultViewUrl;
        }
    }
}
